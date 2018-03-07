using System.ComponentModel.Composition;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsDialog;

namespace ModernApplicationFramework.EditorBase.Settings.Documents
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DocumentsSettingsViewModel : SettingsPage
    {
        private readonly ExternalChangeSettings _externalChangeSettings;
        private bool _detectFileChangesOutsideIde;
        private bool _autoloadExternalChanges;
        public override uint SortOrder => 4;
        public override string Name => DocumentsSettingsResources.DocumentsSettingsName;
        public override SettingsPageCategory Category => SettingsPageCategories.EnvironmentCategory;

        public bool DetectFileChangesOutsideIde
        {
            get => _detectFileChangesOutsideIde;
            set
            {
                if(_detectFileChangesOutsideIde == value)
                    return;
                DirtyObjectManager.SetData(_detectFileChangesOutsideIde, value);
                _detectFileChangesOutsideIde = value;
                OnPropertyChanged();
            }
        }

        public bool AutoloadExternalChanges
        {
            get => _autoloadExternalChanges;
            set
            {
                if (_autoloadExternalChanges == value)
                    return;
                DirtyObjectManager.SetData(_autoloadExternalChanges, value);
                _autoloadExternalChanges = value;
                OnPropertyChanged();
            }
        }

        public DocumentsSettingsViewModel()
        {
            _externalChangeSettings = ExternalChangeSettings.Instance;
            _autoloadExternalChanges = _externalChangeSettings.AutoloadExternalChanges;
            _detectFileChangesOutsideIde = _externalChangeSettings.DetectFileChangesOutsideIde;
        }

        protected override bool SetData()
        {
            _externalChangeSettings.AutoloadExternalChanges = AutoloadExternalChanges;
            _externalChangeSettings.DetectFileChangesOutsideIde = DetectFileChangesOutsideIde;
            return true;
        }

        public override bool CanApply()
        {
            return true;
        }

        public override void Activate()
        {
        }
    }
}
