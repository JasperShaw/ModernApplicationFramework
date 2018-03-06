using System.Collections.Generic;
using ModernApplicationFramework.Extended.Input.KeyBindingScheme;

namespace ModernApplicationFramework.Extended.Interfaces
{
    /// <summary>
    /// An <see cref="IKeyBindingSchemeManager"/> manages the Keyboard schemes for the application
    /// </summary>
    public interface IKeyBindingSchemeManager
    {
        /// <summary>
        /// The loaded scheme definitions.
        /// </summary>
        ICollection<SchemeDefinition> SchemeDefinitions { get; }

        /// <summary>
        /// The current scheme.
        /// </summary>
        SchemeDefinition CurrentScheme { get; }

        /// <summary>
        /// Loads all available scheme definitions into working memory
        /// </summary>
        void LoadSchemeDefinitions();

        /// <summary>
        /// Loads a scheme into current key bindings
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        void SetScheme(SchemeDefinition scheme);
    }
}