using Caliburn.Micro;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    public interface ICustomizeDialogScreen : IScreen
    {
        uint SortOrder { get; }
    }
}