using System.Text.Json.Serialization;

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
        [JsonPropertyOrder(-2)]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets release name
        /// </summary>
        [JsonPropertyOrder(-1)]
        public string Release { get; set; }
    }
}
