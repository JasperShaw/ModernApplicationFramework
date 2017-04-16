using System.Collections.ObjectModel;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    internal interface IToolBarsPageViewModel : IScreen
    {
        ObservableCollection<CommandBarDefinitionBase> Toolbars { get; }

        ToolbarDefinition SelectedToolbarDefinition { get; }

        CommandBase.Command DropDownClickCommand { get; }

        CommandBase.Command DeleteSelectedToolbarCommand { get; }

        CommandBase.Command CreateNewToolbarCommand { get; }
    }
}