using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IToolbarCreator : IMenuCreator
    {
        /// <summary>
        ///     Populate a toolbartray with a ToolbarDefinitionsPopulator
        /// </summary>
        ToolBar CreateToolbar(ToolbarDefinition definition);
    }
}