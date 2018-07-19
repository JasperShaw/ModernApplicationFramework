using Caliburn.Micro;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    internal interface IInternalToolboxStateProvider : IToolboxStateProvider
    {
        IObservableCollection<IToolboxCategory> ItemsSource { get; }
    }
}