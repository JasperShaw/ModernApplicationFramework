using System.Collections.Generic;
using System.Windows;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Interfaces.Services
{
    /// <summary>
    /// A <see cref="IKeyGestureService"/> administrates Keybindings to a <see cref="UIElement"/>
    /// </summary>
    public interface IKeyGestureService
    {

        bool IsInitialized { get; }


        /// <summary>
        /// Initializes the service.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Registers an <see cref="ICanHaveInputBindings"/> to the service
        /// </summary>
        /// <param name="hostingModel">The hosting model.</param>
        void Register(ICanHaveInputBindings hostingModel);


        /// <summary>
        /// Removes an <see cref="ICanHaveInputBindings"/> from the service
        /// </summary>
        /// <param name="hostingModel">The hosting model.</param>
        void Remove(ICanHaveInputBindings hostingModel);


        /// <summary>
        /// Sets key gestures to all registered elements.
        /// </summary>
        void SetKeyGestures();

        /// <summary>
        /// Removes all key gestures from all registered elements.
        /// </summary>
        void RemoveKeyGestures();

        /// <summary>
        /// Loads all available key gestures and applies them to their <see cref="CommandDefinition"/>
        /// </summary>
        void LoadGestures();

        /// <summary>
        /// Loads all default key gestures and applies them to their <see cref="CommandDefinition"/>
        /// </summary>
        void LoadDefaultGestures();

        IEnumerable<CommandCategoryGestureMapping> GetAllBindings();

        IEnumerable<CommandDefinition> GetAllCommandCommandDefinitions();
    }
}