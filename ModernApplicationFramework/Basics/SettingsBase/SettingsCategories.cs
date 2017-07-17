using System.ComponentModel.Composition;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Basics.SettingsBase
{
    public static class SettingsCategories
    {
        [Export] public static SettingsCategory EnvironmentCategory = new SettingsCategory("Environment", CommonUI_Resources.SettingsCategory_Environment, 1);
    }
}