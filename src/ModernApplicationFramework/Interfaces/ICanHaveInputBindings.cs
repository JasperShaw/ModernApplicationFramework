using System.Collections.Generic;
using System.Windows;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Interfaces
{
    public interface ICanHaveInputBindings
    {
        /// <summary>
        /// Collection with all scopes this instance is assignes too. The order of the elements represents the priority of the scope.
        /// The first element has the highest priority
        /// </summary>
        IEnumerable<GestureScope> GestureScopes { get; }

        /// <summary>
        /// The element that hosts the keybindings.
        /// </summary>
        UIElement BindableElement { get; }
    }
}