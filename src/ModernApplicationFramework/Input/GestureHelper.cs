using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Input
{
    public static class GestureHelper
    {
        public static bool IsKeyGestureValid(Key key, ModifierKeys modifiers)
        {
            if (!(key >= Key.F1 && key <= Key.F24 || key >= Key.NumPad0 && key <= Key.Divide))
            {
                if ((modifiers & (ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Windows)) != 0)
                {
                    switch (key)
                    {
                        case Key.LeftCtrl:
                        case Key.RightCtrl:
                        case Key.LeftAlt:
                        case Key.RightAlt:
                        case Key.LWin:
                        case Key.RWin:
                            return false;

                        default:
                            return true;
                    }
                }

                if (key >= Key.D0 && key <= Key.D9 || key >= Key.A && key <= Key.Z)
                {
                    return false;
                }
            }
            return true;
        }

        public static IEnumerable<GestureScope> GetScopesFromElement(DependencyObject element)
        {
            if (element == null)
                return new List<GestureScope>();
            if (element is ICanHaveInputBindings gestureScopeElement)
                return gestureScopeElement.GestureScopes.ToList();
            if (element is FrameworkElement frameworkElement &&
                frameworkElement.DataContext is ICanHaveInputBindings dataContext)
                return dataContext.GestureScopes;
            return GetScopesFromElement(element.GetVisualOrLogicalParent());
        }


        public static UIElement FindParentElementFromList(UIElement inputElement, IReadOnlyCollection<UIElement> possibleParents)
        {
            if (inputElement == null)
                return null;
            if (possibleParents.FirstOrDefault(x => x == inputElement) != null)
                return inputElement;

            return FindParentElementFromList(inputElement.GetVisualOrLogicalParent() as UIElement, possibleParents);
        }
    }
}
