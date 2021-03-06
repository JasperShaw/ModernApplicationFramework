using System.ComponentModel.Composition;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsManager;

namespace ModernApplicationFramework.Extended.Settings
{
    public static class ToolsOptionsSubCategories
    {
        [Export] public static ISettingsCategory GeneralSubCategory =
            new SettingsCategory(SettingsGuids.ToolsOptionsGeneralSettingsCategoryId,
                SettingsCategoryType.ToolsOptionSub, "General", SettingsCategories.ToolsOptionEnvironmentCategory);
    }
}