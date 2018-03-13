using System.ComponentModel.Composition;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Settings.SettingsManager;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.EditorBase.Settings.Documents
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(ExternalChangeSettings))]
    public class ExternalChangeSettings : SettingsDataModel
    {
        [Export]
        public static ISettingsCategory DocumentsSubCategory =
            new SettingsCategory(Guids.DocumentsToolsOptionsCategoryId,
                SettingsCategoryType.ToolsOptionSub, "Documents", SettingsCategories.ToolsOptionEnvironmentCategory);

        private bool _detectFileChangesOutsideIde;
        private bool _autoloadExternalChanges;

        public static ExternalChangeSettings Instance { get; private set; }

        public override ISettingsCategory Category => DocumentsSubCategory;

        public override string Name => "Documents";


        public bool DetectFileChangesOutsideIde
        {
            get => _detectFileChangesOutsideIde;
            set
            {
                if (value == _detectFileChangesOutsideIde) return;
                _detectFileChangesOutsideIde = value;
                OnPropertyChanged();
            }
        }

        public bool AutoloadExternalChanges
        {
            get => _autoloadExternalChanges;
            set
            {
                if (value == _autoloadExternalChanges) return;
                _autoloadExternalChanges = value;
                OnPropertyChanged();
            }
        }

        [ImportingConstructor]
        public ExternalChangeSettings(ISettingsManager settingsManager) : base(settingsManager)
        {
            Instance = this;
        }


        public override void LoadOrCreate()
        {
            SetClassPropertyFromPropertyValue(this, "DetectFileChangesOutsideIDE", nameof(DetectFileChangesOutsideIde), true);
            SetClassPropertyFromPropertyValue(this, "AutoloadExternalChanges", nameof(AutoloadExternalChanges), false);
        }

        public override void StoreSettings()
        {
            SetPropertyValue("DetectFileChangesOutsideIDE", DetectFileChangesOutsideIde);
            SetPropertyValue("AutoloadExternalChanges", AutoloadExternalChanges);
        }
    }
}
