using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Interfaces.Services
{
    /// <summary>
    /// A <see cref="IKeyGestureService"/> administrates Keybindings to a <see cref="UIElement"/>
    /// </summary>
    public interface IKeyGestureService
    {
        /// <summary>
        /// Indicating whether this instance is initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        ///  Option to switch the Visual Studio like behavior for multi-key gestures on or off.
        ///  If this option is enabled all keyboard input events will be filtered separately for multi-key gestures.
        ///  This options has the advantage that a multi-key gestures does not get interrupted by an common gesture like 'CTRL+A'
        ///  Default is <see langword="true"/>
        /// </summary>

        bool IsEnhancedMultiKeyGestureModeEnabled { get; set; }


        /// <summary>
        /// Initializes the service.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Adds an <see cref="ICanHaveInputBindings"/> to the service
        /// </summary>
        /// <param name="hostingModel">The hosting model.</param>
        void AddModel(ICanHaveInputBindings hostingModel);

        /// <summary>
        /// Removes an <see cref="ICanHaveInputBindings"/> from the service
        /// </summary>
        /// <param name="hostingModel">The hosting model.</param>
        void RemoveModel(ICanHaveInputBindings hostingModel);


        void AddKeyGestures(ICommand command, CategoryGestureMapping categoryKeyGesture);

        void RemoveKeyGesture(CategoryGestureMapping categoryKeyGesture);


        /// <summary>
        /// Removes all key gestures from all registered elements.
        /// </summary>
        void RemoveAllKeyGestures();

        /// <summary>
        /// Loads all available key gestures and applies them to their <see cref="CommandDefinition"/>
        /// </summary>
        void LoadGestures();

        /// <summary>
        /// Loads all default key gestures and applies them to their <see cref="CommandDefinition"/>
        /// </summary>
        void LoadDefaultGestures();

        IEnumerable<CommandCategoryGestureMapping> GetAllBindings();

        IEnumerable<CommandDefinition> GetAllCommandDefinitions();
        
        IEnumerable<CommandGestureCategory> GetAllCommandGestureCategories();
        
        IEnumerable<CommandCategoryGestureMapping> FindKeyGestures(IList<KeySequence> sequences, FindKeyGestureOption option);
    }
}