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

        /// <summary>
        ///    Gets a <see cref="CommandDefinitionBase"/> by the specified format
        /// </summary>
        /// <param name="pattern">
        ///   The format specifies the search pattern for a <see cref="CommandDefinitionBase"/>
        ///    <paramref name="pattern" />-parameter can be "cl", "cu", "t", "nu" oder "nl".
        /// </param>
         /// <param name="input">
        ///   The input data to search a <see cref="CommandDefinitionBase"/>
        /// </param>
        /// <returns>
        ///   Returns the <see cref="CommandDefinitionBase"/>
        /// </returns>
        /// <exception cref="T:System.FormatException">
        ///   The value of  <paramref name="pattern" /> is not <see langword="null" />, an empty string (""), "cl", "cu", "t", "nu", or "nl".
        /// </exception>
        CommandDefinitionBase GetCommandDefinitionBy(string pattern, string input);

        CommandDefinitionBase GetCommandDefinitionById(Guid id);
    }
}