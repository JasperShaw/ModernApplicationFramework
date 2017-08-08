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

        public CommandDefinitionBase GetCommandDefinition(string typeName)
        {
            return _commandDefinitions.FirstOrDefault(x => x.GetType().Name.Equals(typeName));
        }
    }
}