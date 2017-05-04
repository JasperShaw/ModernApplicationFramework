using ModernApplicationFramework.Basics.SettingsDialog;

namespace ModernApplicationFramework.Interfaces
{
    public interface ISettingsPage
    {
        int SortOrder { get; }
        string Name { get; }
		SettingsCategory Category { get; }
        void Apply();
        bool CanApply();
    }
}
