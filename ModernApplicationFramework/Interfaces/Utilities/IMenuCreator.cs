using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IMenuCreator
    {
        void CreateMenu(IMenuHostViewModel model);
    }
}