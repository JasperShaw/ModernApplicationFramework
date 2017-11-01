namespace ModernApplicationFramework.WindowManagement.WindowProfile
{
    /// <summary>
    /// Service that provides default window profiles for the application
    /// </summary>
    public interface IDefaultWindowProfileProvider
    {
        /// <summary>
        /// Gets a window profile by name
        /// </summary>
        /// <param name="profileName">Name of the window profile.</param>
        /// <returns>The <see cref="WindowProfile"/></returns>
        WindowProfile GetLayout(string profileName);
    }
}
