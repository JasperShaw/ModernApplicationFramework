using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Interfaces.Services
{
    /// <summary>
    /// A <see cref="IKeyGestureService"/> administrates Keybindings to a <see cref="UIElement"/>
    /// </summary>
    public interface IKeyGestureService
    {
        void BindKeyGestures(ICanHaveInputBindings hostingModel);

        /// <summary>
        /// Loads all available key gestures and applies them to their command
        /// </summary>
        void Load();

        /// <summary>
        /// Loads all default key gestures and applies them to their command
        /// </summary>
        void LoadDefaults();

        IEnumerable<CommandCategoryGestureMapping> GetAllBindings();
    }
}