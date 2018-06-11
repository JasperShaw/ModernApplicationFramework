using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.State
{
    [Export(typeof(IToolboxStateProvider))]
    internal class ToolboxStateProvider : IToolboxStateProvider
    {
        [ImportingConstructor]
        public ToolboxStateProvider(ToolboxItemsBuilder builder, ToolboxItemHost host)
        {
            builder.Initialize();
            ItemsSource = new BindableCollection<IToolboxCategory>(host.AllCategories);
        }

        public IObservableCollection<IToolboxCategory> ItemsSource { get; }
    }

    public interface IToolboxStateProvider
    {
        IObservableCollection<IToolboxCategory> ItemsSource { get; }
    }
}
