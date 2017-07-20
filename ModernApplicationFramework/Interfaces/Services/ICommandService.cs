using System;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Interfaces.Services
{
    /// <summary>
    /// A service to get a <see cref="DefinitionBase"/>
    /// </summary>
    public interface ICommandService
    {
        /// <summary>
        /// Gets a <see cref="DefinitionBase"/> by type
        /// </summary>
        /// <param name="definitionType">The type of the <see cref="DefinitionBase"/> that is requested</param>
        /// <returns>Returns the <see cref="DefinitionBase"/></returns>
        /// <exception cref="T:System.InvalidOperationException">
        ///  No <see cref="DefinitionBase"/> was found
        /// </exception>
        DefinitionBase GetCommandDefinition(Type definitionType);
    }
}