using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    internal interface IToolBarsPageViewModel : IScreen
    {
        ObservableCollectionEx<ToolbarDefinitionOld> Toolbars { get; }

        ToolbarDefinitionOld SelectedToolbarDefinitionOld { get; }

        CommandBase.Command DropDownClickCommand { get; }

        CommandBase.Command DeleteSelectedToolbarCommand { get; }

        CommandBase.Command CreateNewToolbarCommand { get; }
    }
}