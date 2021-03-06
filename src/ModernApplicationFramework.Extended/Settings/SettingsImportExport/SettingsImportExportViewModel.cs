﻿using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsDialog;
using ModernApplicationFramework.Utilities.Interfaces;
using ModernApplicationFramework.Utilities.Settings;

namespace ModernApplicationFramework.Extended.Settings.SettingsImportExport
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SettingsImportExportViewModel : SettingsPage
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IExtendedEnvironmentVariables _environmentVariables;
        private readonly IDialogProvider _dialogProvider;
        private string _settingsPath;
        public override uint SortOrder => 5;
        public override string Name => SettingsImportExportResources.SettingsImportExport_Name;
        public override SettingsPageCategory Category => SettingsPageCategories.EnvironmentCategory;

        public ICommand OpenFileDialogCommand => new Command(DoOpenFileDialog);

        private void DoOpenFileDialog()
        {
            var sd = new SaveFileDialog
            {
                Title = SettingsImportExportResources.SaveFileDialog_Title,
                Filter = SettingsImportExportResources.SaveFileDialog_Filter,
                InitialDirectory = Path.GetDirectoryName(_settingsPath)
            };
            if (sd.ShowDialog() == true)
                SettingsPath = sd.FileName;
        }

        public string SettingsPath
        {
            get => _settingsPath;
            set
            {
                if (_settingsPath == value)
                    return;
                DirtyObjectManager.SetData(_settingsPath, value);
                _settingsPath = value;
                OnPropertyChanged();
            }
        }

        [ImportingConstructor]
        public SettingsImportExportViewModel(ISettingsManager settingsManager, IExtendedEnvironmentVariables environmentVariables,
            IDialogProvider dialogProvider)
        {
            _settingsManager = settingsManager;
            _environmentVariables = environmentVariables;
            _dialogProvider = dialogProvider;
            _settingsPath = environmentVariables.SettingsFilePath;
        }

        protected override bool SetData()
        {
            if (string.IsNullOrEmpty(SettingsPath))
            {
                _dialogProvider.Warn(SettingsImportExportResources.SelectedPathNull_Warning);
                return false;
            }
            _settingsManager.ChangeSettingsFileLocation(SettingsPath, false);
            return true;
        }

        public override bool CanApply()
        {
            return true;
        }

        public override void Activate()
        {
            _settingsPath = _environmentVariables.SettingsFilePath;
        }
    }
}
