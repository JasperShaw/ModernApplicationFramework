using System.ComponentModel;

namespace ModernApplicationFramework.Interfaces.Settings
{
    public interface ISettingsPage : ICanBeDirty, INotifyPropertyChanged
    {
        uint SortOrder { get; }
        string Name { get; }
		ISettingsCategory Category { get; }

        bool Apply();
        bool CanApply();

        /// <summary>
        /// Usually gets called when the Hosting Dialog Window gets active. It should reload/set data in the SettingsPage.
        /// This can be performence relevant, so only insert code that really needs to be reloaded manually.
        /// </summary>
        void Activate();
    }
}
