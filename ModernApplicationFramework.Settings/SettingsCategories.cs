using System.ComponentModel.Composition;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsManager;

namespace ModernApplicationFramework.Settings
{
    /// <summary>
    /// A basic set of setting categories
    /// </summary>
    public static class SettingsCategories
    {
        [Export] public static ISettingsCategory ToolsOptionEnvironmentCategory =
            new SettingsCategory(GuidCollection.ToolOptionsEnvironmentSettingsCategoryId,
                SettingsCategoryType.ToolsOption, "Environment", null);

        [Export]
        public static ISettingsCategory EnvironmentCategory =
            new SettingsCategory(GuidCollection.EnvironmentSettingsCategoryId, SettingsCategoryType.Normal, "Environment_Group", null);
    }
}