using System.ComponentModel.Composition;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsManager;

namespace ModernApplicationFramework.Extended.Settings
{
    public static class ToolsOptionsSubCategories
    {
        [Export] public static ISettingsCategory GeneralSubCategory =
            new SettingsCategory(GuidCollection.ToolsOptionsGeneralSettingsCategoryId,
                SettingsCategoryType.ToolsOptionSub, "General", SettingsCategories.ToolsOptionEnvironmentCategory);

        [Export]
        public static ISettingsCategory TabsAndWindowsSubCategory =
            new SettingsCategory(GuidCollection.ToolsOptionsTabsAndWindowsSettingsCategoryId,
                SettingsCategoryType.ToolsOptionSub, "TabsAndWindows", SettingsCategories.ToolsOptionEnvironmentCategory);
    }
}