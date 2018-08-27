using System;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// A command bar definition that specifies a menu bar
    /// </summary>
    internal sealed class MenuBarDataSource : CommandBarDataSource
    {
        public override CommandControlTypes UiType => CommandControlTypes.MenuBar;

        public override bool InheritInternalName => false;

        public override Guid Id { get; }

        public MenuBarDataSource(Guid id, string text) : base(text, 0, false)
        {
            Id = id;
        }
    }
}