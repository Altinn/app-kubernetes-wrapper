using k8s;
using k8s.Models;

using KubernetesWrapper.Models;
using KubernetesWrapper.Services.Interfaces;

namespace KubernetesWrapper.Services.Implementation
{
    /// <summary>
    ///  An implementation of the Kubernetes API wrapper
    /// </summary>
    public class KubernetesApiWrapper<T> : IKubernetesApiWrapper<T>
        where T : DeployedResource
    {
        private readonly Kubernetes _client;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KubernetesApiWrapper{T}"/> class
        /// </summary>
        /// <param name="logger">The logger</param>
        public KubernetesApiWrapper(ILogger<KubernetesApiWrapper<T>> logger)
        {
            _logger = logger;
            try
            {
                var config = KubernetesClientConfiguration.InClusterConfig();
                _client = new Kubernetes(config);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to initialize KubernetesApiWrapper");
            }
        }

        /// <inheritdoc/>
        public async Task<IList<T>> GetDeployedResources(
            ResourceType resourceType,
            string fieldSelector = null,
            string labelSelector = null)
        {
            IList<T> mappedResources = new List<T>();

            switch (resourceType)
            {
                case ResourceType.Deployment:
                    V1DeploymentList deployments = await _client.ListNamespacedDeploymentAsync("default", fieldSelector: fieldSelector, labelSelector: labelSelector);
                    mappedResources = MapDeployments(deployments.Items).Cast<T>().ToList();
                    break;
                case ResourceType.DaemonSet:
                    V1DaemonSetList deamonSets = await _client.ListNamespacedDaemonSetAsync("default", fieldSelector: fieldSelector, labelSelector: labelSelector);
                    mappedResources = MapDaemonSets(deamonSets.Items).Cast<T>().ToList();
                    break;
            }

            return mappedResources;
        }

        /// <summary>
        /// Maps a list of k8s.Models.V1DaemonSet to DaemonSet
        /// </summary>
        /// <param name="list">The list to be mapped</param>
        private static IList<DaemonSet> MapDaemonSets(IList<V1DaemonSet> list)
        {
            IList<DaemonSet> mappedList = new List<DaemonSet>();
            if (list == null || list.Count == 0)
            {
                return mappedList;
            }

            foreach (V1DaemonSet element in list)
            {
                IList<V1Container> containers = element.Spec?.Template?.Spec?.Containers;
                if (containers != null && containers.Count > 0)
                {
                    DaemonSet daemonSet = new DaemonSet { Release = element.Metadata?.Name };

                    string[] splittedVersion = containers[0].Image?.Split(":");
                    if (splittedVersion != null && splittedVersion.Length > 1)
                    {
                        daemonSet.Version = splittedVersion[1];
                    }

                    mappedList.Add(daemonSet);
                }
            }

            return mappedList;
        }

        /// <summary>
        /// Maps a list of k8s.Models.V1Deployment to Deployment
        /// </summary>
        /// <param name="list">The list to be mapped</param>
        private static IList<Deployment> MapDeployments(IList<V1Deployment> list)
        {
            IList<Deployment> mappedList = new List<Deployment>();
            if (list == null || list.Count == 0)
            {
                return mappedList;
            }

            foreach (V1Deployment element in list)
            {
                Deployment deployment = new Deployment();
                IList<V1Container> containers = element.Spec?.Template?.Spec?.Containers;
                if (containers != null && containers.Count > 0)
                {
                    string[] splittedVersion = containers[0].Image?.Split(":");
                    if (splittedVersion != null && splittedVersion.Length > 1)
                    {
                        deployment.Version = splittedVersion[1];
                    }
                }

                var labels = element.Metadata?.Labels;

                if (labels != null && labels.TryGetValue("release", out string release))
                {
                    deployment.Release = release;
                }

                deployment.Status = GetDeploymentStatus(element).ToString();

                int desiredReplicas = element.Spec.Replicas ?? 0;
                int availableReplicas = element.Status.AvailableReplicas ?? 0;
                deployment.AvailabilityPercentage = (int)Math.Round((double)availableReplicas / desiredReplicas * 100, MidpointRounding.ToEven);

                mappedList.Add(deployment);
            }

            return mappedList;
        }

        /// <summary>
        /// Determines the status of a Kubernetes deployment.
        /// </summary>
        /// <param name="element">The V1Deployment object representing the Kubernetes deployment.</param>
        /// <returns>A DeploymentStatus enum value representing the status of the deployment.</returns>
        private static DeploymentStatus GetDeploymentStatus(V1Deployment element)
        {
            var progressingCondition = element.Status.Conditions.FirstOrDefault(condition => condition.Type == "Progressing");
            if (progressingCondition?.Status == "True")
            {
                var available = element.Status.Conditions.Any(condition => condition.Type == "Available" && condition.Status == "True");
                if (available)
                {
                    if (progressingCondition.Reason == "NewReplicaSetAvailable")
                    {
                        return DeploymentStatus.Completed;
                    }
                    else
                    {
                        return DeploymentStatus.Available;
                    }
                }
                else
                {
                    return DeploymentStatus.Progressing;
                }
            }

            if (progressingCondition?.Reason == "DeploymentPaused")
            {
                return DeploymentStatus.Paused;
            }

            return DeploymentStatus.Failed;
        }
    }
}
