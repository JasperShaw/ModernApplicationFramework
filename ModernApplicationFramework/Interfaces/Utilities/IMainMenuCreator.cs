using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Menu;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IMainMenuCreator : IMenuCreator
    {
        void CreateMenuBar(IMenuHostViewModel model);

        MenuItem CreateMenuItem(CommandBarDefinitionBase contextMenuDefinition);
    }
}