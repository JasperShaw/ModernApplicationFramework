using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.SettingsDialog;

namespace ModernApplicationFramework.Basics
{
    public static class SettingsCategories
    {
        [Export] public static SettingsCategory EnvironmentCategory = new SettingsCategory("Environment", "Umgebung", 1);
    }
}