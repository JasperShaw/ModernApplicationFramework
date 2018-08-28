using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Core.Converters.Customize;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Basics.CommandBar.Customize.ViewModels
{
    [Export(typeof(ModifySelectionDialogViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class ModifySelectionDialogViewModel : Screen
    {
        private CommandBarItemDataSource _dataSource;
        private string _menuName;
        private string _comboboxWidth;
        private CommandBarFlags _selectedStyle;
        private bool _precededBySeparator;

        public CommandBarItemDataSource DataSource
        {
            get => _dataSource;
            private set
            {
                if (Equals(value, _dataSource)) return;
                _dataSource = value;
                NotifyOfPropertyChange();
            }
        }

        public CommandBarFlags SelectedStyle
        {
            get => _selectedStyle;
            set
            {
                if (value == _selectedStyle) return;
                _selectedStyle = value;
                NotifyOfPropertyChange(nameof(ChangeOccured));
            }
        }

        public bool Reset { get; private set; }

        public bool BeginGroup { get; private set; }

        public bool InitialPrecededBySeparator { get; private set; }

        public bool PrecededBySeparator
        {
            get => _precededBySeparator;
            set
            {
                if (value == _precededBySeparator) return;
                _precededBySeparator = value;
                NotifyOfPropertyChange(nameof(ChangeOccured));
            }
        }

        public bool ChangeOccured
        {
            get
            {
                if (InitialMenuName == MenuName && InitialPrecededBySeparator == PrecededBySeparator && InitialCommandFlag == SelectedStyle)
                    return InitialComboboxWidth != ComboboxWidth;
                return true;
            }
        }

        public IEnumerable<CommandBarFlags> CommandFlags { get; }

        public ModifySelectionDialogViewModel()
        {
            CommandFlags = new List<CommandBarFlags>
            {
                CommandBarFlags.CommandFlagNone,
                CommandBarFlags.CommandFlagText,
                CommandBarFlags.CommandFlagPict,
                CommandBarFlags.CommandFlagPictAndText
            };
        }

        public ICommand CancelCommand => new Command(Cancel);


        public ICommand SubmitCommand => new DelegateCommand(Submit);


        public ICommand ResetCommand => new DelegateCommand(HandleReset);


        public CommandBarFlags InitialCommandFlag { get; private set; }

        public string ComboboxWidth
        {
            get => _comboboxWidth;
            set
            {
                if (value == _comboboxWidth) return;
                _comboboxWidth = value;
                NotifyOfPropertyChange(nameof(ChangeOccured));
            }
        }

        public string InitialComboboxWidth { get; private set; }

        public string MenuName
        {
            get => _menuName;
            set
            {
                if (value == _menuName) return;
                _menuName = value;
                NotifyOfPropertyChange(nameof(ChangeOccured));
            }
        }

        public string InitialMenuName { get; private set; }

        //public bool Reset { get; private set; }

        internal void Initialize(CommandBarItemDataSource dataSource)
        {
            DataSource = dataSource;

            if (!(dataSource is ComboBoxDataSource comboBox))
                InitialComboboxWidth = string.Empty;
            else
                InitialComboboxWidth = comboBox.DropDownWidth.ToString();
            ComboboxWidth = InitialComboboxWidth;
            InitialMenuName = DataSource.Text;
            MenuName = InitialMenuName;

            InitialPrecededBySeparator = DataSource.PrecededBySeparator;
            PrecededBySeparator = InitialPrecededBySeparator;

            InitialCommandFlag = DataSource.Flags.AllFlags & StylingFlagsConverter.StylingMask;
            SelectedStyle = InitialCommandFlag;
        }


        private void Cancel(object obj)
        {
            TryClose(false);
        }

        private void Submit(object obj)
        {
            if (InitialMenuName != MenuName)
                DataSource.Text = MenuName;
            if (InitialPrecededBySeparator != PrecededBySeparator)
                BeginGroup = PrecededBySeparator;
            if (InitialComboboxWidth != ComboboxWidth && DataSource is ComboBoxDataSource comboBoxDataSource)
                comboBoxDataSource.DropDownWidth = double.Parse(ComboboxWidth);
            TryClose(true);
        }

        private void HandleReset(object obj)
        {
            Reset = true;
            TryClose(true);
        }
    }
}

