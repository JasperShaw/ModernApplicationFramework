using System;
using ModernApplicationFramework.Extended.KeyBindingScheme;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface IKeyBindingManager
    {
        /// <summary>
        /// Gets the scheme manager.
        /// </summary>
        IKeyBindingSchemeManager SchemeManager { get; }

        /// <summary>
        /// Gets the key gesture service.
        /// </summary>
        IKeyGestureService KeyGestureService { get; }

        /// <summary>
        ///     Builds the current key layout.
        /// <exception cref="NotSupportedException">
        ///     The current application has settings disabled.
        /// </exception>
        /// </summary>
        void BuildCurrent();

        /// <summary>
        ///     Saves the current key layout in memory.
        /// <exception cref="NotSupportedException">
        ///     The current application has settings disabled.
        /// </exception>
        /// </summary>
        void SaveCurrent();

        /// <summary>
        ///     Applies the key bindings from settings.
        /// <exception cref="NotSupportedException">
        ///     The current application has settings disabled.
        /// </exception>
        /// </summary>
        void ApplyKeyBindingsFromSettings();

        /// <summary>
        /// Removes all key bindings and applies scheme.
        /// </summary>
        /// <param name="selectedScheme">The selected scheme.</param>
        void ResetToKeyScheme(SchemeDefinition selectedScheme);

        /// <summary>
        /// Switches key binding scheme and applies additional changes from settings if possible
        /// </summary>
        /// <param name="selectedScheme">The selected scheme.</param>
        void SetKeyScheme(SchemeDefinition selectedScheme);

        /// <summary>
        /// Loads and resets to the default scheme.
        /// </summary>
        void LoadDefaultScheme();
    }
}