using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.Customize.Views;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Basics.CommandBar.Hosts;
using ModernApplicationFramework.Core.Converters.AccessKey;
using ModernApplicationFramework.Core.Converters.Customize;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Basics.CommandBar.Customize.ViewModels
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
    [Export(typeof(ICustomizeDialogScreen))]
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
        private IEnumerable<CommandBarDataSource> _customizableContextMenus;
        private IEnumerable<CommandBarDataSource> _customizableMenuBars;
        private IEnumerable<CommandBarDataSource> _customizableToolBars;
        private IEnumerable<CommandBarItemDataSource> _items;
        private CommandBarDataSource _selectedContextMenuItem;
        private CommandBarItemDataSource _selectedListBoxDataSource;

        private CommandBarDataSource _selectedMenuItem;
        private CustomizeRadioButtonOptions _selectedOption;
        private CommandBarDataSource _selectedToolBarItem;
        private readonly ICommandBarDefinitionHost _definitionHost;

        public IEnumerable<CommandBarDataSource> CustomizableContextMenus
        {
            get => _customizableContextMenus;
            set
            {
                if (Equals(value, _customizableContextMenus)) return;
                _customizableContextMenus = value;
                NotifyOfPropertyChange();
            }
        }

        public IEnumerable<CommandBarDataSource> CustomizableMenuBars
        {
            get => _customizableMenuBars;
            set
            {
                if (Equals(value, _customizableMenuBars)) return;
                _customizableMenuBars = value;
                NotifyOfPropertyChange();
            }
        }

        public IEnumerable<CommandBarDataSource> CustomizableToolBars
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

        public ICommand ModifySelectionCommand => new Command(HandleModifySelection);

        public ICommand HandleStylingFlagChangeCommand => new DelegateCommand(HandleStylingFlagChange);

        public IEnumerable<CommandBarItemDataSource> Items
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

        public CommandBarDataSource SelectedContextMenuItem
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

        public CommandBarItemDataSource SelectedListBoxItem
        {
            get => _selectedListBoxDataSource;
            set
            {
                if (_selectedListBoxDataSource == value)
                    return;
                _selectedListBoxDataSource = value;
                NotifyOfPropertyChange();
            }
        }

        public CommandBarDataSource SelectedMenuItem
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

        public CommandBarDataSource SelectedToolBarItem
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

        public uint SortOrder => 1;

        [ImportingConstructor]
        public CommandsPageViewModel()
        {
            _menuCreator = IoC.Get<IMainMenuCreator>();
            _toolbarCreator = IoC.Get<IToolbarCreator>();
            _contextMenuCreator = IoC.Get<IContextMenuCreator>();
            _menuHost = IoC.Get<IMenuHostViewModel>();
            _toolbarHost = IoC.Get<IToolBarHostViewModel>();
            _contextMenuHost = IoC.Get<IContextMenuHost>();


            _definitionHost = IoC.Get<ICommandBarDefinitionHost>();

            DisplayName = Customize_Resources.CustomizeDialog_Commands;
            Items = new List<CommandBarItemDataSource>();
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
            SelectedListBoxItem = Items.FirstOrDefault();
        }

        private void BuildCheckBoxItems(CustomizeRadioButtonOptions value)
        {
            CommandBarDataSource selected;
            if ((value & CustomizeRadioButtonOptions.Menu) != 0)
            {
                selected = SelectedMenuItem;
                CustomizableMenuBars =
                    new ObservableCollection<CommandBarDataSource>(_menuHost.GetMenuHeaderItemDefinitions());
                SelectedMenuItem = selected;
            }
            if ((value & CustomizeRadioButtonOptions.Toolbar) != 0)
            {
                selected = SelectedToolBarItem;
                CustomizableToolBars =
                    new ObservableCollection<CommandBarDataSource>(Sort(_toolbarHost.GetMenuHeaderItemDefinitions().ToList()));
                SelectedToolBarItem = selected;
            }
            if ((value & CustomizeRadioButtonOptions.ContextMenu) != 0)
            {
                selected = SelectedContextMenuItem;
                CustomizableContextMenus =
                    new ObservableCollection<CommandBarDataSource>(Sort(_contextMenuHost.GetMenuHeaderItemDefinitions().ToList()));
                SelectedContextMenuItem = selected;
            }
        }

        private static IEnumerable<CommandBarDataSource> Sort(IReadOnlyCollection<CommandBarDataSource> list)
        {
            var dict = new List<KeyValuePair<CommandBarDataSource, string>>();
            var internalNames = list.Where(x => x is IHasInternalName).ToList();
            var remaining = list.Except(internalNames);
            foreach (var definition in internalNames)
            {
                if (definition is IHasInternalName internalNameDef)
                    dict.Add(new KeyValuePair<CommandBarDataSource, string>(definition,
                        internalNameDef.InternalName));
            }
            remaining.ForEach(x => dict.Add(new KeyValuePair<CommandBarDataSource, string>(x, x.Name)));
            var sorted = dict.OrderBy(x => x.Value).Select(x => x.Key);
            return sorted;
        }

        private void BuildItemSources(CustomizeRadioButtonOptions value)
        {
            if ((value & CustomizeRadioButtonOptions.Menu) != 0)
            {
                Items = BuildItemSourcesCore(_menuCreator, SelectedMenuItem);
            }
            if ((value & CustomizeRadioButtonOptions.Toolbar) != 0)
                Items = BuildItemSourcesCore(_toolbarCreator, SelectedToolBarItem);
            if ((value & CustomizeRadioButtonOptions.ContextMenu) != 0)
                Items = BuildItemSourcesCore(_contextMenuCreator, SelectedContextMenuItem);
        }

        private IEnumerable<CommandBarItemDataSource> BuildItemSourcesCore(ICreatorBase creator, CommandBarDataSource definition)
        {
            var groups = _definitionHost.ItemGroupDefinitions.Where(x => x.Parent == definition)
                .Where(x => !_definitionHost.ExcludedItemDefinitions.Contains(x))
                //.Where(x => x.Items.Any(y => y.IsVisible))
                .OrderBy(x => x.SortOrder)
                .ToList();
            return creator.GetSingleSubDefinitions(definition, groups, group =>
                {
                    return _definitionHost.ItemDefinitions.OfType<CommandBarItemDataSource>().Where(x => x.Group == group)
                        .Where(x => !_definitionHost.ExcludedItemDefinitions.Contains(x))
                        .OrderBy(x => x.SortOrder).ToList();
                },
                CommandBarCreationOptions.DisplaySeparatorsInAnyCase | CommandBarCreationOptions.DisplayInvisibleItems);
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
            var selectedItem = SelectedListBoxItem;

            GetModelAndParent(out var model, out var parent);

            model.MoveItem(SelectedListBoxItem, offset, parent);

            model.Build();
            BuildItemSources(SelectedOption);

            var newSelectedItem = Items.Where(x => x.Group == selectedItem.Group)
                                      .FirstOrDefault(x => x.SortOrder == selectedItem.SortOrder) ??
                                  Items.Where(x => x.Group == model.GetPreviousGroup(selectedItem.Group))
                                      .FirstOrDefault(x => x.SortOrder == selectedItem.SortOrder);

            SelectedListBoxItem = newSelectedItem;
        }


        private void ExecuteDropDownClick()
        {
            //var dropDownMenu = _control.ModifySelectionButton.DropDownMenu;
            //dropDownMenu.DataContext = SelectedListBoxItem;
            //dropDownMenu.IsOpen = true;
        }

        private CommandBarDataSource GetActiveItem()
        {
            if (SelectedOption.HasFlag(CustomizeRadioButtonOptions.Menu))
                return SelectedMenuItem;
            if (SelectedOption.HasFlag(CustomizeRadioButtonOptions.Toolbar))
                return SelectedToolBarItem;
            return SelectedContextMenuItem;
        }

        private void GetModelAndParent(out ICommandBarHost host, out CommandBarDataSource parent)
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
            SelectedListBoxItem = def;
        }

        private void HandleCommandAddNewMenu()
        {
            var newMenuItem = new MenuDataSource(Guid.Empty, CommandBarResources.NewMenuDefaultName, null, 0, true, CommandBarFlags.CommandFlagNone);

            InternalAddItem(newMenuItem);
            BuildItemSources(SelectedOption);
            BuildCheckBoxItems(SelectedOption);
            SelectedListBoxItem = newMenuItem;
        }

        private void HandleCommandAddOrRemoveGroup(object value)
        {
            if (!(value is bool))
                return;
            GetModelAndParent(out var model, out var parent);

            var nextSelectedItem = SelectedListBoxItem;

            //Needs to be inverted as the Checkbox will be added before this code executes
            if (!(bool) value && SelectedListBoxItem.PrecededBySeparator)
                model.DeleteGroup(SelectedListBoxItem.Group, AppendTo.Previous);
            else if ((bool) value && !SelectedListBoxItem.PrecededBySeparator)
                model.AddGroupAt(SelectedListBoxItem);

            BuildItemSources(SelectedOption);
            BuildCheckBoxItems(SelectedOption);
            SelectedListBoxItem = nextSelectedItem;
            model.Build(parent);
        }

        private void HandleCommandDelete()
        {
            if (SelectedListBoxItem == null)
                return;
            GetModelAndParent(out var model, out CommandBarDataSource _);

            var nextSelectedItem = model.GetNextItemInGroup(SelectedListBoxItem) ??
                                   model.GetPreviousItem(SelectedListBoxItem) ??
                                   model.GetNextItem(SelectedListBoxItem);

            model.DeleteItemDefinition(SelectedListBoxItem);
            BuildItemSources(SelectedOption);
            BuildCheckBoxItems(SelectedOption);
            SelectedListBoxItem = nextSelectedItem;
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
            GetModelAndParent(out var host, out _);
            host.Reset(item);
            BuildCheckBoxItems(SelectedOption);
            BuildItemSources(SelectedOption);
            foreach (var dataSource in Items)
                dataSource.Reset();
        }

        private void HandleResetItem()
        {
            SelectedListBoxItem.Reset();
        }

        private void HandleStylingFlagChange(object value)
        {
            if (!(value is CommandBarFlags commandFlag))
                return;
            var allFlags = SelectedListBoxItem.Flags.AllFlags & ~StylingFlagsConverter.StylingMask | commandFlag;
            var commandflag2 = (allFlags & ~StylingFlagsConverter.StylingMask) | commandFlag;
            SelectedListBoxItem.Flags.EnableStyleFlags(commandflag2);
        }

        private void HandleModifySelection()
        {
            var dialog = IoC.Get<ModifySelectionDialogViewModel>();
            dialog.Initialize(SelectedListBoxItem);
            var wm = IoC.Get<IWindowManager>();
            var result = wm.ShowDialog(dialog);
            if (!result.HasValue || !result.Value)
                return;
            if (dialog.Reset)
            {
                HandleResetItem();
            }
            else
            {
                HandleCommandAddOrRemoveGroup(dialog.BeginGroup);
                if (dialog.InitialCommandFlag == dialog.SelectedStyle)
                    return;
                HandleStylingFlagChange(dialog.SelectedStyle);
            }
        }

        private void InternalAddItem(CommandBarItemDataSource dataSourceToAdd)
        {
            uint newSortOrder;
            bool flag;
            if (SelectedListBoxItem == null)
            {
                newSortOrder = 0;
                flag = false;
            }
            else
            {
                newSortOrder = SelectedListBoxItem.SortOrder;
                flag = SelectedListBoxItem.SortOrder > 0 &&
                       SelectedListBoxItem.UiType == CommandControlTypes.Separator;
                dataSourceToAdd.Group = SelectedListBoxItem.Group;
            }
            dataSourceToAdd.SortOrder = newSortOrder;

            GetModelAndParent(out var model, out var parent);
            model.AddItemDefinition(dataSourceToAdd, parent, flag);
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