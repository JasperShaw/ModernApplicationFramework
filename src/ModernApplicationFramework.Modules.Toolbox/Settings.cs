using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsManager;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public static class Settings
    {
        public static Guid ToolboxStateCategoryId = new Guid("{23D3EAC3-4D63-473E-A1AE-51EB1A9DA127}");

        [Export] public static ISettingsCategory ToolboxStateCategoryCategory =
            new SettingsCategory(ToolboxStateCategoryId,
                SettingsCategoryType.Normal, "Environment_Toolbox", SettingsCategories.EnvironmentCategory);
    }
}
