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
        //public ObservableCollection<Definitions.ContextMenu.ContextMenuDefinition> ContextMenuDefinitions { get; }

        public override ICollection<CommandBarDefinitionBase> TopLevelDefinitions { get; }

        [ImportingConstructor]
        public ContextMenuHost([ImportMany] Definitions.ContextMenu.ContextMenuDefinition[] contextMenuDefinitions)
        {
            _hostedContextMenus = new Dictionary<Definitions.ContextMenu.ContextMenuDefinition, ContextMenu>();

            //ContextMenuDefinitions = new ObservableCollection<Definitions.ContextMenu.ContextMenuDefinition>(contextMenuDefinitions);

            TopLevelDefinitions = new ObservableCollection<CommandBarDefinitionBase>(contextMenuDefinitions);

            Build();
        }

        public override void Build()
        {
            _hostedContextMenus.Clear();
            foreach (var definition in TopLevelDefinitions.Where(x => !DefinitionHost.ExcludedItemDefinitions
                .Contains(x))
                .Cast<Definitions.ContextMenu.ContextMenuDefinition>())
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