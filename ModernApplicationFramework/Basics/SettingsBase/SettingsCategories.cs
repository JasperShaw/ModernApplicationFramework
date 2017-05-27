using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Basics.SettingsBase
{
    public static class SettingsCategories
    {
        [Export] public static SettingsCategory EnvironmentCategory = new SettingsCategory("Environment", "Umgebung", 1);
    }
}