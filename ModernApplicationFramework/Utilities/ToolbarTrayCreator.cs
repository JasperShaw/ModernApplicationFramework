using System.Linq;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Utilities
{
    public class ToolbarTrayCreator : IToolbarTrayCreator
    {
        public void CreateToolbarTray(IToolBarHostViewModel model, ToolbarDefinitionsPopulatorBase definitions)
        {
            var toolbarDefinitions = definitions.GetDefinitions().OrderBy(x => x.SortOrder);
            foreach (var definition in toolbarDefinitions)
                model.AddToolBar(definition.ToolBar, definition.VisibleOnLoad, definition.Position);
        }

        public virtual void CreateToolbarTray(IToolBarHostViewModel model)
        {
        }
    }
}