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
        /// Represents a completed deployment.
        /// <see href="https://kubernetes.io/docs/concepts/workloads/controllers/deployment/#complete-deployment"/>
        /// </summary>
        Completed,

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