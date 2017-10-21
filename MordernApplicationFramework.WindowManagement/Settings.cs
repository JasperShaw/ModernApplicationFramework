using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsManager;

namespace MordernApplicationFramework.WindowManagement
{
    public static class Settings
    {
        public static Guid ToolsOptionsTabsAndWindowsSettingsCategoryId = new Guid("{4F0ED64D-AD7B-4AFE-8968-BAA6B228DD03}");

        [Export]
        public static ISettingsCategory TabsAndWindowsSubCategory =
            new SettingsCategory(ToolsOptionsTabsAndWindowsSettingsCategoryId,
                SettingsCategoryType.Normal, "TabsAndWindows", SettingsCategories.EnvironmentCategory);
    }
}
