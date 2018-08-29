using System;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Platform.Enums;

namespace ModernApplicationFramework.Core.Utilities
{
    internal static class VisualUtilities
    {
        public static T FindChild<T>(DependencyObject depObj, string childName) where T : DependencyObject
        {
            // Confirm obj is valid. 
            if (depObj == null)
                return null;

            // success case
            if (depObj is T && ((FrameworkElement) depObj).Name == childName)
                return depObj as T;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                //DFS
                var obj = FindChild<T>(child, childName);

                if (obj != null)
                    return obj;
            }

            return null;
        }

        public static void HitTestVisibleElements(Visual visual, HitTestResultCallback resultCallback,
                                                  HitTestParameters parameters)
        {
            VisualTreeHelper.HitTest(visual, ExcludeNonVisualElements, resultCallback, parameters);
        }

        internal static bool ModifyStyle(IntPtr hWnd, int styleToRemove, int styleToAdd)
        {
            var windowLong = User32.GetWindowLong(hWnd, (int) Gwl.Style);
            var dwNewLong = windowLong & ~styleToRemove | styleToAdd;
            if (dwNewLong == windowLong)
                return false;
            NativeMethods.SetWindowLong(hWnd, Gwl.Style, dwNewLong);
            return true;
        }

        private static HitTestFilterBehavior ExcludeNonVisualElements(DependencyObject potentialHitTestTarget)
        {
            if (!(potentialHitTestTarget is Visual))
                return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
            var uiElement = potentialHitTestTarget as UIElement;
            return uiElement == null || uiElement.IsVisible && uiElement.IsEnabled
                ? HitTestFilterBehavior.Continue
                : HitTestFilterBehavior.ContinueSkipSelfAndChildren;
        }

        public static T FindLogicalAncestor<T>(this DependencyObject dependencyObject) where T : class
        {
            DependencyObject target = dependencyObject;
            do
            {
                var current = target;
                target = LogicalTreeHelper.GetParent(target) ?? VisualTreeHelper.GetParent(current);
            } while (target != null && !(target is T));
            return target as T;
        }
    }
}