using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface IWaitDialogFactory
    {
        void CreateInstance(out IWaitDialog waitDialog);
    }
}