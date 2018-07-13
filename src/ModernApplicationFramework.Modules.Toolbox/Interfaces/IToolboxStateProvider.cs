using System.Collections.Generic;
using Caliburn.Micro;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxStateProvider
    {
        IReadOnlyCollection<IToolboxCategory> State { get; }
    }

    internal interface IInternalToolboxStateProvider : IToolboxStateProvider
    {
        IObservableCollection<IToolboxCategory> ItemsSource { get; }
    }

}