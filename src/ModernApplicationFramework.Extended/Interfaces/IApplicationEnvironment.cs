namespace ModernApplicationFramework.Extended.Interfaces
{
    /// <summary>
    /// Global application environment that holds essential information and routines. 
    /// </summary>
    public interface IApplicationEnvironment
    {
        /// <summary>
        /// The local application data path.
        /// </summary>
        string LocalAppDataPath { get; }

        /// <summary>
        /// Gets the app data path for this application.
        /// </summary>
        string AppDataPath { get; }

        /// <summary>
        /// Indicates whether this application uses the xml based settings
        /// </summary>
        bool UseApplicationSettings { get; }

        /// <summary>
        /// Setups the application.
        /// </summary>
        void Setup();

        /// <summary>
        /// Prepares the application to close.
        /// </summary>
        void PrepareClose();
    }
}