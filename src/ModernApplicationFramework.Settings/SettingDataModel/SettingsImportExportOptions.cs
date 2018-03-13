using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Interfaces;
using ModernApplicationFramework.Utilities.Interfaces.Settings;
using ISettingsCategory = ModernApplicationFramework.Settings.Interfaces.ISettingsCategory;
using ISettingsDataModel = ModernApplicationFramework.Settings.Interfaces.ISettingsDataModel;

namespace ModernApplicationFramework.Settings.SettingDataModel
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(SettingsImportExportOptions))]
    public sealed class SettingsImportExportOptions : SettingsDataModel
    {
        private readonly IExtendedEnvironmentVariables _environmentVariables;
        public override ISettingsCategory Category => SettingsCategories.ExportSettingsSubCategory;
        public override string Name => "Import and Export Settings";

        [ImportingConstructor]
        public SettingsImportExportOptions(ISettingsManager settingsManager, IExtendedEnvironmentVariables environmentVariables) 
            : base(settingsManager)
        {
            _environmentVariables = environmentVariables;
            settingsManager.SettingsLocationChanged += _settingsManager_SettingsLocationChanged;
        }

        private void _settingsManager_SettingsLocationChanged(object sender, System.EventArgs e)
        {
            StoreSettings();
        }

        public override void LoadOrCreate()
        {
            GetOrCreatePropertyValueSetting("AutoSaveFile", _environmentVariables.SettingsFilePath);
        }

        public override void StoreSettings()
        {
            SetPropertyValue("AutoSaveFile", _environmentVariables.SettingsFilePath);
        }
    }
}
