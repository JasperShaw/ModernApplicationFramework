﻿using System.ComponentModel.Composition;
using System.Globalization;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.SettingsDialog;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.Settings
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GeneralWindowItemListSettingsViewModel : AbstractSettingsPage
    {
        private readonly EnvironmentGeneralOptions _generalOptions;
        private string _windowListItems;
        public override uint SortOrder => 1;
        public override string Name => Basics.Settings.General.GeneralSettingsResources.GeneralSettings_Name;
        public override SettingsCategory Category => SettingsCategories.EnvironmentCategory;


        [ImportingConstructor]
        public GeneralWindowItemListSettingsViewModel(EnvironmentGeneralOptions generalOptions)
        {
            _generalOptions = generalOptions;
            WindowListItems = generalOptions.WindowListItems.ToString();
        }

        public string WindowListItems
        {
            get => _windowListItems;
            set
            {
                if (value == _windowListItems)
                    return;
                DirtyObjectManager.SetData(_windowListItems, value);
                _windowListItems = value;
                OnPropertyChanged();
            }
        }

        protected override bool SetData()
        {
            if (!PreValidate(out int number))
            {
                ShowErrorMessage();
                return false;
            }
            _generalOptions.WindowListItems = number;
            _generalOptions.Save();
            return true;
        }

        private bool PreValidate(out int number)
        {
            if (!int.TryParse(WindowListItems, NumberStyles.Integer, CultureInfo.CurrentCulture, out number))
                return false;
            return EnvironmentGeneralOptions.MinFileListCount <= number && number <= EnvironmentGeneralOptions.MaxFileListCount;
        }

        private void ShowErrorMessage()
        {
            IoC.Get<IDialogProvider>().Warn(string.Format(CultureInfo.CurrentCulture,
                GeneralSettingsResources.Error_ItemsListCountNotMatching, EnvironmentGeneralOptions.MinFileListCount,
                EnvironmentGeneralOptions.MaxFileListCount));
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