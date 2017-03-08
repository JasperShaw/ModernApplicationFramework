using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ModernApplicationFramework.Core.Platform;

namespace ModernApplicationFramework.Controls
{
    public class CheckableListBoxItem : ListBoxItem
    {
        private static readonly DependencyProperty InternalIsCheckedProperty =
            DependencyProperty.Register("InternalIsChecked",
                typeof(bool?), typeof(CheckableListBoxItem),
                new FrameworkPropertyMetadata(Boxes.BoolFalse, FrameworkPropertyMetadataOptions.AffectsRender,
                    OnInternalIsCheckedChanged));

        public static DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool?),
            typeof(CheckableListBoxItem),
            new FrameworkPropertyMetadata(Boxes.BoolFalse, FrameworkPropertyMetadataOptions.AffectsRender,
                OnIsCheckedChanged));

        public static readonly RoutedEvent CheckedEvent =
            ToggleButton.CheckedEvent.AddOwner(typeof(CheckableListBoxItem));

        public static readonly RoutedEvent UncheckedEvent =
            ToggleButton.UncheckedEvent.AddOwner(typeof(CheckableListBoxItem));


        public static DependencyProperty IsToggleEnabledProperty = DependencyProperty.Register("IsToggleEnabled",
            typeof(bool), typeof(CheckableListBoxItem), new FrameworkPropertyMetadata(Boxes.BoolTrue));


        private bool _contentLoaded;

        public CheckableListBoxItem()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler Checked
        {
            add => AddHandler(CheckedEvent, value);
            remove => RemoveHandler(CheckedEvent, value);
        }

        public event RoutedEventHandler Unchecked
        {
            add => AddHandler(UncheckedEvent, value);
            remove => RemoveHandler(UncheckedEvent, value);
        }

        public bool? IsChecked
        {
            get => (bool?) GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public bool IsToggleEnabled
        {
            get => (bool) GetValue(IsToggleEnabledProperty);
            set => SetValue(IsToggleEnabledProperty, Boxes.Box(value));
        }

        private bool? InternalIsChecked
        {
            set => SetValue(InternalIsCheckedProperty, value);
        }

        public void OnIsCheckedChanged(DependencyPropertyChangedEventArgs e)
        {
            InternalIsChecked = (bool?) e.NewValue;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.OemPlus:
                case Key.Add:
                    e.Handled = true;
                    if (!IsToggleEnabled)
                        return;
                    IsChecked = true;
                    break;
                case Key.OemMinus:
                case Key.Subtract:
                    e.Handled = true;
                    if (!IsToggleEnabled)
                        return;
                    IsChecked = false;
                    break;
                default:
                    var checkableListBox = ItemsControl.ItemsControlFromItemContainer(this) as CheckableListBox;
                    if (checkableListBox == null || !checkableListBox.ToggleKeys.Contains(e.Key))
                        return;
                    e.Handled = true;
                    if (!IsToggleEnabled)
                        return;
                    bool? nullable;
                    if (!IsChecked.HasValue)
                        nullable = true;
                    else
                    {
                        var isChecked = IsChecked;
                        // ReSharper disable once MergeConditionalExpression
                        nullable = !isChecked.GetValueOrDefault();
                    }
                    IsChecked = nullable;
                    break;
            }
        }

        private static void OnInternalIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CheckableListBoxItem) d).OnInternalIsCheckedChanged(e);
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CheckableListBoxItem) d).OnIsCheckedChanged(e);
        }

        private void InitializeComponent()
        {
            if (_contentLoaded)
                return;
            _contentLoaded = true;
            Application.LoadComponent(this,
                new Uri("/ModernApplicationFramework;component/Themes/Generic/CheckableListBoxItem.xaml",
                    UriKind.Relative));
        }

        private void OnInternalIsCheckedChanged(DependencyPropertyChangedEventArgs e)
        {
            IsChecked = (bool?) e.NewValue;
        }
    }
}