using System;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// Simple command bar item definition that contains a <see cref="CommandBarDataSource" />
    /// </summary>
    /// <typeparam name="T">The type of the command definition this item should have</typeparam>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition`1" />
    public sealed class CommandBarCommandItemDataSource<T> : CommandBarItemDataSource<T> where T : CommandDefinitionBase
	{
	    public override Guid Id { get; }

        public CommandBarCommandItemDataSource(Guid id, CommandBarGroup group, uint sortOrder,
            bool isCustom = false, bool isChecked = false,
            bool bindItemVisibilityToCommandExecution = false, CommandBarFlags flags = CommandBarFlags.CommandFlagNone)
            : base(null, sortOrder, group, isCustom,  isChecked, flags)
        {
            Id = id;
        }
	}
}