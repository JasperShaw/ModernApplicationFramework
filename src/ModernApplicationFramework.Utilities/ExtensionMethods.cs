using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ModernApplicationFramework.Utilities.NativeMethods;

namespace ModernApplicationFramework.Utilities
{
    public static class ExtensionMethods
    {
        public static void RaiseEvent(this EventHandler eventHandler, object source)
        {
            RaiseEvent(eventHandler, source, EventArgs.Empty);
        }

        public static void RaiseEvent<T>(this EventHandler<T> eventHandler, object source, T args) where T : class
        {
            eventHandler?.Invoke(source, args);
        }

        public static void RaiseEvent(this EventHandler eventHandler, object source, EventArgs args)
        {
            eventHandler?.Invoke(source, args);
        }

        public static void RaiseEvent(this PropertyChangedEventHandler eventHandler, object source, string propertyName)
        {
            eventHandler?.Invoke(source, new PropertyChangedEventArgs(propertyName));
        }

        public static bool IsConnectedToPresentationSource(this DependencyObject obj)
        {
            return PresentationSource.FromDependencyObject(obj) != null;
        }

        public static bool AcquireWin32Focus(this DependencyObject obj, out IntPtr previousFocus)
        {
            if (PresentationSource.FromDependencyObject(obj) is HwndSource hwndSource)
            {
                previousFocus = User32.GetFocus();
                if (previousFocus != hwndSource.Handle)
                {
                    User32.SetFocus(hwndSource.Handle);
                    return true;
                }
            }
            previousFocus = IntPtr.Zero;
            return false;
        }

        public static DependencyObject GetVisualOrLogicalParent(this DependencyObject sourceElement)
        {
            if (sourceElement == null)
                return null;
            if (sourceElement is Visual)
                return VisualTreeHelper.GetParent(sourceElement) ?? LogicalTreeHelper.GetParent(sourceElement);
            return LogicalTreeHelper.GetParent(sourceElement);
        }

        public static TAncestorType FindAncestorOrSelf<TAncestorType>(this Visual obj) where TAncestorType : DependencyObject
        {
            return obj.FindAncestorOrSelf<TAncestorType, DependencyObject>(GetVisualOrLogicalParent);
        }

        public static TAncestorType FindAncestorOrSelf<TAncestorType, TElementType>(this TElementType obj, Func<TElementType, TElementType> parentEvaluator) where TAncestorType : DependencyObject
        {
            if (obj is TAncestorType ancestorType)
                return ancestorType;
            return obj.FindAncestor<TAncestorType, TElementType>(parentEvaluator);
        }

        public static object FindAncestorOrSelf<TElementType>(this TElementType obj, Func<TElementType, TElementType> parentEvaluator, Func<TElementType, bool> ancestorSelector)
        {
            if (ancestorSelector(obj))
                return obj;
            return obj.FindAncestor(parentEvaluator, ancestorSelector);
        }

        public static TAncestorType FindAncestor<TAncestorType>(this Visual obj) where TAncestorType : DependencyObject
        {
            return FindAncestor<TAncestorType, DependencyObject>(obj, GetVisualOrLogicalParent);
        }

        public static TAncestorType FindAncestor<TAncestorType, TElementType>(this TElementType obj,
            Func<TElementType, TElementType>
                parentEvaluator)
            where TAncestorType : class
        {
            return FindAncestor(obj, parentEvaluator, ancestor => (object)ancestor is TAncestorType) as TAncestorType;
        }

        public static bool IsAncestorOf<TElementType>(this TElementType element, TElementType other, Func<TElementType, TElementType> parentEvaluator) where TElementType : class
        {
            for (var elementType = parentEvaluator(other); elementType != null; elementType = parentEvaluator(elementType))
            {
                if (elementType == element)
                    return true;
            }
            return false;
        }

        public static bool IsLogicalAncestorOf(this DependencyObject element, DependencyObject other)
        {
            if (other == null)
                return false;
            return element.IsAncestorOf(other, GetVisualOrLogicalParent);
        }

