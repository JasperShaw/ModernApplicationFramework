using System;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;

namespace ModernApplicationFramework.Interfaces.Services
{
    /// <summary>
    /// A service to get a <see cref="CommandBarItemDefinition"/>
    /// </summary>
    public interface ICommandBarItemService
    {
        /// <summary>
        /// Gets a <see cref="CommandBarItemDefinition"/> by type
        /// </summary>
        /// <param name="definitionType">The type of the <see cref="CommandBarItemDefinition"/> that is requested</param>
        /// <returns>Returns the <see cref="CommandBarItemDefinition"/></returns>
        /// <exception cref="T:System.InvalidOperationException">
        ///  No <see cref="CommandBarItemDefinition"/> was found
        /// </exception>
        CommandBarItemDefinition GetItemDefinition(Type definitionType);

        /// <summary>
        ///    Gets a <see cref="CommandBarItemDefinition"/> by the specified format
        /// </summary>
        /// <param name="pattern">
        ///   The format specifies the search pattern for a <see cref="CommandBarItemDefinition"/>
        ///    <paramref name="pattern" />-parameter can be "cl", "cu", "t", "nu" oder "nl".
        /// </param>
         /// <param name="input">
        ///   The input data to search a <see cref="CommandBarItemDefinition"/>
        /// </param>
        /// <returns>
        ///   Returns the <see cref="CommandBarItemDefinition"/>
        /// </returns>
        /// <exception cref="T:System.FormatException">
        ///   The value of  <paramref name="pattern" /> is not <see langword="null" />, an empty string (""), "cl", "cu", "t", "nu", or "nl".
        /// </exception>
        CommandBarItemDefinition GetItemDefinition(string pattern, string input);

        CommandBarItemDefinition GetItemDefinitionById(Guid id);
    }
}