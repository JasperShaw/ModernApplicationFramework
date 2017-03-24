using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Command;

namespace ModernApplicationFramework.Basics.Services
{
    [Export(typeof(ICommandService))]
    public class CommandService : ICommandService
    {
        private readonly Dictionary<Type, DefinitionBase> _commandDefinitionsLookup;

#pragma warning disable 649
        [ImportMany] private DefinitionBase[] _commandDefinitions;
#pragma warning restore 649

        public CommandService()
        {
            _commandDefinitionsLookup = new Dictionary<Type, DefinitionBase>();
        }

        public DefinitionBase GetCommandDefinition(Type commandDefinitionType)
        {
            DefinitionBase commandDefinition;
            if (!_commandDefinitionsLookup.TryGetValue(commandDefinitionType, out commandDefinition))
                commandDefinition = _commandDefinitionsLookup[commandDefinitionType] =
                    _commandDefinitions.First(x => x.GetType() == commandDefinitionType);
            return commandDefinition;
        }
    }
}