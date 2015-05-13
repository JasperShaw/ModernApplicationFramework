using System;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Core.Platform;

namespace ModernApplicationFramework.Core.Utilities
{
    internal static class VisualUtilities
    {
        public static void HitTestVisibleElements(Visual visual, HitTestResultCallback resultCallback, HitTestParameters parameters)
        {
            VisualTreeHelper.HitTest(visual, ExcludeNonVisualElements, resultCallback, parameters);
        }

        private static HitTestFilterBehavior ExcludeNonVisualElements(DependencyObject potentialHitTestTarget)
        {
            if (!(potentialHitTestTarget is Visual))
                return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
            var uiElement = potentialHitTestTarget as UIElement;
            return uiElement == null || uiElement.IsVisible && uiElement.IsEnabled ? HitTestFilterBehavior.Continue : HitTestFilterBehavior.ContinueSkipSelfAndChildren;
        }

        public static DependencyObject GetVisualOrLogicalParent1(this DependencyObject sourceElement)
        {
            if (sourceElement == null)
                return null;
            if (sourceElement is Visual)
                return VisualTreeHelper.GetParent(sourceElement) ?? LogicalTreeHelper.GetParent(sourceElement);
            return LogicalTreeHelper.GetParent(sourceElement);
        }

        internal static bool ModifyStyle(IntPtr hWnd, int styleToRemove, int styleToAdd)
        {
            int windowLong = NativeMethods.NativeMethods.GetWindowLong(hWnd, Gwl.Style);
            int dwNewLong = windowLong & ~styleToRemove | styleToAdd;
            if (dwNewLong == windowLong)
                return false;
            NativeMethods.NativeMethods.SetWindowLong(hWnd, Gwl.Style, dwNewLong);
            return true;
        }

        public static TAncestorType FindAncestor<TAncestorType>(this Visual obj) where TAncestorType : DependencyObject
        {
            return FindAncestor<TAncestorType, DependencyObject>(obj, GetVisualOrLogicalParent1);
        }

        public static TAncestorType FindAncestor<TAncestorType, TElementType>(this TElementType obj, Func<TElementType, TElementType> parentEvaluator) where TAncestorType : class
        {
            return FindAncestor(obj, parentEvaluator, ancestor => (object)ancestor is TAncestorType) as TAncestorType;
        }

        public static object FindAncestor<TElementType>(this TElementType obj, Func<TElementType, TElementType> parentEvaluator, Func<TElementType, bool> ancestorSelector)
        {
            for (var elementType = parentEvaluator(obj); elementType != null; elementType = parentEvaluator(elementType))
            {
                if (ancestorSelector(elementType))
                    return elementType;
            }
            return null;
        }

        public static T FindChild<T>(DependencyObject depObj, string childName) where T : DependencyObject
        {
            // Confirm obj is valid. 
            if (depObj == null) return null;

            // success case
            if (depObj is T && ((FrameworkElement)depObj).Name == childName)
                return depObj as T;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                //DFS
                T obj = FindChild<T>(child, childName);

                if (obj != null)
                    return obj;
            }

            return null;
        }
    }
}
