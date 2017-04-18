using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar.Hosts
{
    [Export(typeof(IContextMenuHost))]
    public sealed class ContextMenuHost : CommandBarHost, IContextMenuHost
    {
        private readonly Dictionary<Definitions.ContextMenu.ContextMenuDefinition, ContextMenu> _hostedContextMenus;

        public override ObservableCollection<CommandBarDefinitionBase> TopLevelDefinitions { get; }

        [ImportingConstructor]
        public ContextMenuHost([ImportMany] Definitions.ContextMenu.ContextMenuDefinition[] contextMenuDefinitions)
        {
            _hostedContextMenus = new Dictionary<Definitions.ContextMenu.ContextMenuDefinition, ContextMenu>();
            TopLevelDefinitions = new ObservableCollection<CommandBarDefinitionBase>(contextMenuDefinitions);
            Build();
        }

        public override void Build()
        {
            _hostedContextMenus.Clear();
            var contextMenus = TopLevelDefinitions.Where(x => !DefinitionHost.ExcludedItemDefinitions.Contains(x))
                .Cast<Definitions.ContextMenu.ContextMenuDefinition>();

            foreach (var definition in contextMenus)
                Build(definition);
        }

        public override void Build(CommandBarDefinitionBase definition)
        {
            if (!(definition is Definitions.ContextMenu.ContextMenuDefinition contextMenuDefinition))
                return;
            BuildLogical(contextMenuDefinition);
            var contextMenu = IoC.Get<IContextMenuCreator>().CreateContextMenu(definition);
            if (!_hostedContextMenus.ContainsKey(contextMenuDefinition))
                _hostedContextMenus.Add(contextMenuDefinition, contextMenu);
            else
                _hostedContextMenus[contextMenuDefinition] = contextMenu;
        }

        public ContextMenu GetContextMenu(Definitions.ContextMenu.ContextMenuDefinition contextMenuDefinition)
        {
            if (!_hostedContextMenus.TryGetValue(contextMenuDefinition, out ContextMenu contextMenu))
                throw new ArgumentException(contextMenuDefinition.Text);
            return contextMenu;
        }
    }
}