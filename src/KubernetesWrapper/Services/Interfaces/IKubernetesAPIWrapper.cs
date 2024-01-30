using KubernetesWrapper.Models;

namespace KubernetesWrapper.Services.Interfaces
{
    /// <summary>
    /// Interface for the kubernetes api wrapper
    /// </summary>
    public interface IKubernetesApiWrapper<T>
        where T : DeployedResource
    {
        /// <summary>
        /// Gets a list of deployed resources of a given type in the cluster. Parameters are described in further detail in the kubernetes api doc.
        /// </summary>
        /// <param name="resourceType">The deployed resource type to retrieve.</param>
        /// <param name="fieldSelector">A selector to restrict the list of returned objects by their fields. Defaults to everything</param>
        /// <param name="labelSelector">A selector to restrict the list of returned objects by their labels. Defaults to everything</param>

        /// <returns>A list of deployments</returns>
        Task<IList<T>> GetDeployedResources(
            ResourceType resourceType,
            string fieldSelector = null,
            string labelSelector = null);
    }
}
