using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IMenuCreator
    {
        /// <summary>
        ///     Populate a menu freely
        /// </summary>
        /// <param name="model"></param>
        void CreateMenuBar(IMenuHostViewModel model);

        void CreateMenuTree(CommandBarDefinitionBase definition, MenuItem menuItem);
    }
}