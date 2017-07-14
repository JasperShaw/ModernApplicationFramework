using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Menu;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface IToolBarHostViewModel : ICommandBarHost, IHasMainWindowViewModel
    {
        ICommand OpenContextMenuCommand { get; }

        ContextMenu ContextMenu { get; }

        ToolBarTray TopToolBarTray { get; set; }
        ToolBarTray LeftToolBarTray { get; set; }
        ToolBarTray RightToolBarTray { get; set; }
        ToolBarTray BottomToolBarTray { get; set; }


        void AddToolbarDefinition(ToolbarDefinition definitionOld);
        void RemoveToolbarDefinition(ToolbarDefinition definitionOld);
        string GetUniqueToolBarName();
    }
}