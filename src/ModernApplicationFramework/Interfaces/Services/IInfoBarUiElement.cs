using System.Windows;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface IInfoBarUiElement
    {
        FrameworkElement CreateControl();

        int Close();

        int Advise(IInfoBarUiEvents eventSink, out uint cookie);

        int Unadvise(uint cookie);
    }
}