using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar.Hosts
{
    [Export(typeof(IContextMenuHost))]
    public sealed class ContextMenuHost : CommandBarHost, IContextMenuHost
    {
        private readonly Dictionary<Definitions.ContextMenu.ContextMenuDefinition, ContextMenu> _hostedContextMenus;
        public ObservableCollectionEx<Definitions.ContextMenu.ContextMenuDefinition> ContextMenuDefinitions { get; }

        [ImportingConstructor]
        public ContextMenuHost([ImportMany] Definitions.ContextMenu.ContextMenuDefinition[] contextMenuDefinitions)
        {
            _hostedContextMenus = new Dictionary<Definitions.ContextMenu.ContextMenuDefinition, ContextMenu>();

            ContextMenuDefinitions = new ObservableCollectionEx<Definitions.ContextMenu.ContextMenuDefinition>();
            foreach (var menuDefinition in contextMenuDefinitions)
                ContextMenuDefinitions.Add(menuDefinition);

            Build();
        }

        public override void Build()
        {
            _hostedContextMenus.Clear();
            foreach (var definition in ContextMenuDefinitions.Where(x => !DefinitionHost.ExcludedItemDefinitions
                .Contains(x)))
            {
                var contextMenu = IoC.Get<IContextMenuCreator>().CreateContextMenu(definition);
                _hostedContextMenus.Add(definition, contextMenu);
            }
        }

        public ContextMenu GetContextMenu(Definitions.ContextMenu.ContextMenuDefinition contextMenuDefinition)
        {
            if (!_hostedContextMenus.TryGetValue(contextMenuDefinition, out ContextMenu contextMenu))
                throw new ArgumentException(contextMenuDefinition.Text);
            return contextMenu;
        }
    }
}