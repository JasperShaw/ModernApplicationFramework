using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.Extended.Settings.General
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(StorableEnvironmentGeneralOptions))]
    public class StorableEnvironmentGeneralOptions : AbstractSettingsDataModel
    {
        private readonly EnvironmentGeneralOptions _generalOptions;
        public override ISettingsCategory Category { get; }

        public override string Name => "General";

        [ImportingConstructor]
        public StorableEnvironmentGeneralOptions(ISettingsManager settingsManager, EnvironmentGeneralOptions generalOptions)
        {
            SettingsManager = settingsManager;
            _generalOptions = generalOptions;
            Category = SettingsCategories.EnvironmentCategory;
        }

        public override void LoadOrCreate()
        {
            SetPropertyFromSettings(_generalOptions, "ShowStatusBar", nameof(_generalOptions.ShowStatusBar), true);
            SetPropertyFromSettings(_generalOptions, "WindowMenuContainsNItems", nameof(_generalOptions.WindowListItems), 10);
            SetPropertyFromSettings(_generalOptions, "MRUListContainsNItems", nameof(_generalOptions.MRUListItems), 10);
            SetPropertyFromSettings<bool>(_generalOptions,"AutohidePinActiveTabOnly", nameof(_generalOptions.DockedWinAuto));
            SetPropertyFromSettings(_generalOptions,"CloseButtonActiveTabOnly", nameof(_generalOptions.DockedWinClose), true);
            SetPropertyFromSettings(_generalOptions,"UseTitleCaseOnMenu", nameof(_generalOptions.UseTitleCaseOnMenu), true);
            SetPropertyFromSettings(_generalOptions,"UseHardwareAcceleration", nameof(_generalOptions.UseHardwareAcceleration), _generalOptions.IsHardwareAccelerationSupported);
            SetPropertyFromSettings<bool>(_generalOptions,"RichClientExperienceOptions", nameof(_generalOptions.UseRichVisualExperience));
            SetPropertyFromSettings<bool>(_generalOptions,"AutoAdjustExperience", nameof(_generalOptions.AutoAdjustExperience));
        }

        public override void StoreSettings()
        {
            SetSettingsValue("ShowStatusBar", _generalOptions.ShowStatusBar);
            SetSettingsValue("WindowMenuContainsNItems", _generalOptions.WindowListItems);
            SetSettingsValue("MRUListContainsNItems", _generalOptions.MRUListItems);
            SetSettingsValue("AutohidePinActiveTabOnly", _generalOptions.DockedWinAuto);
            SetSettingsValue("CloseButtonActiveTabOnly", _generalOptions.DockedWinClose);
            SetSettingsValue("UseTitleCaseOnMenu", _generalOptions.UseTitleCaseOnMenu);
            SetSettingsValue("UseHardwareAcceleration", _generalOptions.UseHardwareAcceleration);
            SetSettingsValue("RichClientExperienceOptions", _generalOptions.UseRichVisualExperience);
            SetSettingsValue("AutoAdjustExperience", _generalOptions.AutoAdjustExperience);
        }
    }
}
