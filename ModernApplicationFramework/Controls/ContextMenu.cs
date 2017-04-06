using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Native.Standard;

namespace ModernApplicationFramework.Controls
{
    public class ContextMenu : System.Windows.Controls.ContextMenu, IExposeStyleKeys
    {
        public static readonly DependencyProperty PopupAnimationProperty;
        public static readonly DependencyProperty IsInsideContextMenuProperty;
        public static readonly DependencyProperty ShowKeyboardCuesProperty;
        private static ResourceKey _buttonStyleKey;
        private static ResourceKey _menuControllerStyleKey;
        private static ResourceKey _comboBoxStyleKey;
        private static ResourceKey _menuStyleKey;
        private static ResourceKey _separatorStyleKey;

        private static readonly DependencyPropertyKey IsInsideContextMenuPropertyKey;
        private ScrollViewer _scrollViewer;

        public static ResourceKey ButtonStyleKey => _buttonStyleKey ?? (_buttonStyleKey = new StyleKey<ContextMenu>());

        public static ResourceKey MenuControllerStyleKey => _menuControllerStyleKey ??
                                                            (_menuControllerStyleKey = new StyleKey<ContextMenu>());

        public static ResourceKey ComboBoxStyleKey => _comboBoxStyleKey ??
                                                      (_comboBoxStyleKey = new StyleKey<ContextMenu>());

        public static ResourceKey MenuStyleKey => _menuStyleKey ?? (_menuStyleKey = new StyleKey<ContextMenu>());

        public static ResourceKey SeparatorStyleKey => _separatorStyleKey ??
                                                       (_separatorStyleKey = new StyleKey<ContextMenu>());

        public PopupAnimation PopupAnimation
        {
            get => (PopupAnimation) GetValue(PopupAnimationProperty);
            set => SetValue(PopupAnimationProperty, value);
        }

        static ContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContextMenu),
                new FrameworkPropertyMetadata(typeof(ContextMenu)));
            PopupAnimationProperty = Popup.PopupAnimationProperty.AddOwner(typeof(ContextMenu));
            IsInsideContextMenuPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsInsideContextMenu",
                typeof(bool), typeof(ContextMenu),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse, FrameworkPropertyMetadataOptions.Inherits));
            IsInsideContextMenuProperty = IsInsideContextMenuPropertyKey.DependencyProperty;
            ShowKeyboardCuesProperty = FetchShowKeyboardCuesProperty();
        }

        public ContextMenu()
        {
            SetIsInsideContextMenu(this, true);
            PresentationSource.AddSourceChangedHandler(this, OnSourceChanged);
            var themeManager = IoC.Get<IThemeManager>();
            themeManager.OnThemeChanged += _themeManager_OnThemeChanged;
            CustomPopupPlacementCallback = GetPopupPlacements;
        }

        private Border _iconBorder;


        ResourceKey IExposeStyleKeys.MenuControllerStyleKey => MenuControllerStyleKey;

        ResourceKey IExposeStyleKeys.ComboBoxStyleKey => ComboBoxStyleKey;

        ResourceKey IExposeStyleKeys.MenuStyleKey => MenuStyleKey;

        ResourceKey IExposeStyleKeys.SeparatorStyleKey => SeparatorStyleKey;

        ResourceKey IExposeStyleKeys.ButtonStyleKey => ButtonStyleKey;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;

            _iconBorder = GetTemplateChild("PART_IconBackground") as Border;
        }

        private Color GetIconBackgroundColor()
        {
            if (_iconBorder == null)
                return Colors.Transparent;
            var b = _iconBorder.Background;
            return ((SolidColorBrush) b).Color;
        }

        public void ChangeTheme(Theme oldValue, Theme newValue)
        {
            var oldTheme = oldValue;
            var newTheme = newValue;
            var resources = Resources;
            if (oldTheme != null)
            {
                var resourceDictionaryToRemove =
                    resources.MergedDictionaries.FirstOrDefault(r => r.Source == oldTheme.GetResourceUri());
                if (resourceDictionaryToRemove != null)
                    resources.MergedDictionaries.Remove(
                        resourceDictionaryToRemove);
            }

            if (newTheme != null)
                resources.MergedDictionaries.Add(new ResourceDictionary {Source = newTheme.GetResourceUri()});
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            MenuUtilities.ProcessForDirectionalNavigation(e, this, Orientation.Vertical);
        }

        protected override void OnOpened(RoutedEventArgs e)
        {
            _scrollViewer?.ScrollToVerticalOffset(0.0);
            foreach (var item in Items)
            {
                if (item is MenuItem menuItem)
                    menuItem.SetThemedIcon();
            }
            base.OnOpened(e);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            StyleUtilities.SelectStyleForItem(element as FrameworkElement, item, this);
        }

        private static DependencyProperty FetchShowKeyboardCuesProperty()
        {
            var field = typeof(KeyboardNavigation).GetField("ShowKeyboardCuesProperty",
                BindingFlags.Static | BindingFlags.NonPublic);
            if (field == null)
                return null;
            return field.GetValue(null) as DependencyProperty;
        }

        public static bool GetIsInsideContextMenu(DependencyObject element)
        {
            Validate.IsNotNull(element, "element");
            return (bool)element.GetValue(IsInsideContextMenuProperty);
        }

        private static void SetIsInsideContextMenu(DependencyObject element, bool value)
        {
            Validate.IsNotNull(element, "element");
            element.SetValue(IsInsideContextMenuPropertyKey, Boxes.Box(value));
        }

        private CustomPopupPlacement[] GetPopupPlacements(Size popupSize, Size targetSize, Point offset)
        {
            return new[]
            {
                new CustomPopupPlacement(new Point(0.0, 0.0), PopupPrimaryAxis.Horizontal),
                new CustomPopupPlacement(new Point(0.0, -popupSize.Height), PopupPrimaryAxis.Horizontal),
                new CustomPopupPlacement(new Point(-popupSize.Width, 0.0), PopupPrimaryAxis.Horizontal),
                new CustomPopupPlacement(new Point(-popupSize.Width, -popupSize.Height), PopupPrimaryAxis.Horizontal)
            };
        }

        private void _themeManager_OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            ChangeTheme(e.OldTheme, e.NewTheme);
            ImageThemingUtilities.SetImageBackgroundColor(this, GetIconBackgroundColor());
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            ImageThemingUtilities.SetImageBackgroundColor(this, GetIconBackgroundColor());
        }

        private void OnSourceChanged(object sender, SourceChangedEventArgs e)
        {
            if (e.NewSource == null)
                return;
            var parent = LogicalTreeHelper.GetParent(this) as Popup;
            if (parent == null)
                return;
            var binding1 = new Binding {Source = this};
            var propertyPath = new PropertyPath(PopupAnimationProperty);
            binding1.Path = propertyPath;
            var binding2 = binding1;
            parent.SetBinding(Popup.PopupAnimationProperty, binding2);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ItemsSourceProperty && e.NewValue != null)
            {
                foreach (var item in Items)
                {
                    if (item is MenuItem menuItem)
                        menuItem.SetThemedIcon();
                }
            }
            base.OnPropertyChanged(e);
            if (e.Property != ShowKeyboardCuesProperty || (bool)e.NewValue)
                return;
            SetValue(ShowKeyboardCuesProperty, Boxes.BooleanTrue);
        }
    }
}