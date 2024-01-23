using k8s;
using k8s.Models;

namespace KubernetesWrapper.Models
{
    /// <summary>
    /// Class describing a deployed entity i kubernetes
    /// </summary>
    public abstract class DeployedResource
    {
        /// <summary>
        /// Gets or sets the version of the deployed entity, the image tag number
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets release name
        /// </summary>
        public string Release { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public V1DeploymentStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the number of desired pods
        /// </summary>
        public int? Replicas { get; set; }
    }
}
