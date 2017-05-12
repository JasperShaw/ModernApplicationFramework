using System.ComponentModel;
using ModernApplicationFramework.Basics.SettingsDialog;

namespace ModernApplicationFramework.Interfaces
{
    public interface ISettingsPage : INotifyPropertyChanged
    {
        uint SortOrder { get; }
        string Name { get; }
		SettingsCategory Category { get; }

        bool Apply();
        bool CanApply();

        /// <summary>
        /// Usually gets called when the Hosting Dialog Window gets active. It should reload/set data in the SettingsPage.
        /// This can be performence relevant, so only insert code that really needs to be reloaded manually.
        /// </summary>
        void Activate();
    }
}
