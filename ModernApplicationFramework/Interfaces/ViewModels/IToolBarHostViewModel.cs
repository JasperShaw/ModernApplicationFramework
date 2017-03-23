using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface IToolBarHostViewModel : IHasTheme
    {
        IMainWindowViewModel MainWindowViewModel { get; set; }

        CommandBase.Command OpenContextMenuCommand { get; }

        ContextMenu ContextMenu { get; }


        void AddToolbarDefinition(ToolbarDefinition definition);
        void RemoveToolbarDefinition(ToolbarDefinition definition);

        ObservableCollectionEx<ToolbarDefinition> ToolbarDefinitions { get; }

        void SetupToolbars();

        ToolBarTray TopToolBarTray { get; set; }
        ToolBarTray LeftToolBarTray { get; set; }
        ToolBarTray RightToolBarTray { get; set; }
        ToolBarTray BottomToolBarTray { get; set; }
        string GetUniqueToolBarName();
    }
}