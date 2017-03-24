using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface IToolBarHostViewModel : IHasTheme
    {
        IMainWindowViewModel MainWindowViewModel { get; set; }

        CommandBase.Command OpenContextMenuCommand { get; }

        ContextMenu ContextMenu { get; }


        void AddToolbarDefinition(ToolbarDefinitionOld definitionOld);
        void RemoveToolbarDefinition(ToolbarDefinitionOld definitionOld);

        ObservableCollectionEx<ToolbarDefinitionOld> ToolbarDefinitions { get; }

        void SetupToolbars();

        ToolBarTray TopToolBarTray { get; set; }
        ToolBarTray LeftToolBarTray { get; set; }
        ToolBarTray RightToolBarTray { get; set; }
        ToolBarTray BottomToolBarTray { get; set; }
        string GetUniqueToolBarName();
    }
}