using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;

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

            //var topLevelMenuItems = IoC.GetAll<MenuItemDefinitionOld>().Where(x => !x.HasParent);

            //CustomizableMenuBars = new ObservableCollection<MenuItemDefinitionOld>(topLevelMenuItems);
        }


        public ObservableCollectionEx<ToolbarDefinition> CustomizableToolBars { get; set; }
        //public ObservableCollection<MenuItemDefinitionOld> CustomizableMenuBars { get; set; }
    }
}
