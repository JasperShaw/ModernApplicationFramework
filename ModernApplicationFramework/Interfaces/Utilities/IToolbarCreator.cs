using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IToolbarTrayCreator
    {
        /// <summary>
        /// Populate a toolbartray with a ToolbarDefinitionsPopulator
        /// </summary>
        /// <param name="model"></param>
        /// <param name="definitions"></param>
        void CreateToolbarTray(IToolBarHostViewModel model, ToolbarDefinitionsPopulatorBase definitions);


        /// <summary>
        /// Populate a toolbartray freely
        /// </summary>
        /// <param name="model"></param>
        void CreateToolbarTray(IToolBarHostViewModel model);
    }
}
