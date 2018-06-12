﻿using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.State
{
    [Export(typeof(IToolboxStateProvider))]
    internal class ToolboxStateProvider : IToolboxStateProvider
    {
        public IObservableCollection<IToolboxCategory> ItemsSource { get; }

        [ImportingConstructor]
        public ToolboxStateProvider(ToolboxItemHost host)
        {
            host.Initialize();
            ItemsSource = new BindableCollection<IToolboxCategory>(host.AllCategories);
        }
    }
}
