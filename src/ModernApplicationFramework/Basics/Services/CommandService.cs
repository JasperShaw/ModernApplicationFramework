using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Services
{
    /// <inheritdoc />
    /// <summary>
    /// A service to get a <see cref="CommandDefinitionBase" /> by its actual type
    /// </summary>
    /// <seealso cref="ICommandService" />
    [Export(typeof(ICommandService))]
    public class CommandService : ICommandService
    {
        private readonly Dictionary<Type, CommandDefinitionBase> _commandDefinitionsLookup;

#pragma warning disable 649
        [ImportMany] private CommandDefinitionBase[] _commandDefinitions;
#pragma warning restore 649

        public CommandService()
        {
            _commandDefinitionsLookup = new Dictionary<Type, CommandDefinitionBase>();
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the command definition.
        /// </summary>
        /// <param name="commandDefinitionType">Type of the command definition.</param>
        /// <returns></returns>
        public CommandDefinitionBase GetCommandDefinition(Type commandDefinitionType)
        {
            if (!_commandDefinitionsLookup.TryGetValue(commandDefinitionType, out CommandDefinitionBase commandDefinition))
                commandDefinition = _commandDefinitionsLookup[commandDefinitionType] =
                    _commandDefinitions.First(x => x.GetType() == commandDefinitionType);
            return commandDefinition;
        }

        public CommandDefinitionBase GetCommandDefinitionBy(string pattern, string input)
        {
            switch (pattern.ToLowerInvariant())
            {
                case "t":
                    return GetFirst(x => x.GetType().Name.Equals(input));
                case "nu":
                    return GetFirst(x => x.NameUnlocalized.Equals(input));
                case "nl":
                    return GetFirst(x => x.Name.Equals(input));
                case "cl":
                    return GetFirst(x => x.TrimmedCategoryCommandName.Equals(input));
                case "cu":
                    return GetFirst(x => x.TrimmedCategoryCommandNameUnlocalized.Equals(input));
                default:
                    throw new FormatException();
            }
        }

        private CommandDefinitionBase GetFirst(Func<CommandDefinitionBase, bool> predicate)
        {
            return _commandDefinitions.FirstOrDefault(predicate);
        }
    }
}