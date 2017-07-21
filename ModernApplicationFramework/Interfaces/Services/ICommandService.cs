using System;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Interfaces.Services
{
    /// <summary>
    /// A service to get a <see cref="CommandDefinitionBase"/>
    /// </summary>
    public interface ICommandService
    {
        /// <summary>
        /// Gets a <see cref="CommandDefinitionBase"/> by type
        /// </summary>
        /// <param name="definitionType">The type of the <see cref="CommandDefinitionBase"/> that is requested</param>
        /// <returns>Returns the <see cref="CommandDefinitionBase"/></returns>
        /// <exception cref="T:System.InvalidOperationException">
        ///  No <see cref="CommandDefinitionBase"/> was found
        /// </exception>
        CommandDefinitionBase GetCommandDefinition(Type definitionType);
    }
}