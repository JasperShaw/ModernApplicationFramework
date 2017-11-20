using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.Hosts;
using ModernApplicationFramework.Basics.CustomizeDialog.Views;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Core.Converters.AccessKey;
using ModernApplicationFramework.Core.Converters.Customize;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Basics.CustomizeDialog.ViewModels
{
    /// <inheritdoc cref="ICommandsPageViewModel" />
    /// <summary>
    ///     Data view model implementing
    ///     <see cref="T:ModernApplicationFramework.Interfaces.ViewModels.ICommandsPageViewModel" />
    /// </summary>
    /// <seealso cref="T:Caliburn.Micro.Screen" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.ViewModels.ICommandsPageViewModel" />
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(ICommandsPageViewModel))]
    public sealed class CommandsPageViewModel : Screen, ICommandsPageViewModel
    {
        private static readonly AccessKeyRemovingConverter AccessKeyRemovingConverter = 
            new AccessKeyRemovingConverter();

        private static readonly ICommandBarLayoutBackupProvider LayoutBackupProvider =
            IoC.Get<ICommandBarLayoutBackupProvider>();

        private readonly IContextMenuCreator _contextMenuCreator;
        private readonly IContextMenuHost _contextMenuHost;
        private readonly IMainMenuCreator _menuCreator;
        private readonly IMenuHostViewModel _menuHost;
        private readonly IToolbarCreator _toolbarCreator;
        private readonly IToolBarHostViewModel _toolbarHost;
        private ICommandsPageView _control;
        private IEnumerable<CommandBarDefinitionBase> _customizableContextMenus;
        private IEnumerable<CommandBarDefinitionBase> _customizableMenuBars;
        private IEnumerable<CommandBarDefinitionBase> _customizableToolBars;
        private IEnumerable<CommandBarItemDefinition> _items;
        private CommandBarDefinitionBase _selectedContextMenuItem;
        private CommandBarItemDefinition _selectedListBoxDefinition;

        private CommandBarDefinitionBase _selectedMenuItem;
        private CustomizeRadioButtonOptions _selectedOption;
        private CommandBarDefinitionBase _selectedToolBarItem;

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

        public Command DropDownClickCommand => new Command(ExecuteDropDownClick);

        public ICommand HandleAddCommand => new Command(HandleCommandAdd);
        public ICommand HandleAddNewMenuCommand => new Command(HandleCommandAddNewMenu);

        public ICommand HandleAddOrRemoveGroupCommand => new DelegateCommand(HandleCommandAddOrRemoveGroup);
        public ICommand HandleDeleteCommand => new Command(HandleCommandDelete);
        public ICommand HandleMoveDownCommand => new Command(HandleCommandMoveDown);
        public ICommand HandleMoveUpCommand => new Command(HandleCommandMoveUp);
        public ICommand HandleResetItemCommand => new Command(HandleResetItem);

        public ICommand HandleStylingFlagChangeCommand => new DelegateCommand(HandleStylingFlagChange);

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

        public ICommand ResetAllCommand => new Command(HandleResetAll, () => LayoutBackupProvider.Backup != null);

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

        [ImportingConstructor]
        public CommandsPageViewModel()
        {
            _menuCreator = IoC.Get<IMainMenuCreator>();
            _toolbarCreator = IoC.Get<IToolbarCreator>();
            _contextMenuCreator = IoC.Get<IContextMenuCreator>();
            _menuHost = IoC.Get<IMenuHostViewModel>();
            _toolbarHost = IoC.Get<IToolBarHostViewModel>();
            _contextMenuHost = IoC.Get<IContextMenuHost>();

            DisplayName = Customize_Resources.CustomizeDialog_Commands;
            Items = new List<CommandBarItemDefinition>();
            BuildCheckBoxItems(CustomizeRadioButtonOptions.All);
            SelectedMenuItem = CustomizableMenuBars.FirstOrDefault();
            SelectedToolBarItem = CustomizableToolBars.FirstOrDefault();
            SelectedContextMenuItem = CustomizableContextMenus.FirstOrDefault();

            SelectedOption = CustomizeRadioButtonOptions.Menu;
            BuildItemSources(SelectedOption);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            BuildCheckBoxItems(CustomizeRadioButtonOptions.All);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (view is ICommandsPageView correctView)
                _control = correctView;
            SelectedListBoxDefinition = Items.FirstOrDefault();
        }

        private void BuildCheckBoxItems(CustomizeRadioButtonOptions value)
        {
            CommandBarDefinitionBase selected;
            if ((value & CustomizeRadioButtonOptions.Menu) != 0)
            {
                selected = SelectedMenuItem;
                CustomizableMenuBars =
                    new ObservableCollection<CommandBarDefinitionBase>(_menuHost.GetMenuHeaderItemDefinitions());
                SelectedMenuItem = selected;
            }
            if ((value & CustomizeRadioButtonOptions.Toolbar) != 0)
            {
                selected = SelectedToolBarItem;
                CustomizableToolBars =
                    new ObservableCollection<CommandBarDefinitionBase>(_toolbarHost.GetMenuHeaderItemDefinitions());
                SelectedToolBarItem = selected;
            }
            if ((value & CustomizeRadioButtonOptions.ContextMenu) != 0)
            {
                selected = SelectedContextMenuItem;
                CustomizableContextMenus =
                    new ObservableCollection<CommandBarDefinitionBase>(_contextMenuHost.GetMenuHeaderItemDefinitions());
                SelectedContextMenuItem = selected;
            }
        }

        private void BuildItemSources(CustomizeRadioButtonOptions value)
        {
            if ((value & CustomizeRadioButtonOptions.Menu) != 0)
                Items = _menuCreator.GetSingleSubDefinitions(SelectedMenuItem,
                    CommandBarCreationOptions.DisplaySeparatorsInAnyCase);
            if ((value & CustomizeRadioButtonOptions.Toolbar) != 0)
                Items = _toolbarCreator.GetSingleSubDefinitions(SelectedToolBarItem,
                    CommandBarCreationOptions.DisplaySeparatorsInAnyCase);
            if ((value & CustomizeRadioButtonOptions.ContextMenu) != 0)
                Items = _contextMenuCreator.GetSingleSubDefinitions(SelectedContextMenuItem,
                    CommandBarCreationOptions.DisplaySeparatorsInAnyCase);
        }

        private string ChooseResetFormatString()
        {
            if (SelectedOption.HasFlag(CustomizeRadioButtonOptions.Menu))
                return Customize_Resources.Prompt_ResetMenuConfirmation;
            if (SelectedOption.HasFlag(CustomizeRadioButtonOptions.Toolbar))
                return Customize_Resources.Prompt_ResetToolBarConfirmation;
            return Customize_Resources.Prompt_ResetContextMenuConfirmation;
        }

        private void DoMove(int offset)
        {
            var selectedItem = SelectedListBoxDefinition;

            GetModelAndParent(out var model, out var parent);

            model.MoveItem(SelectedListBoxDefinition, offset, parent);

            model.Build();
            BuildItemSources(SelectedOption);

            var newSelectedItem = Items.Where(x => x.Group == selectedItem.Group)
                                      .FirstOrDefault(x => x.SortOrder == selectedItem.SortOrder) ??
                                  Items.Where(x => x.Group == model.GetPreviousGroup(selectedItem.Group))
                                      .FirstOrDefault(x => x.SortOrder == selectedItem.SortOrder);

            SelectedListBoxDefinition = newSelectedItem;
        }


        private void ExecuteDropDownClick()
        {
            var dropDownMenu = _control.ModifySelectionButton.DropDownMenu;
            dropDownMenu.DataContext = SelectedListBoxDefinition;
            dropDownMenu.IsOpen = true;
        }

        private CommandBarDefinitionBase GetActiveItem()
        {
            if (SelectedOption.HasFlag(CustomizeRadioButtonOptions.Menu))
                return SelectedMenuItem;
            if (SelectedOption.HasFlag(CustomizeRadioButtonOptions.Toolbar))
                return SelectedToolBarItem;
            return SelectedContextMenuItem;
        }

        private void GetModelAndParent(out ICommandBarHost host, out CommandBarDefinitionBase parent)
        {
            switch (SelectedOption)
            {
                case CustomizeRadioButtonOptions.Menu:
                    host = _menuHost;
                    parent = SelectedMenuItem;
                    break;
                case CustomizeRadioButtonOptions.Toolbar:
                    host = _toolbarHost;
                    parent = SelectedToolBarItem;
                    break;
                case CustomizeRadioButtonOptions.ContextMenu:
                    host = _contextMenuHost;
                    parent = SelectedContextMenuItem;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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

        private void HandleCommandAddNewMenu()
        {
            var newMenuItem = new MenuDefinition(Guid.Empty, null, 0, CommandBarResources.NewMenuDefaultName, true);

            InternalAddItem(newMenuItem);
            BuildItemSources(SelectedOption);
            BuildCheckBoxItems(SelectedOption);
            SelectedListBoxDefinition = newMenuItem;
        }

        private void HandleCommandAddOrRemoveGroup(object value)
        {
            if (!(value is bool))
                return;
            GetModelAndParent(out var model, out var parent);

            var nextSelectedItem = SelectedListBoxDefinition;

            //Needs to be inverted as the Checkbox will be added before this code executes
            if (!(bool) value)
                model.DeleteGroup(SelectedListBoxDefinition.Group, AppendTo.Previous);
            else
                model.AddGroupAt(SelectedListBoxDefinition);

            BuildItemSources(SelectedOption);
            BuildCheckBoxItems(SelectedOption);
            SelectedListBoxDefinition = nextSelectedItem;
            model.Build(parent);
        }

        private void HandleCommandDelete()
        {
            if (SelectedListBoxDefinition == null)
                return;
            GetModelAndParent(out var model, out CommandBarDefinitionBase _);

            var nextSelectedItem = model.GetNextItemInGroup(SelectedListBoxDefinition) ??
                                   model.GetPreviousItem(SelectedListBoxDefinition) ??
                                   model.GetNextItem(SelectedListBoxDefinition);

            model.DeleteItemDefinition(SelectedListBoxDefinition);
            BuildItemSources(SelectedOption);
            BuildCheckBoxItems(SelectedOption);
            SelectedListBoxDefinition = nextSelectedItem;
        }

        private void HandleCommandMoveDown()
        {
            DoMove(1);
        }

        private void HandleCommandMoveUp()
        {
            DoMove(-1);
        }

        private void HandleResetAll()
        {
            var item = GetActiveItem();
            var message = string.Format(CultureInfo.CurrentCulture, ChooseResetFormatString(),
                AccessKeyRemovingConverter.Convert(item.Text, typeof(string), null,
                    CultureInfo.CurrentCulture));
            var title = IoC.Get<IEnvironmentVariables>().ApplicationName;
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question,
                MessageBoxResult.No);
            if (result == MessageBoxResult.No)
                return;
            IoC.Get<ICommandBarSerializer>().ResetFromBackup(LayoutBackupProvider.Backup, item);
            GetModelAndParent(out var host, out var _);
            host.Build();
            BuildCheckBoxItems(SelectedOption);
            BuildItemSources(SelectedOption);
        }

        private void HandleResetItem()
        {
            SelectedListBoxDefinition.Reset();
        }

        private void HandleStylingFlagChange(object value)
        {
            if (!(value is CommandBarFlags commandFlag))
                return;
            var allFlags = SelectedListBoxDefinition.Flags.AllFlags;
            var commandflag2 = ((CommandBarFlags) allFlags & ~StylingFlagsConverter.StylingMask) | commandFlag;
            SelectedListBoxDefinition.Flags.EnableStyleFlags(commandflag2);
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

            GetModelAndParent(out var model, out var parent);
            model.AddItemDefinition(definitionToAdd, parent, flag);
        }
    }

    [Flags]
    public enum CustomizeRadioButtonOptions
    {
        Menu = 1,
        Toolbar = 2,
        ContextMenu = 4,
        All = Menu | Toolbar | ContextMenu
    }
}