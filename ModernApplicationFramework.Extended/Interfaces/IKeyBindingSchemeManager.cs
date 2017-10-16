using System.Collections.Generic;
using ModernApplicationFramework.Extended.KeyBindingScheme;

namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface IKeyBindingSchemeManager
    {
        /// <summary>
        /// The loaded scheme definitions.
        /// </summary>
        ICollection<SchemeDefinition> SchemeDefinitions { get; }


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

        /// <summary>
        /// Clears all current bindings and applies this specified scheme
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        void ResetToScheme(SchemeDefinition scheme);

        /// <summary>
        /// Selects a default scheme and applies it
        /// </summary>
        void SetDefaultScheme();
    }
}