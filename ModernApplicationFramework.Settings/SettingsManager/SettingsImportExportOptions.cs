using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.SettingsBase;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Settings;

namespace ModernApplicationFramework.Settings.SettingsManager
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(SettingsImportExportOptions))]
    public sealed class SettingsImportExportOptions : AbstractSettingsDataModel
    {
        private readonly IExtendedEnvironmentVarirables _environmentVarirables;
        public override ISettingsCategory Category => SettingsCategories.EnvironmentCategory;
        public override string Name => "Import and Export Settings";

        [ImportingConstructor]
        public SettingsImportExportOptions(ISettingsManager settingsManager, IExtendedEnvironmentVarirables environmentVarirables)
        {
            SettingsManager = settingsManager;
            _environmentVarirables = environmentVarirables;
            SettingsManager.SettingsLocationChanged += _settingsManager_SettingsLocationChanged;
        }

        private void _settingsManager_SettingsLocationChanged(object sender, System.EventArgs e)
        {
            StoreSettings();
        }

        public override void LoadOrCreate()
        {
            GetSettingsValue("AutoSaveFile", _environmentVarirables.SettingsFilePath);
        }

        public override void StoreSettings()
        {
            StoreSettingsValue("AutoSaveFile", _environmentVarirables.SettingsFilePath);
        }
    }
}