        public static TSource GetOriginalSource<TSource>(this MouseButtonEventArgs mouseArgs) where TSource: FrameworkElement
        {
            if (!(mouseArgs.OriginalSource is FrameworkElement frameworkElement))
                return null;
            var elementItem = frameworkElement.FindAncestorOrSelf<TSource>();
            return elementItem;
        }

        public static object FindAncestor<TElementType>(this TElementType obj,
            Func<TElementType, TElementType> parentEvaluator,
            Func<TElementType, bool> ancestorSelector)
        {
            for (var elementType = parentEvaluator(obj);
                elementType != null;
                elementType = parentEvaluator(elementType))
            {
                if (ancestorSelector(elementType))
                    return elementType;
            }
            return null;
        }

        public static IEnumerable<T> FindDescendants<T>(this DependencyObject obj) where T : class
        {
            if (obj == null)
                return Enumerable.Empty<T>();
            var descendants = new List<T>();
            obj.TraverseVisualTree<T>(child => descendants.Add(child));
            return descendants;
        }

        public static T FindDescendant<T>(this DependencyObject obj) where T : class
        {
            if (obj == null)
                return default;
            var obj1 = default(T);
            for (var childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(obj); ++childIndex)
            {
                var child = VisualTreeHelper.GetChild(obj, childIndex);
                if (child != null)
                {
                    obj1 = child as T;
                    if (obj1 == null)
                    {
                        obj1 = child.FindDescendant<T>();
                        if (obj1 != null)
                            break;
                    }
                    else
                        break;
                }
            }
            return obj1;
        }

        public static T FindDescendantReverse<T>(this DependencyObject obj) where T : class
        {
            if (obj == null)
                return default;
            T obj1 = default;
            for (int childIndex = VisualTreeHelper.GetChildrenCount(obj) - 1; childIndex >= 0; --childIndex)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, childIndex);
                if (child != null)
                {
                    obj1 = child as T;
                    if (obj1 == null)
                    {
                        obj1 = child.FindDescendantReverse<T>();
                        if (obj1 != null)
                            break;
                    }
                    else
                        break;
                }
            }
            return obj1;
        }

        public static void TraverseVisualTree<T>(this DependencyObject obj, Action<T> action) where T : class
        {
            if (obj == null)
                return;
            for (var childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(obj); ++childIndex)
            {
                var child = VisualTreeHelper.GetChild(obj, childIndex);
                if (child != null)
                {
                    var obj1 = child as T;
                    child.TraverseVisualTreeReverse(action);
                    if (obj1 != null)
                        action(obj1);
                }
            }
        }

        public static void TraverseVisualTreeReverse<T>(this DependencyObject obj, Action<T> action) where T : class
        {
            if (obj == null)
                return;
            for (var childIndex = VisualTreeHelper.GetChildrenCount(obj) - 1; childIndex >= 0; --childIndex)
            {
                var child = VisualTreeHelper.GetChild(obj, childIndex);
                if (child != null)
                {
                    var obj1 = child as T;
                    child.TraverseVisualTreeReverse(action);
                    if (obj1 != null)
                        action(obj1);
                }
            }
        }

        public static bool IsSignificantlyGreaterThan(this double value1, double value2)
        {
            return value1.GreaterThan(value2);
        }

        public static bool GreaterThan(this double value1, double value2)
        {
            if (value1 > value2)
                return !value1.AreClose(value2);
            return false;
        }

        public static bool AreClose(this double value1, double value2)
        {
            if (value1.IsNonreal() || value2.IsNonreal())
                return value1.CompareTo(value2) == 0;
            if (value1 == value2)
                return true;
            var num = value1 - value2;
            if (num < 1.53E-06)
                return num > -1.53E-06;
            return false;
        }

        public static bool IsNonreal(this double value)
        {
            if (!double.IsNaN(value))
                return double.IsInfinity(value);
            return true;
        }

        public static bool IsNearlyEqual(this double value1, double value2)
        {
            return value1.AreClose(value2);
        }

        public static int IndexOf<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            var num = 0;
            foreach (var obj in items)
            {
                if (predicate(obj))
                    return num;
                ++num;
            }
            return -1;
        }
    }
}
