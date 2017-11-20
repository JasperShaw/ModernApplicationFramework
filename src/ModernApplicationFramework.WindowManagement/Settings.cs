using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsManager;

namespace ModernApplicationFramework.WindowManagement
{
    public static class Settings
    {
        public static Guid ToolsOptionsTabsAndWindowsSettingsCategoryId = new Guid("{4F0ED64D-AD7B-4AFE-8968-BAA6B228DD03}");

        [Export]
        public static ISettingsCategory TabsAndWindowsCategory =
            new SettingsCategory(ToolsOptionsTabsAndWindowsSettingsCategoryId,
                SettingsCategoryType.Normal, "Environment_TabsAndWindows", SettingsCategories.EnvironmentCategory);

        public static Guid WindowLayoutsSettingsCategoryId = new Guid("{0988CD77-C823-471A-BC44-376ABC6C44A2}");

        [Export]
        public static ISettingsCategory WindowLayoutsSettingsCategory =
            new SettingsCategory(WindowLayoutsSettingsCategoryId,
                SettingsCategoryType.Normal, "Environment_WindowLayoutStore", SettingsCategories.EnvironmentCategory);


        public static Guid CommandBarLayoutCategoryId = new Guid("{20234054-C143-4E02-BEED-BE19A185A5C3}");

        [Export]
        public static ISettingsCategory CommandBarLayoutCategory =
            new SettingsCategory(CommandBarLayoutCategoryId,
                SettingsCategoryType.Normal, "Environment_CommandBars", SettingsCategories.EnvironmentCategory);

    }
}
