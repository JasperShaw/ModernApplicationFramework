using System;

namespace ModernApplicationFramework.Basics.CommandBar.DataSources
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

        public MenuBarDataSource(Guid id, string text) : base(text, false)
        {
            Id = id;
        }
    }
}