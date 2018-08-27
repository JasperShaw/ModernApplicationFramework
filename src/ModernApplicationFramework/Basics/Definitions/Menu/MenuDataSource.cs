﻿using System;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    /// <inheritdoc />
    /// <summary>
    /// A command bar definition that specifies a menu item
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition" />
    public class MenuDataSource : CommandBarItemDataSource
    {
        public override Guid Id { get; }

        public MenuDataSource(Guid id, CommandBarGroup group, uint sortOrder, string text, bool isCustom = false)
            : base(text, sortOrder, group, new MenuHeaderCommandDefinition(), isCustom, false)
        {
            Id = id;
        }

        public override CommandControlTypes UiType => CommandControlTypes.Menu;

        public ObservableCollection<CommandBarItemDataSource> Items { get; set; }

        private sealed class MenuHeaderCommandDefinition : CommandDefinitionBase
        {
            public override string Name => null;
            public override string NameUnlocalized => null;
            public override string Text => null;
            public override string ToolTip => null;
            public override bool IsList => false;
            public override CommandCategory Category => null;
            public override CommandControlTypes ControlType => CommandControlTypes.Menu;
            public override Guid Id => new Guid("{BED985A2-2FE6-4FA1-AD74-731EFEBFF786}");
        }
    }
}