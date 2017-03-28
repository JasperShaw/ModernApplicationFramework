using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface IToolBarHostViewModel
    {
        IMainWindowViewModel MainWindowViewModel { get; set; }

        CommandBase.Command OpenContextMenuCommand { get; }

        ContextMenu ContextMenu { get; }


        void AddToolbarDefinition(ToolbarDefinition definitionOld);
        void RemoveToolbarDefinition(ToolbarDefinition definitionOld);

        ObservableCollectionEx<ToolbarDefinition> ToolbarDefinitions { get; }

        void SetupToolbars();

        ToolBarTray TopToolBarTray { get; set; }
        ToolBarTray LeftToolBarTray { get; set; }
        ToolBarTray RightToolBarTray { get; set; }
        ToolBarTray BottomToolBarTray { get; set; }
        string GetUniqueToolBarName();
    }
}