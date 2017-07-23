﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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
            HwndSource hwndSource = PresentationSource.FromDependencyObject(obj) as HwndSource;
            if (hwndSource != null)
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

        public static TAncestorType FindAncestorOrSelf<TAncestorType, TElementType>(this TElementType obj, Func<TElementType, TElementType> parentEvaluator) where TAncestorType : DependencyObject
        {
            TAncestorType ancestorType = (object)obj as TAncestorType;
            if (ancestorType != null)
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

        public static void TraverseVisualTree<T>(this DependencyObject obj, Action<T> action) where T : class
        {
            if (obj == null)
                return;
            for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(obj); ++childIndex)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, childIndex);
                if (child != null)
                {
                    T obj1 = child as T;
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
            for (int childIndex = VisualTreeHelper.GetChildrenCount(obj) - 1; childIndex >= 0; --childIndex)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, childIndex);
                if (child != null)
                {
                    T obj1 = child as T;
                    child.TraverseVisualTreeReverse(action);
                    if (obj1 != null)
                        action(obj1);
                }
            }
        }
    }
}