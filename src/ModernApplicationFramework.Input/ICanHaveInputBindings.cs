using System.Collections.Generic;
using System.Windows;

namespace ModernApplicationFramework.Input
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
