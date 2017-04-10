using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Creators;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.CommandBase;
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
        private IEnumerable _items;


        public ICommand HandleAddCommand => new Command(HandleCommandAdd);

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

        public IEnumerable Items
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
            CustomizableToolBars = IoC.Get<IToolBarHostViewModel>().ToolbarDefinitions;

            var menuHost = IoC.Get<IMenuHostViewModel>();

            IEnumerable<CommandBarDefinitionBase> barDefinitions = menuHost.MenuBars.OrderBy(x => x.SortOrder).ToList();
            IEnumerable<CommandBarDefinitionBase> menuDefinitions =
                menuHost.MenuDefinitions.OrderBy(x => x.SortOrder).ToList();
            IEnumerable<CommandBarDefinitionBase> submenus = menuHost.MenuItemDefinitions.Where(
                x => x.CommandDefinition == null || x.CommandDefinition.ControlType == CommandControlTypes.Menu);


            CustomizableMenuBars =
                new ObservableCollection<CommandBarDefinitionBase>(
                    barDefinitions.Concat(menuDefinitions.Concat(submenus)));

            CustomizableContextMenus = IoC.Get<IContextMenuHost>().ContextMenuDefinitions;


            Items = new List<CommandBarDefinitionBase>();


            SelectedMenuItem = CustomizableMenuBars.FirstOrDefault();
            SelectedToolBarItem = CustomizableToolBars.FirstOrDefault();
            SelectedContextMenuItem = CustomizableContextMenus.FirstOrDefault();

            SetupListBoxItems(SelectedOption);
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
            var addCommandDialog = new AddCommandDialogViewModel();
            windowManager.ShowDialog(addCommandDialog);
        }
    }

    public enum CustomizeRadioButtonOptions
    {
        Menu,
        Toolbar,
        ContextMenu
    }
}