using System;
using System.Windows;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Docking
{
    public class DockingManagerPreferences : DependencyObject
    {
        public static DockingManagerPreferences Instance
        {
            get => _instance ?? (_instance = new DockingManagerPreferences());
            private set => _instance = value;
        }

        public static readonly DependencyProperty DocumentDockPreferenceProperty = DependencyProperty.Register(nameof(DocumentDockPreference), typeof(DockPreference), typeof(DockingManagerPreferences), new PropertyMetadata(DockPreference.DockAtBeginning));
        public static readonly DependencyProperty AutoHideHoverDelayProperty = DependencyProperty.Register(nameof(AutoHideHoverDelay), typeof(TimeSpan), typeof(DockingManagerPreferences), new PropertyMetadata(TimeSpan.FromMilliseconds(SystemParameters.MenuShowDelay)));
        public static readonly DependencyProperty AutoHideMouseExitGracePeriodProperty = DependencyProperty.Register(nameof(AutoHideMouseExitGracePeriod), typeof(TimeSpan), typeof(DockingManagerPreferences), new PropertyMetadata(TimeSpan.FromMilliseconds(500.0)));
        public static readonly DependencyProperty MaintainPinStatusProperty = DependencyProperty.Register(nameof(MaintainPinStatus), typeof(bool), typeof(DockingManagerPreferences), new PropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty IsPinnedTabPanelSeparateProperty = DependencyProperty.Register(nameof(IsPinnedTabPanelSeparate), typeof(bool), typeof(DockingManagerPreferences), new PropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty ShowPinButtonInUnpinnedTabsProperty = DependencyProperty.Register(nameof(ShowPinButtonInUnpinnedTabs), typeof(bool), typeof(DockingManagerPreferences), new PropertyMetadata(Boxes.BooleanTrue));
        public static readonly DependencyProperty ShowAutoHiddenWindowsOnHoverProperty = DependencyProperty.Register(nameof(ShowAutoHiddenWindowsOnHover), typeof(bool), typeof(DockingManagerPreferences), new PropertyMetadata(Boxes.BooleanFalse));
        private static DockingManagerPreferences _instance;

        public TimeSpan AutoHideMouseExitGracePeriod
        {
            get => (TimeSpan)GetValue(AutoHideMouseExitGracePeriodProperty);
            set => SetValue(AutoHideMouseExitGracePeriodProperty, value);
        }

        public DockPreference DocumentDockPreference
        {
            get => (DockPreference)GetValue(DocumentDockPreferenceProperty);
            set => SetValue(DocumentDockPreferenceProperty, value);
        }

        public TimeSpan AutoHideHoverDelay
        {
            get => (TimeSpan)GetValue(AutoHideHoverDelayProperty);
            set => SetValue(AutoHideHoverDelayProperty, value);
        }


        public bool MaintainPinStatus
        {
            get => (bool)GetValue(MaintainPinStatusProperty);
            set => SetValue(MaintainPinStatusProperty, Boxes.Box(value));
        }

        public bool IsPinnedTabPanelSeparate
        {
            get => (bool)GetValue(IsPinnedTabPanelSeparateProperty);
            set => SetValue(IsPinnedTabPanelSeparateProperty, Boxes.Box(value));
        }

        public bool ShowPinButtonInUnpinnedTabs
        {
            get => (bool)GetValue(ShowPinButtonInUnpinnedTabsProperty);
            set => SetValue(ShowPinButtonInUnpinnedTabsProperty, Boxes.Box(value));
        }

        public bool ShowAutoHiddenWindowsOnHover
        {
            get => (bool)GetValue(ShowAutoHiddenWindowsOnHoverProperty);
            set => SetValue(ShowAutoHiddenWindowsOnHoverProperty, Boxes.Box(value));
        }

        public DockingManagerPreferences()
        {
            Instance = this;
        }
    }
}
