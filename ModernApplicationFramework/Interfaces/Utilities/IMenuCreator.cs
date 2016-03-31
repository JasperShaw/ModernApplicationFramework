using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IMenuCreator
    {
        /// <summary>
        /// Populate a menu with a MenuItemDefinitionsPopulator
        /// </summary>
        /// <param name="model"></param>
        /// <param name="definitions"></param>
        void CreateMenu(IMenuHostViewModel model, MenuItemDefinitionsPopulatorBase definitions);


        /// <summary>
        /// Populate a menu freely
        /// </summary>
        /// <param name="model"></param>
        void CreateMenu(IMenuHostViewModel model);
    }
}