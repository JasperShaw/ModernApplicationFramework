using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Interfaces.ViewModels;
using Screen = Caliburn.Micro.Screen;

namespace ModernApplicationFramework.Basics.CustomizeDialog.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(CommandsPageViewModel))]
    public sealed class CommandsPageViewModel : Screen
    {
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
                x => x.CommandDefinition == null || x.CommandDefinition.ControlType == Definitions.Command.CommandControlTypes.Menu);


            CustomizableMenuBars =
                new ObservableCollection<CommandBarDefinitionBase>(barDefinitions.Concat(menuDefinitions.Concat(submenus)));

        }

        public IEnumerable<CommandBarDefinitionBase> CustomizableToolBars { get; set; }
        public IEnumerable<CommandBarDefinitionBase> CustomizableMenuBars { get; set; }

        public ICommand HandleAddCommand => new Command(HandleCommandAdd);

        private void HandleCommandAdd()
        {
            var windowManager = new WindowManager();
            var addCommandDialog = new AddCommandDialogViewModel();
            windowManager.ShowDialog(addCommandDialog);
        }
    }
}
