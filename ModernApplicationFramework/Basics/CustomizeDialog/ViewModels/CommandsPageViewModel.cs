using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions;
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

            var topLevelMenuItems = IoC.GetAll<MenuItemDefinition>().Where(x => !x.HasParent);

            CustomizableMenuBars = new ObservableCollection<MenuItemDefinition>(topLevelMenuItems);
        }


        public ObservableCollectionEx<ToolbarDefinition> CustomizableToolBars { get; set; }
        public ObservableCollection<MenuItemDefinition> CustomizableMenuBars { get; set; }
    }
}
