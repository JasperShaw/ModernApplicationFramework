using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Services
{
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

        public CommandDefinitionBase GetCommandDefinition(Type commandDefinitionType)
        {
            if (!_commandDefinitionsLookup.TryGetValue(commandDefinitionType, out CommandDefinitionBase commandDefinition))
                commandDefinition = _commandDefinitionsLookup[commandDefinitionType] =
                    _commandDefinitions.First(x => x.GetType() == commandDefinitionType);
            return commandDefinition;
        }
    }
}