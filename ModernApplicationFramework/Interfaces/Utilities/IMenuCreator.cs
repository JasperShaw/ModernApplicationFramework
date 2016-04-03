using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IMenuCreator
    {
        /// <summary>
        /// Populate a menu freely
        /// </summary>
        /// <param name="model"></param>
        void CreateMenu(IMenuHostViewModel model);
    }
}