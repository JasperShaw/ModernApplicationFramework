using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    internal interface IToolBarsPageViewModel : IScreen
    {
        ObservableCollectionEx<ToolbarDefinition> Toolbars { get; }

        ToolbarDefinition SelectedToolbarDefinition { get; }

        Command DropDownClickCommand { get; }

        Command DeleteSelectedToolbarCommand { get; }

        Command CreateNewToolbarCommand { get; }
    }
}