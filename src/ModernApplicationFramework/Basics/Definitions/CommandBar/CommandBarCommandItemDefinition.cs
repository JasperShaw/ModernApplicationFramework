using System;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// Simple command bar item
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition" />
    public sealed class CommandBarCommandItemDefinition : CommandBarItemDefinition
    {
        public override Guid Id { get; }

        public CommandBarCommandItemDefinition(Guid id, uint sortOrder, CommandDefinitionBase commandDefinition, bool isCustom = false,
            bool isCustomizable = true)
            : base(null, sortOrder, null, commandDefinition, true, false, isCustom, isCustomizable)
        {
            Id = id;
            Text = CommandDefinition?.Text;
            Name = CommandDefinition?.Name;
        }
    }
}