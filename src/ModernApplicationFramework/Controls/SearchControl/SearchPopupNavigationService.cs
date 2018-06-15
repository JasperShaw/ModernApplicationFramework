using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.SearchControl
{
    public static class SearchPopupNavigationService
    {
        public static readonly DependencyProperty IsNavigableProperty = DependencyProperty.RegisterAttached("IsNavigable", typeof(bool), typeof(SearchPopupNavigationService), new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnIsNavigablePropertyChanged));
        public static readonly DependencyProperty IsCurrentLocationProperty = DependencyProperty.RegisterAttached("IsCurrentLocation", typeof(bool), typeof(SearchPopupNavigationService), new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnIsCurrentLocationPropertyChanged));
        public static readonly DependencyProperty IsNavigationEnabledProperty = DependencyProperty.RegisterAttached("IsNavigationEnabled", typeof(bool), typeof(SearchPopupNavigationService), new FrameworkPropertyMetadata(Boxes.BooleanTrue));
        public static readonly DependencyProperty CurrentLocationProperty = DependencyProperty.RegisterAttached("CurrentLocation", typeof(FrameworkElement), typeof(SearchPopupNavigationService), new PropertyMetadata(OnCurrentLocationPropertyChanged));
        public static readonly DependencyProperty CurrentLocationSetModeProperty = DependencyProperty.RegisterAttached("CurrentLocationSetMode", typeof(CurrentLocationSetMode), typeof(SearchPopupNavigationService));

        public static bool GetIsNavigable(FrameworkElement element)
        {
            Validate.IsNotNull(element, nameof(element));
            return (bool)element.GetValue(IsNavigableProperty);
        }

        public static void SetIsNavigable(FrameworkElement element, bool value)
        {
            Validate.IsNotNull(element, nameof(element));
            element.SetValue(IsNavigableProperty, Boxes.Box(value));
        }

        public static bool GetIsCurrentLocation(FrameworkElement element)
        {
            Validate.IsNotNull(element, nameof(element));
            return (bool)element.GetValue(IsCurrentLocationProperty);
        }

        public static void SetIsCurrentLocation(FrameworkElement element, bool value)
        {
            Validate.IsNotNull(element, nameof(element));
            var ancestor = element.FindAncestor<Popup>();
            if (ancestor != null && !GetIsNavigationEnabled(ancestor))
                return;
            element.SetValue(IsCurrentLocationProperty, Boxes.Box(value));
        }

        public static bool GetIsNavigationEnabled(Popup searchControlPopup)
        {
            Validate.IsNotNull(searchControlPopup, nameof(searchControlPopup));
            return (bool)searchControlPopup.GetValue(IsNavigationEnabledProperty);
        }

        public static void SetIsNavigationEnabled(Popup searchControlPopup, bool value)
        {
            Validate.IsNotNull(searchControlPopup, nameof(searchControlPopup));
            searchControlPopup.SetValue(IsNavigationEnabledProperty, Boxes.Box(value));
        }

        public static FrameworkElement GetCurrentLocation(Popup searchControlPopup)
        {
            var element = searchControlPopup?.GetValue(CurrentLocationProperty) as FrameworkElement;
            if (element != null && !element.IsConnectedToPresentationSource())
            {
                SetIsCurrentLocation(element, false);
                SetCurrentLocation(searchControlPopup, null);
                element = null;
            }
            return element;
        }

        public static void SetCurrentLocation(Popup searchControlPopup, FrameworkElement value)
        {
            Validate.IsNotNull(searchControlPopup, nameof(searchControlPopup));
            searchControlPopup.SetValue(CurrentLocationProperty, value);
        }

        public static CurrentLocationSetMode GetCurrentLocationSetMode(Popup searchControlPopup)
        {
            Validate.IsNotNull(searchControlPopup, nameof(searchControlPopup));
            return (CurrentLocationSetMode)searchControlPopup.GetValue(CurrentLocationSetModeProperty);
        }

        public static void SetCurrentLocationSetMode(Popup searchControlPopup, CurrentLocationSetMode value)
        {
            Validate.IsNotNull(searchControlPopup, nameof(searchControlPopup));
            searchControlPopup.SetValue(CurrentLocationSetModeProperty, value);
        }

        internal static void Register(Popup searchControlPopup)
        {
            Validate.IsNotNull(searchControlPopup, nameof(searchControlPopup));
            ClearCurrentLocation(searchControlPopup);
        }

        internal static void Unregister(Popup searchControlPopup)
        {
            Validate.IsNotNull(searchControlPopup, nameof(searchControlPopup));
            ClearCurrentLocation(searchControlPopup);
        }

        internal static void ClearCurrentLocation(Popup searchControlPopup)
        {
            Validate.IsNotNull(searchControlPopup, nameof(searchControlPopup));
            var currentLocation = GetCurrentLocation(searchControlPopup);
            if (currentLocation == null)
                return;
            SetIsCurrentLocation(currentLocation, false);
            SetCurrentLocation(searchControlPopup, null);
        }

        private static void OnIsNavigablePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool)e.NewValue;
            var frameworkElement = obj as FrameworkElement;
            if (newValue)
            {
                frameworkElement.PreviewMouseDown += OnNavigableControlMouseHander;
                frameworkElement.PreviewMouseMove += OnNavigableControlMouseHander;
            }
            else
            {
                frameworkElement.PreviewMouseMove -= OnNavigableControlMouseHander;
                frameworkElement.PreviewMouseDown -= OnNavigableControlMouseHander;
            }
        }

        private static void OnNavigableControlMouseHander(object sender, MouseEventArgs mouseEventArgs)
        {
            var element = sender as FrameworkElement;
            var ancestor = element.FindAncestor<Popup>();
            var currentLocation = GetCurrentLocation(ancestor);
            if (ancestor == null || currentLocation == element)
                return;
            SetIsCurrentLocation(element, true);
            SetCurrentLocationSetMode(ancestor, CurrentLocationSetMode.ByMouse);
        }

        private static void OnIsCurrentLocationPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool)e.NewValue;
            var frameworkElement = obj as FrameworkElement;
            if (!(frameworkElement != null & newValue))
                return;
            var ancestor = frameworkElement.FindAncestor<Popup>();
            var currentLocation = GetCurrentLocation(ancestor);
            if (ancestor == null || currentLocation == frameworkElement)
                return;
            if (currentLocation != null)
                SetIsCurrentLocation(currentLocation, false);
            SetCurrentLocation(ancestor, frameworkElement);
        }

        private static void OnCurrentLocationPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            SetCurrentLocationSetMode((Popup)obj, CurrentLocationSetMode.ByKeyboard);
        }

        private static DependencyObject GetNextNavigationStop(DependencyObject e, DependencyObject container)
        {
            var e1 = e != null ? GetNextInTree(e, container) : container;
            while (e1 != null && !IsNavigable(e1))
                e1 = GetNextInTree(e1, container);
            return e1;
        }

        private static DependencyObject GetPreviousNavigationStop(DependencyObject e, DependencyObject container)
        {
            var e1 = e != null ? GetPreviousInTree(e, container) : GetLastInTree(container);
            while (e1 != null && !IsNavigable(e1))
                e1 = GetPreviousInTree(e1, container);
            return e1;
        }

        private static DependencyObject GetNextInTree(DependencyObject e, DependencyObject container)
        {
            DependencyObject dependencyObject = null;
            if (e == container || !IsNavigable(e))
                dependencyObject = GetFirstChild(e);
            if (dependencyObject != null || e == container)
                return dependencyObject;
            var e1 = e;
            do
            {
                var nextSibling = GetNextSibling(e1);
                if (nextSibling != null)
                    return nextSibling;
                e1 = GetParent(e1);
            }
            while (e1 != null && e1 != container);
            return null;
        }

        private static DependencyObject GetPreviousInTree(DependencyObject e, DependencyObject container)
        {
            if (e == container)
                return null;
            var previousSibling = GetPreviousSibling(e);
            if (previousSibling == null)
                return GetParent(e);
            if (IsNavigable(previousSibling))
                return previousSibling;
            return GetLastInTree(previousSibling);
        }

        private static DependencyObject GetLastInTree(DependencyObject container)
        {
            DependencyObject dependencyObject;
            do
            {
                dependencyObject = container;
                container = GetLastChild(container);
            }
            while (container != null && !IsNavigable(container));
            if (container != null)
                return container;
            return dependencyObject;
        }

        private static DependencyObject GetParent(DependencyObject e)
        {
            return e.GetVisualOrLogicalParent();
        }

        private static DependencyObject GetFirstChild(DependencyObject e)
        {
            return e.FindDescendant<DependencyObject>();
        }

        private static DependencyObject GetLastChild(DependencyObject e)
        {
            return e.FindDescendantReverse<DependencyObject>();
        }

        private static DependencyObject GetPreviousSibling(DependencyObject e)
        {
            var parent = GetParent(e);
            DependencyObject dependencyObject = null;
            for (var childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(parent); ++childIndex)
            {
                var child = VisualTreeHelper.GetChild(parent, childIndex);
                if (child != e)
                    dependencyObject = child;
                else
                    break;
            }
            return dependencyObject;
        }

        private static DependencyObject GetNextSibling(DependencyObject e)
        {
            var parent = GetParent(e);
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            var childIndex1 = 0;
            while (childIndex1 < childrenCount && VisualTreeHelper.GetChild(parent, childIndex1) != e)
                ++childIndex1;
            var childIndex2 = childIndex1 + 1;
            if (childIndex2 < childrenCount)
                return VisualTreeHelper.GetChild(parent, childIndex2);
            return null;
        }

        private static bool IsNavigable(DependencyObject e)
        {
            var element = e as FrameworkElement;
            if (element != null && element.IsEnabled && element.IsVisible)
                return GetIsNavigable(element);
            return false;
        }

        internal static bool NavigateNext(Popup searchControlPopup)
        {
            return NavigateNext(searchControlPopup, 1);
        }

        internal static bool NavigateNext(Popup searchControlPopup, int stepCount)
        {
            if (searchControlPopup == null)
                throw new ArgumentNullException(nameof(searchControlPopup));
            var currentLocation = GetCurrentLocation(searchControlPopup);
            return NavigateNext(searchControlPopup, currentLocation, stepCount);
        }

        internal static bool NavigateFirst(Popup searchControlPopup)
        {
            if (searchControlPopup == null)
                throw new ArgumentNullException(nameof(searchControlPopup));
            return NavigateNext(searchControlPopup, null, 1);
        }

        private static bool NavigateNext(Popup searchControlPopup, FrameworkElement currentLocation, int stepCount)
        {
            if (searchControlPopup == null)
                throw new ArgumentNullException(nameof(searchControlPopup));
            var element = currentLocation;
            while (stepCount-- > 0)
            {
                element = GetNextNavigationStop(element, searchControlPopup.Child) as FrameworkElement;
                if (element == null && currentLocation != null)
                    element = GetNextNavigationStop(null, searchControlPopup.Child) as FrameworkElement;
            }
            if (element == null)
                return false;
            SetIsCurrentLocation(element, true);
            return true;
        }

        internal static bool NavigatePrevious(Popup searchControlPopup)
        {
            return NavigatePrevious(searchControlPopup, 1);
        }

        internal static bool NavigatePrevious(Popup searchControlPopup, int stepCount)
        {
            if (searchControlPopup == null)
                throw new ArgumentNullException(nameof(searchControlPopup));
            var currentLocation = GetCurrentLocation(searchControlPopup);
            return NavigatePrevious(searchControlPopup, currentLocation, stepCount);
        }

        internal static bool NavigateLast(Popup searchControlPopup)
        {
            if (searchControlPopup == null)
                throw new ArgumentNullException(nameof(searchControlPopup));
            return NavigatePrevious(searchControlPopup, null, 1);
        }

        private static bool NavigatePrevious(Popup searchControlPopup, FrameworkElement currentLocation, int stepCount)
        {
            if (searchControlPopup == null)
                throw new ArgumentNullException(nameof(searchControlPopup));
            var element = currentLocation;
            while (stepCount-- > 0)
            {
                element = GetPreviousNavigationStop(element, searchControlPopup.Child) as FrameworkElement;
                if (element == null && currentLocation != null)
                    element = GetPreviousNavigationStop(null, searchControlPopup.Child) as FrameworkElement;
            }
            if (element == null)
                return false;
            SetIsCurrentLocation(element, true);
            return true;
        }
    }

    public enum CurrentLocationSetMode
    {
        ByKeyboard,
        ByMouse,
    }
}
