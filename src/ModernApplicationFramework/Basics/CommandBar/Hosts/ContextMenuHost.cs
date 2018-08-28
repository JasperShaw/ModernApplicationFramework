using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
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
        private readonly Dictionary<ContextMenuDataSource, ContextMenu> _hostedContextMenus;

        public override ObservableCollection<CommandBarDataSource> TopLevelDefinitions { get; }

        [ImportingConstructor]
        public ContextMenuHost([Import] ICommandBarDefinitionHost host)
        {
            _hostedContextMenus = new Dictionary<ContextMenuDataSource, ContextMenu>();
            TopLevelDefinitions = new ObservableCollection<CommandBarDataSource>();


            foreach (var contextMenu in host.ItemDefinitions.Where(x => x.UiType == CommandControlTypes.ContextMenu))
                TopLevelDefinitions.Add(contextMenu);


            Build();
        }

        public override void Build()
        {
            _hostedContextMenus.Clear();
            var contextMenus = TopLevelDefinitions.Where(x => !DefinitionHost.ExcludedItemDefinitions.Contains(x))
                .Cast<ContextMenuDataSource>();

            foreach (var definition in contextMenus)
                Build(definition);
        }

        public override void Build(CommandBarDataSource definition)
        {
            if (!(definition is ContextMenuDataSource contextMenuDefinition))
                return;
            BuildLogical(contextMenuDefinition);

            var groups = DefinitionHost.GetSortedGroupsOfDefinition(definition);
            var contextMenu = IoC.Get<IContextMenuCreator>().CreateContextMenu(definition, groups, DefinitionHost.GetItemsOfGroup);
            if (!_hostedContextMenus.ContainsKey(contextMenuDefinition))
                _hostedContextMenus.Add(contextMenuDefinition, contextMenu);
            else
                _hostedContextMenus[contextMenuDefinition] = contextMenu;
        }

        public ContextMenu GetContextMenu(CommandBarItem commandBarItem)
        {
            if (!(commandBarItem.ItemDataSource is ContextMenuDataSource contextMenuDataSource))
                return null;
            return GetContextMenu(contextMenuDataSource);
        }

        private ContextMenu GetContextMenu(ContextMenuDataSource dataSource)
        {
            if (!_hostedContextMenus.ContainsKey(dataSource))
                throw new KeyNotFoundException(dataSource.Text);
            Build(dataSource);
            return _hostedContextMenus[dataSource];
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