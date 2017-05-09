namespace LeanTest.Core.ExecutionHandling
{
    /// <summary>
    /// Determines how the context builder factory creates a 'clean' context.
    /// </summary>
    public enum CleanContextMode
    {
        /// <summary>
        /// Re-create is the easy way.
        /// </summary>
        ReCreate,
        /// <summary>
        /// Re-use requires more work, as e.g. an IoC container must have caches (and other singletons) cleared.
        /// </summary>
        ReUse
    }
}