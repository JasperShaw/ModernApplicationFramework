using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.Hosts;
using ModernApplicationFramework.Basics.CustomizeDialog.Views;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
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
        private IEnumerable<CommandBarDefinitionBase> _customizableToolBars;
        private IEnumerable<CommandBarDefinitionBase> _customizableMenuBars;
        private IEnumerable<CommandBarDefinitionBase> _customizableContextMenus;

        public ICommand HandleAddCommand => new Command(HandleCommandAdd);
        public ICommand HandleDeleteCommand => new Command(HandleCommandDelete);
        public ICommand HandleAddNewMenuCommand => new Command(HandleCommandAddNewMenu);

        public ICommand HandleAddOrRemoveGroupCommand => new Command<object>(HandleCommandAddOrRemoveGroup, obj => true);

        public ICommand HandleStylingFlagChangeCommand => new Command<object>(HandleStylingFlagChange, obj => true);

        public Command DropDownClickCommand => new Command(ExecuteDropDownClick);

        public IEnumerable<CommandBarDefinitionBase> CustomizableToolBars
        {
            get => _customizableToolBars;
            set
            {
                if (Equals(value, _customizableToolBars)) return;
                _customizableToolBars = value;
                NotifyOfPropertyChange();
            }
        }

        public IEnumerable<CommandBarDefinitionBase> CustomizableMenuBars
        {
            get => _customizableMenuBars;
            set
            {
                if (Equals(value, _customizableMenuBars)) return;
                _customizableMenuBars = value;
                NotifyOfPropertyChange();
            }
        }

        public IEnumerable<CommandBarDefinitionBase> CustomizableContextMenus
        {
            get => _customizableContextMenus;
            set
            {
                if (Equals(value, _customizableContextMenus)) return;
                _customizableContextMenus = value;
                NotifyOfPropertyChange();
            }
        }

        public CommandBarDefinitionBase SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                if (Equals(value, _selectedMenuItem))
                    return;
                _selectedMenuItem = value;
                NotifyOfPropertyChange();
                BuildItemSources(SelectedOption);
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
                BuildItemSources(SelectedOption);
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
                BuildItemSources(SelectedOption);
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
                BuildItemSources(value);
            }
        }

        public CommandBarItemDefinition SelectedListBoxDefinition
        {
            get => _selectedListBoxDefinition;
            set
            {
                if (_selectedListBoxDefinition == value)
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

            BuildItemSources(SelectedOption);
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

        private void BuildItemSources(CustomizeRadioButtonOptions value)
        {
            switch (value)
            {
                case CustomizeRadioButtonOptions.Menu:
                    var menuCreator = IoC.Get<IMenuCreator>();
                    Items = menuCreator.GetSingleSubDefinitions(SelectedMenuItem);
                    CustomizableMenuBars =
                        new ObservableCollection<CommandBarDefinitionBase>(IoC.Get<IMenuHostViewModel>()
                            .GetMenuHeaderItemDefinitions());
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
            var def = addCommandDialog.SelectedItem;

            InternalAddItem(def);  
            BuildItemSources(SelectedOption);
            SelectedListBoxDefinition = def;
        }

        private void HandleCommandAddOrRemoveGroup(object value)
        {
            if (!(value is bool))
                return;
            GetModelAndParent(out ICommandBarHost model, out CommandBarDefinitionBase parent);

            var nextSelectedItem = SelectedListBoxDefinition;

            //Needs to be inverted as the Checkbox will be added before this code executes
            if (!(bool) value)
                model.DeleteGroup(SelectedListBoxDefinition.Group, parent, AppendTo.Previous);

            BuildItemSources(SelectedOption);
            SelectedListBoxDefinition = nextSelectedItem;
        }

        private void InternalAddItem(CommandBarItemDefinition definitionToAdd)
        {
            uint newSortOrder;
            bool flag;
            if (SelectedListBoxDefinition == null)
            {
                newSortOrder = 0;
                flag = false;
            }
            else
            {
                newSortOrder = SelectedListBoxDefinition.SortOrder;
                flag = SelectedListBoxDefinition.SortOrder > 0 &&
                       SelectedListBoxDefinition.CommandDefinition.ControlType == CommandControlTypes.Separator;
                definitionToAdd.Group = SelectedListBoxDefinition.Group;
            }
            definitionToAdd.SortOrder = newSortOrder;

            GetModelAndParent(out ICommandBarHost model, out CommandBarDefinitionBase parent);

            model.AddItemDefinition(definitionToAdd, parent, flag);
        }

        private void HandleCommandDelete()
        {
            if (SelectedListBoxDefinition == null)
                return;
            GetModelAndParent(out ICommandBarHost model, out CommandBarDefinitionBase parent);

            var nextSelectedItem = model.GetNextItemInGroup(SelectedListBoxDefinition) ??
                                   model.GetPreviousItem(SelectedListBoxDefinition, parent) ??
                                   model.GetNextItem(SelectedListBoxDefinition, parent);

            model.DeleteItemDefinition(SelectedListBoxDefinition, parent);
            BuildItemSources(SelectedOption);
            SelectedListBoxDefinition = nextSelectedItem;
        }

        private void HandleCommandAddNewMenu()
        {
            var newMenuItem = new MenuDefinition(null, 0, "New Menu", true);

            InternalAddItem(newMenuItem);
            BuildItemSources(SelectedOption);
            SelectedListBoxDefinition = newMenuItem;
        }

        private void GetModelAndParent(out ICommandBarHost host, out CommandBarDefinitionBase parent)
        {
            switch (SelectedOption)
            {
                case CustomizeRadioButtonOptions.Menu:
                    host = IoC.Get<IMenuHostViewModel>();
                    parent = SelectedMenuItem;
                    break;
                case CustomizeRadioButtonOptions.Toolbar:
                    host = IoC.Get<IToolBarHostViewModel>();
                    parent = SelectedToolBarItem;
                    break;
                case CustomizeRadioButtonOptions.ContextMenu:
                    host = IoC.Get<IContextMenuHost>();
                    parent = SelectedContextMenuItem;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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