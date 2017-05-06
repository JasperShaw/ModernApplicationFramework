using ModernApplicationFramework.Basics.SettingsDialog;

namespace ModernApplicationFramework.Basics
{
    public static class SettingsCategories
    {
        private static SettingsCategory _environmentSetting;

        public static SettingsCategory EnvironmentCategory => _environmentSetting ?? (_environmentSetting =  new SettingsCategory("Environment", 1));
    }
}