using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Creators;
using ModernApplicationFramework.Basics.CustomizeDialog.Views;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Core.Converters.Customize;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.CustomizeDialog.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(CommandsPageViewModel))]
    public sealed class CommandsPageViewModel : Screen
    {
        private CommandBarDefinitionBase _selectedMenuItem;
        private CommandBarDefinitionBase _selectedToolBarItem;
        private CommandBarDefinitionBase _selectedContextMenuItem;
        private CustomizeRadioButtonOptions _selectedOption;
        private CommandBarItemDefinition _selectedListBoxDefinition;
        private IEnumerable<CommandBarItemDefinition> _items;
        private ICommandsPageView _control;


        public ICommand HandleAddCommand => new Command(HandleCommandAdd);

        public ICommand HandleStylingFlagChangeCommand => new Command<object>(HandleStylingFlagChange, obj => true);

        public Command DropDownClickCommand => new Command(ExecuteDropDownClick);

        public IEnumerable<CommandBarDefinitionBase> CustomizableToolBars { get; set; }
        public IEnumerable<CommandBarDefinitionBase> CustomizableMenuBars { get; set; }
        public IEnumerable<CommandBarDefinitionBase> CustomizableContextMenus { get; set; }

        public CommandBarDefinitionBase SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                if (Equals(value, _selectedMenuItem))
                    return;
                _selectedMenuItem = value;
                NotifyOfPropertyChange();
                SetupListBoxItems(SelectedOption);
            }
        }

        public CommandBarDefinitionBase SelectedToolBarItem
        {
            get => _selectedToolBarItem;
            set
            {
                if (Equals(value, _selectedToolBarItem))
                    return;
                _selectedToolBarItem = value;
                NotifyOfPropertyChange();
                SetupListBoxItems(SelectedOption);
            }
        }

        public CommandBarDefinitionBase SelectedContextMenuItem
        {
            get => _selectedContextMenuItem;
            set
            {
                if (Equals(value, _selectedContextMenuItem))
                    return;
                _selectedContextMenuItem = value;
                NotifyOfPropertyChange();
                SetupListBoxItems(SelectedOption);
            }
        }

        public CustomizeRadioButtonOptions SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (value == _selectedOption)
                    return;
                _selectedOption = value;
                NotifyOfPropertyChange();
                SetupListBoxItems(value);
            }
        }

        public CommandBarItemDefinition SelectedListBoxDefinition
        {
            get => _selectedListBoxDefinition;
            set
            {
                if (_selectedListBoxDefinition == value)
                    return;
                if (value == null)
                    return;
                _selectedListBoxDefinition = value;
                NotifyOfPropertyChange();
            }
        }

        public IEnumerable<CommandBarItemDefinition> Items
        {
            get => _items;
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                NotifyOfPropertyChange();
            }
        }

        [ImportingConstructor]
        public CommandsPageViewModel()
        {
            DisplayName = "Commands";

            Items = new List<CommandBarItemDefinition>();

            CustomizableMenuBars =
                new ObservableCollection<CommandBarDefinitionBase>(IoC.Get<IMenuHostViewModel>()
                    .GetMenuHeaderItemDefinitions());
            CustomizableToolBars = IoC.Get<IToolBarHostViewModel>().ToolbarDefinitions;
            CustomizableContextMenus = IoC.Get<IContextMenuHost>().ContextMenuDefinitions;

            SelectedMenuItem = CustomizableMenuBars.FirstOrDefault();
            SelectedToolBarItem = CustomizableToolBars.FirstOrDefault();
            SelectedContextMenuItem = CustomizableContextMenus.FirstOrDefault();

            SetupListBoxItems(SelectedOption);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (view is ICommandsPageView correctView)
                _control = correctView;
            SelectedListBoxDefinition = Items.FirstOrDefault();
        }

        private void HandleStylingFlagChange(object value)
        {
            if (!(value is CommandBarFlags commandFlag))
                return;
            var allFlags = SelectedListBoxDefinition.Flags.AllFlags;
            var commandflag2 = ((CommandBarFlags) allFlags & ~StylingFlagsConverter.StylingMask) | commandFlag;
            SelectedListBoxDefinition.Flags.EnableStyleFlags(commandflag2);
        }

        private void SetupListBoxItems(CustomizeRadioButtonOptions value)
        {
            switch (value)
            {
                case CustomizeRadioButtonOptions.Menu:
                    var menuCreator = IoC.Get<IMenuCreator>();
                    Items = menuCreator.GetSingleSubDefinitions(SelectedMenuItem);
                    break;
                case CustomizeRadioButtonOptions.Toolbar:
                    var toolbarCreator = IoC.Get<IToolbarCreator>();
                    Items = toolbarCreator.GetToolBarItemDefinitions(SelectedToolBarItem);
                    break;
                case CustomizeRadioButtonOptions.ContextMenu:
                    var contextMenuCreator = IoC.Get<IContextMenuCreator>();
                    Items = contextMenuCreator.GetContextMenuItemDefinitions(SelectedContextMenuItem);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        private void HandleCommandAdd()
        {
            var windowManager = new WindowManager();
            var addCommandDialog = IoC.Get<IAddCommandDialogViewModel>();
            var nullable = windowManager.ShowDialog(addCommandDialog);
            if (!nullable.HasValue || !nullable.Value || addCommandDialog.SelectedItem == null)
                return;

            uint newSortOrder = SelectedListBoxDefinition.SortOrder;
            bool flag = SelectedListBoxDefinition.SortOrder > 0 &&
                        SelectedListBoxDefinition.CommandDefinition.ControlType == CommandControlTypes.Separator;
            var def = addCommandDialog.SelectedItem as CommandBarItemDefinition;
            if (def == null)
                return;

            switch (SelectedOption)
            {
                case CustomizeRadioButtonOptions.Menu:
                    AddNewMenuItem(def, newSortOrder, flag);
                    break;
                case CustomizeRadioButtonOptions.Toolbar:
                    break;
                case CustomizeRadioButtonOptions.ContextMenu:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }          
            SetupListBoxItems(SelectedOption);
            SelectedListBoxDefinition = def;
        }

        private void AddNewMenuItem(CommandBarItemDefinition def, uint newSortOrder, bool flag)
        {
            var model = IoC.Get<IMenuHostViewModel>();
            def.SortOrder = newSortOrder;

            if (!flag)
            {
                var definitionsToChange =
                    model.MenuItemDefinitions.Where(
                            x => x.Group == SelectedListBoxDefinition.Group)
                        .OrderBy(x => x.SortOrder);

                foreach (var definition in definitionsToChange)
                {
                    if (definition.Group != SelectedListBoxDefinition.Group)
                        continue;
                    if (definition.SortOrder >= newSortOrder)
                        definition.SortOrder++;
                }
            }
            def.Group = SelectedListBoxDefinition.Group;
            model.MenuItemDefinitions.Add(def);
        }


        private void ExecuteDropDownClick()
        {
            var dropDownMenu = _control.ModifySelectionButton.DropDownMenu;
            dropDownMenu.DataContext = SelectedListBoxDefinition;
            dropDownMenu.IsOpen = true;
        }
    }

    public enum CustomizeRadioButtonOptions
    {
        Menu,
        Toolbar,
        ContextMenu
    }
}