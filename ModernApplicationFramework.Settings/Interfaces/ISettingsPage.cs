using System.ComponentModel;
using ModernApplicationFramework.Settings.SettingsDialog;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Settings.Interfaces
{
    /// <inheritdoc cref="ICanBeDirty" />
    /// <summary>
    /// This interface holds the data model for the UI-Page inside a dialog window
    /// </summary>
    public interface ISettingsPage : ICanBeDirty, INotifyPropertyChanged
    {
        /// <summary>
        /// The sorting order of the settings page
        /// </summary>
        uint SortOrder { get; }

        /// <summary>
        /// The name of the settings page
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The category of the settings page
        /// </summary>
        SettingsPageCategory Category { get; }

        /// <summary>
        /// Applies all entered settings. 
        /// </summary>
        /// <returns><see langword="true"/> when the entered values allow the settings dialog to close; <see langword="false"/> if not</returns>
        bool Apply();

        /// <summary>
        /// Evaluates if all entered values are valid.
        /// </summary>
        /// <returns><see langword="true"/> when the entered settings are valid; <see langowrd="false"/>when not</returns>
        bool CanApply();

        /// <summary>
        /// Usually gets called when the Hosting Dialog Window gets active. It should reload/set data in the SettingsPage.
        /// This can be performence relevant, so only insert code that really needs to be reloaded manually.
        /// </summary>
        void Activate();
    }
}
