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
    }
}
