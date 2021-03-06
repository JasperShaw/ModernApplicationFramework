﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Services.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// A service to get a <see cref="CommandBarItemDefinition" /> by its actual type
    /// </summary>
    /// <seealso cref="ICommandBarItemService" />
    [Export(typeof(ICommandBarItemService))]
    public class CommandBarItemService : ICommandBarItemService
    {
        private readonly Dictionary<Type, CommandBarItemDefinition> _commandDefinitionsLookup;

#pragma warning disable 649
        [ImportMany] private List<CommandBarItemDefinition> _commandDefinitions;
#pragma warning restore 649

        public CommandBarItemService()
        {
            _commandDefinitionsLookup = new Dictionary<Type, CommandBarItemDefinition>();
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the command definition.
        /// </summary>
        /// <param name="commandDefinitionType">Type of the command definition.</param>
        /// <returns></returns>
        public CommandBarItemDefinition GetItemDefinition(Type commandDefinitionType)
        {
            if (!_commandDefinitionsLookup.TryGetValue(commandDefinitionType, out var commandDefinition))
                commandDefinition = _commandDefinitionsLookup[commandDefinitionType] =
                    _commandDefinitions.First(x => x.GetType() == commandDefinitionType);
            return commandDefinition;
        }

        public CommandBarItemDefinition GetItemDefinition(string pattern, string input)
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

        public CommandBarItemDefinition GetItemDefinitionById(Guid id)
        {
            return GetFirst(x => x.Id == id);
        }


        private CommandBarItemDefinition GetFirst(Func<CommandBarItemDefinition, bool> predicate)
        {
            return _commandDefinitions.FirstOrDefault(predicate);
        }
    }
}