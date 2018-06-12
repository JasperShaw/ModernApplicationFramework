using Caliburn.Micro;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxStateProvider
    {
        IObservableCollection<IToolboxCategory> ItemsSource { get; }
    }
}