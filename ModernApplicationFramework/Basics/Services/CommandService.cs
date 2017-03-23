using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Interfaces.Command;

namespace ModernApplicationFramework.Basics.Services
{
    [Export(typeof(ICommandService))]
    public class CommandService : ICommandService
    {
        private readonly Dictionary<Type, CommandDefinition> _commandDefinitionsLookup;

#pragma warning disable 649
        [ImportMany] private CommandDefinition[] _commandDefinitions;
#pragma warning restore 649

        public CommandService()
        {
            _commandDefinitionsLookup = new Dictionary<Type, CommandDefinition>();
        }

        public CommandDefinition GetCommandDefinition(Type commandDefinitionType)
        {
            CommandDefinition commandDefinition;
            if (!_commandDefinitionsLookup.TryGetValue(commandDefinitionType, out commandDefinition))
                commandDefinition = _commandDefinitionsLookup[commandDefinitionType] =
                    _commandDefinitions.First(x => x.GetType() == commandDefinitionType);
            return commandDefinition;
        }
    }
}