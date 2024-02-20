using k8s.Models;

namespace KubernetesWrapper.Models
{
    /// <summary>
    /// Class describing a deployment
    /// </summary>
    public class Deployment : DeployedResource
    {
        /// <summary>
        /// Gets or sets the status of the deployment.
        /// This represents the current state of the deployment in the Kubernetes cluster, such as 'Available', 'Progressing', 'Failed', etc.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the resource is available.
        /// </summary>
        public bool Available { get; set; }

        /// <summary>
        /// Gets or sets the availability percentage of the deployment.
        /// This represents the percentage of pods that are up and running compared to the total number of pods in the deployment.
        /// </summary>
        public int AvailabilityPercentage { get; set; }
    }
}
