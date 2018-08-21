using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls.Menu;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar.Hosts
{
    /// <inheritdoc cref="IContextMenuHost" />
    /// <summary>
    /// Implementation of <see cref="IContextMenuHost"/>
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.CommandBar.Hosts.CommandBarHost" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.IContextMenuHost" />
    [Export(typeof(IContextMenuHost))]
    public sealed class ContextMenuHost : CommandBarHost, IContextMenuHost
    {
        private readonly Dictionary<Definitions.ContextMenu.ContextMenuDataSource, ContextMenu> _hostedContextMenus;

        public override ObservableCollection<CommandBarDataSource> TopLevelDefinitions { get; }

        [ImportingConstructor]
        public ContextMenuHost([ImportMany] Definitions.ContextMenu.ContextMenuDataSource[] contextMenuDataSources)
        {
            _hostedContextMenus = new Dictionary<Definitions.ContextMenu.ContextMenuDataSource, ContextMenu>();
            TopLevelDefinitions = new ObservableCollection<CommandBarDataSource>(contextMenuDataSources);
            Build();
        }

        public override void Build()
        {
            _hostedContextMenus.Clear();
            var contextMenus = TopLevelDefinitions.Where(x => !DefinitionHost.ExcludedItemDefinitions.Contains(x))
                .Cast<Definitions.ContextMenu.ContextMenuDataSource>();

            foreach (var definition in contextMenus)
                Build(definition);
        }

        public override void Build(CommandBarDataSource definition)
        {
            if (!(definition is Definitions.ContextMenu.ContextMenuDataSource contextMenuDefinition))
                return;
            BuildLogical(contextMenuDefinition);

            var groups = DefinitionHost.GetSortedGroupsOfDefinition(definition);
            var contextMenu = IoC.Get<IContextMenuCreator>().CreateContextMenu(definition, groups, DefinitionHost.GetItemsOfGroup);
            if (!_hostedContextMenus.ContainsKey(contextMenuDefinition))
                _hostedContextMenus.Add(contextMenuDefinition, contextMenu);
            else
                _hostedContextMenus[contextMenuDefinition] = contextMenu;
        }

        public ContextMenu GetContextMenu(Definitions.ContextMenu.ContextMenuDataSource contextMenuDataSource)
        {
            if (!_hostedContextMenus.ContainsKey(contextMenuDataSource))
                throw new KeyNotFoundException(contextMenuDataSource.Text);
            Build(contextMenuDataSource);
            return _hostedContextMenus[contextMenuDataSource];
        }

        public ContextMenu GetContextMenu(Guid contextMenuDefinition)
        {
            var definition = _hostedContextMenus.Keys.FirstOrDefault(x => x.Id == contextMenuDefinition);
            if (definition == null)
                throw new KeyNotFoundException();
            return GetContextMenu(definition);
        }
    }
}