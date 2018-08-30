using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Utilities.Settings;

namespace ModernApplicationFramework.Extended.Settings.General
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(StorableEnvironmentGeneralOptions))]
    public class StorableEnvironmentGeneralOptions : SettingsDataModel
    {
        private readonly EnvironmentGeneralOptions _generalOptions;
        public override ISettingsCategory Category { get; }

        public override string Name => "General";

        [ImportingConstructor]
        public StorableEnvironmentGeneralOptions(ISettingsManager settingsManager, EnvironmentGeneralOptions generalOptions) : base(settingsManager)
        {
            _generalOptions = generalOptions;
            Category = ToolsOptionsSubCategories.GeneralSubCategory;
        }

        public override void LoadOrCreate()
        {
            SetClassPropertyFromPropertyValue(_generalOptions, "ShowStatusBar", nameof(_generalOptions.ShowStatusBar), true);
            SetClassPropertyFromPropertyValue(_generalOptions, "WindowMenuContainsNItems", nameof(_generalOptions.WindowListItems), 10);
            SetClassPropertyFromPropertyValue(_generalOptions, "MRUListContainsNItems", nameof(_generalOptions.MRUListItems), 10);
            SetClassPropertyFromPropertyValue<bool>(_generalOptions,"AutohidePinActiveTabOnly", nameof(_generalOptions.DockedWinAuto));
            SetClassPropertyFromPropertyValue(_generalOptions,"CloseButtonActiveTabOnly", nameof(_generalOptions.DockedWinClose), true);
            SetClassPropertyFromPropertyValue(_generalOptions,"UseTitleCaseOnMenu", nameof(_generalOptions.UseTitleCaseOnMenu), true);
            SetClassPropertyFromPropertyValue(_generalOptions,"UseHardwareAcceleration", nameof(_generalOptions.UseHardwareAcceleration), _generalOptions.IsHardwareAccelerationSupported);
            SetClassPropertyFromPropertyValue<bool>(_generalOptions,"RichClientExperienceOptions", nameof(_generalOptions.UseRichVisualExperience));
            SetClassPropertyFromPropertyValue<bool>(_generalOptions,"AutoAdjustExperience", nameof(_generalOptions.AutoAdjustExperience));
        }

        public override void StoreSettings()
        {
            SetPropertyValue("ShowStatusBar", _generalOptions.ShowStatusBar);
            SetPropertyValue("WindowMenuContainsNItems", _generalOptions.WindowListItems);
            SetPropertyValue("MRUListContainsNItems", _generalOptions.MRUListItems);
            SetPropertyValue("AutohidePinActiveTabOnly", _generalOptions.DockedWinAuto);
            SetPropertyValue("CloseButtonActiveTabOnly", _generalOptions.DockedWinClose);
            SetPropertyValue("UseTitleCaseOnMenu", _generalOptions.UseTitleCaseOnMenu);
            SetPropertyValue("UseHardwareAcceleration", _generalOptions.UseHardwareAcceleration);
            SetPropertyValue("RichClientExperienceOptions", _generalOptions.UseRichVisualExperience);
            SetPropertyValue("AutoAdjustExperience", _generalOptions.AutoAdjustExperience);
        }
    }
}
