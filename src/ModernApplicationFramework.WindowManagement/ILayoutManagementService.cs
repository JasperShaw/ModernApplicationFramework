namespace ModernApplicationFramework.WindowManagement
{
    public interface ILayoutManagementService
    {
        /// <summary>
        /// Loads the or creates a window profile. Active option is <see cref="ProcessStateOption.ToolsOnly"/>
        /// </summary>
        /// <param name="profileName">The name of the window profile</param>
        void LoadOrCreateProfile(string profileName);

        /// <summary>
        /// Loads the or creates a window profile.
        /// </summary>
        /// <param name="profileName">The name of the window profile</param>
        /// <param name="loadOptions">The load options. Default option is <see cref="ProcessStateOption.ToolsOnly"/> </param>
        void LoadOrCreateProfile(string profileName, ProcessStateOption loadOptions);

        /// <summary>
        /// Saves the layout configuration to the active window profile. Default option is <see cref="ProcessStateOption.ToolsOnly"/>
        /// </summary>
        void SaveActiveFrameLayout();

        /// <summary>
        /// Saves the layout configuration to the active window profile. Default option is <see cref="ProcessStateOption.ToolsOnly"/>
        /// </summary>
        /// <param name="loadOption">The load option.</param>
        void SaveActiveFrameLayout(ProcessStateOption loadOption);
      
        /// <summary>
        /// Reloads the window profiles and creates a backup.
        /// </summary>
        void Reload();
    }
}