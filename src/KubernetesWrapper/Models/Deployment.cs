using k8s.Models;
using KubernetesWrapper.Services.Implementation;

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
        public DeploymentStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the status date of the deployment.
        /// </summary>
        public DateTime? StatusDate { get; set; }
    }
}
