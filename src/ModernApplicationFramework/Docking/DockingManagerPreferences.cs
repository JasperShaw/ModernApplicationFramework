using System;
using System.Windows;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Docking
{
    public class DockingManagerPreferences : DependencyObject
    {
        public static readonly DependencyProperty DocumentDockPreferenceProperty = DependencyProperty.Register(nameof(DocumentDockPreference), typeof(DockPreference), typeof(DockingManagerPreferences), new PropertyMetadata(DockPreference.DockAtBeginning));
        public static readonly DependencyProperty TabDockPreferenceProperty = DependencyProperty.Register(nameof(TabDockPreference), typeof(DockPreference), typeof(DockingManagerPreferences), new PropertyMetadata(DockPreference.DockAtBeginning));
        public static readonly DependencyProperty AllowDocumentTabAutoDockingProperty = DependencyProperty.Register(nameof(AllowDocumentTabAutoDocking), typeof(bool), typeof(DockingManagerPreferences), new PropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty AllowTabGroupTabAutoDockingProperty = DependencyProperty.Register(nameof(AllowTabGroupTabAutoDocking), typeof(bool), typeof(DockingManagerPreferences), new PropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty AutoHideHoverDelayProperty = DependencyProperty.Register(nameof(AutoHideHoverDelay), typeof(TimeSpan), typeof(DockingManagerPreferences), new PropertyMetadata(TimeSpan.FromMilliseconds(SystemParameters.MenuShowDelay)));
        public static readonly DependencyProperty AutoHideMouseExitGracePeriodProperty = DependencyProperty.Register(nameof(AutoHideMouseExitGracePeriod), typeof(TimeSpan), typeof(DockingManagerPreferences), new PropertyMetadata(TimeSpan.FromMilliseconds(500.0)));
        public static readonly DependencyProperty HideOnlyActiveViewProperty = DependencyProperty.Register(nameof(HideOnlyActiveView), typeof(bool), typeof(DockingManagerPreferences), new PropertyMetadata(Boxes.BooleanTrue));
        public static readonly DependencyProperty AutoHideOnlyActiveViewProperty = DependencyProperty.Register(nameof(AutoHideOnlyActiveView), typeof(bool), typeof(DockingManagerPreferences), new PropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty MaintainPinStatusProperty = DependencyProperty.Register(nameof(MaintainPinStatus), typeof(bool), typeof(DockingManagerPreferences), new PropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty IsPinnedTabPanelSeparateProperty = DependencyProperty.Register(nameof(IsPinnedTabPanelSeparate), typeof(bool), typeof(DockingManagerPreferences), new PropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty ShowPinButtonInUnpinnedTabsProperty = DependencyProperty.Register(nameof(ShowPinButtonInUnpinnedTabs), typeof(bool), typeof(DockingManagerPreferences), new PropertyMetadata(Boxes.BooleanTrue));
        public static readonly DependencyProperty AutoZOrderDelayProperty = DependencyProperty.Register(nameof(AutoZOrderDelay), typeof(TimeSpan), typeof(DockingManagerPreferences), new PropertyMetadata(TimeSpan.FromMilliseconds(500.0)));
        public static readonly DependencyProperty ShowAutoHiddenWindowsOnHoverProperty = DependencyProperty.Register(nameof(ShowAutoHiddenWindowsOnHover), typeof(bool), typeof(DockingManagerPreferences), new PropertyMetadata(Boxes.BooleanFalse));

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

        public DockPreference TabDockPreference
        {
            get => (DockPreference)GetValue(TabDockPreferenceProperty);
            set => SetValue(TabDockPreferenceProperty, value);
        }

        public TimeSpan AutoHideHoverDelay
        {
            get => (TimeSpan)GetValue(AutoHideHoverDelayProperty);
            set => SetValue(AutoHideHoverDelayProperty, value);
        }

        public bool AllowDocumentTabAutoDocking
        {
            get => (bool)GetValue(AllowDocumentTabAutoDockingProperty);
            set => SetValue(AllowDocumentTabAutoDockingProperty, Boxes.Box(value));
        }

        public bool AllowTabGroupTabAutoDocking
        {
            get => (bool)GetValue(AllowTabGroupTabAutoDockingProperty);
            set => SetValue(AllowTabGroupTabAutoDockingProperty, Boxes.Box(value));
        }

        public bool HideOnlyActiveView
        {
            get => (bool)GetValue(HideOnlyActiveViewProperty);
            set => SetValue(HideOnlyActiveViewProperty, Boxes.Box(value));
        }

        public bool AutoHideOnlyActiveView
        {
            get => (bool)GetValue(AutoHideOnlyActiveViewProperty);
            set => SetValue(AutoHideOnlyActiveViewProperty, Boxes.Box(value));
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

        public TimeSpan AutoZOrderDelay
        {
            get => (TimeSpan)GetValue(AutoZOrderDelayProperty);
            set => SetValue(AutoZOrderDelayProperty, value);
        }

        public bool ShowAutoHiddenWindowsOnHover
        {
            get => (bool)GetValue(ShowAutoHiddenWindowsOnHoverProperty);
            set => SetValue(ShowAutoHiddenWindowsOnHoverProperty, Boxes.Box(value));
        }
    }
}
