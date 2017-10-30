using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.ListBoxes
{
    /// <inheritdoc />
    /// <summary>
    /// List box item control used for a <see cref="T:ModernApplicationFramework.Controls.ListBoxes.CheckableListBox" />
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.ListBoxItem" />
    public class CheckableListBoxItem : ListBoxItem
    {
        private static readonly DependencyProperty InternalIsCheckedProperty =
            DependencyProperty.Register("InternalIsChecked",
                typeof(bool?), typeof(CheckableListBoxItem),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse, FrameworkPropertyMetadataOptions.AffectsRender,
                    OnInternalIsCheckedChanged));

        /// <summary>
        /// The is checked property
        /// </summary>
        public static DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool?),
            typeof(CheckableListBoxItem),
            new FrameworkPropertyMetadata(Boxes.BooleanFalse, FrameworkPropertyMetadataOptions.AffectsRender,
                OnIsCheckedChanged));

        /// <summary>
        /// The checked event
        /// </summary>
        public static readonly RoutedEvent CheckedEvent =
            ToggleButton.CheckedEvent.AddOwner(typeof(CheckableListBoxItem));

        /// <summary>
        /// The unchecked event
        /// </summary>
        public static readonly RoutedEvent UncheckedEvent =
            ToggleButton.UncheckedEvent.AddOwner(typeof(CheckableListBoxItem));


        /// <summary>
        /// The is toggle enabled property
        /// </summary>
        public static DependencyProperty IsToggleEnabledProperty = DependencyProperty.Register("IsToggleEnabled",
            typeof(bool), typeof(CheckableListBoxItem), new FrameworkPropertyMetadata(Boxes.BooleanTrue));


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
            get => (bool?)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public bool IsToggleEnabled
        {
            get => (bool)GetValue(IsToggleEnabledProperty);
            set => SetValue(IsToggleEnabledProperty, Boxes.Box(value));
        }

        private bool? InternalIsChecked
        {
            set => SetValue(InternalIsCheckedProperty, value);
        }

        public void OnIsCheckedChanged(DependencyPropertyChangedEventArgs e)
        {
            InternalIsChecked = (bool?)e.NewValue;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                e.Handled = true;
                if (!IsToggleEnabled)
                    return;
                IsChecked = true;
            }
            else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            {
                e.Handled = true;
                if (!IsToggleEnabled)
                    return;
                IsChecked = false;
            }
            else
            {
                var checkableListBox = ItemsControl.ItemsControlFromItemContainer(this) as CheckableListBox;
                if (checkableListBox == null || !checkableListBox.ToggleKeys.Contains(e.Key))
                    return;
                e.Handled = true;
                if (!IsToggleEnabled)
                    return;
                bool? nullable;
                if (!IsChecked.HasValue)
                {
                    nullable = true;
                }
                else
                {
                    var isChecked = IsChecked;
                    nullable = !isChecked.GetValueOrDefault();
                }
                IsChecked = nullable;
            }
        }

        private static void OnInternalIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CheckableListBoxItem)d).OnInternalIsCheckedChanged(e);
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CheckableListBoxItem)d).OnIsCheckedChanged(e);
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
            IsChecked = (bool?)e.NewValue;
        }
    }
}