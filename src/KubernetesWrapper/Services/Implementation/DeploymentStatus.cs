namespace KubernetesWrapper.Services.Implementation
{
    /// <summary>
    /// Represents the status of a deployment in Kubernetes.
    /// <see href="https://kubernetes.io/docs/concepts/workloads/controllers/deployment/#deployment-status"/>
    /// </summary>
    public enum DeploymentStatus
    {
        /// <summary>
        /// Represents a progressing deployment.
        /// <see href="https://kubernetes.io/docs/concepts/workloads/controllers/deployment/#progressing-deployment"/>
        /// </summary>
        Progressing,

        /// <summary>
        /// Represents a partially available deployment.
        /// </summary>
        PartiallyAvailable,

        /// <summary>
        /// Represents an available and completed deployment.
        /// <see href="https://kubernetes.io/docs/concepts/workloads/controllers/deployment/#complete-deployment"/>
        /// </summary>
        Available,

        /// <summary>
        /// Represents a paused deployment.
        /// <see href="https://kubernetes.io/docs/concepts/workloads/controllers/deployment/#pausing-and-resuming-a-deployment"/>
        /// </summary>
        Paused,

        /// <summary>
        /// Represents a failed deployment.
        /// <see href="https://kubernetes.io/docs/concepts/workloads/controllers/deployment/#failed-deployment"/>
        /// </summary>
        Failed,
    }
}
