namespace Telerik.Sitefinity.Frontend
{
    /// <summary>
    /// Exposes methods for initializig and reverting the initialization (uninitializing).
    /// </summary>
    internal interface IInitializer
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Uninitializes this instance.
        /// </summary>
        void Uninitialize();
    }
}
