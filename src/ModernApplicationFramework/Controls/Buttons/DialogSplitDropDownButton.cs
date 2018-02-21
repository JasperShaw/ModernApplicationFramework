using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls.Buttons
{
    public class DialogSplitDropDownButton : System.Windows.Controls.Button
    {
        private bool _contentLoaded;

        public static readonly DependencyProperty DropDownMenuProperty = DependencyProperty.Register(nameof(DropDownMenu), typeof(ContextMenu), typeof(DialogSplitDropDownButton));
        public static readonly DependencyProperty DropDownButtonVisibleProperty = DependencyProperty.Register(nameof(IsDropDownButtonVisible), typeof(bool), typeof(DialogSplitDropDownButton), new PropertyMetadata(true));

        public ContextMenu DropDownMenu
        {
            get => (ContextMenu)GetValue(DropDownMenuProperty);
            set => SetValue(DropDownMenuProperty, value);
        }

        public bool IsDropDownButtonVisible
        {
            get => (bool)GetValue(DropDownButtonVisibleProperty);
            set => SetValue(DropDownButtonVisibleProperty, value);
        }

        public DialogSplitDropDownButton()
        {
            InitializeComponent();
            Click += DialogSplitDropDownButton_Click;
        }

        public void Connect(int connectionId, object target)
        {
            _contentLoaded = true;
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;
            _contentLoaded = true;
            Application.LoadComponent(this,
                new Uri("/ModernApplicationFramework;component/Themes/Generic/DialogSplitDropDownButton.xaml", UriKind.Relative));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsDropDownButtonVisible)
            {
                var position = e.GetPosition(this);
                var visualChild = FindVisualChild<ContentPresenter>(this);
                if (visualChild != null)
                {
                    if (visualChild.ContentTemplate.FindName("SplitLine", visualChild) is FrameworkElement name && position.X >= name.TranslatePoint(new Point(0.0, 0.0), this).X)
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Down && IsDropDownButtonVisible)
            {
                OpenDropDown();
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }

        private void OpenDropDown()
        {
            DropDownMenu.PlacementTarget = this;
            DropDownMenu.Placement = PlacementMode.Bottom;
            DropDownMenu.IsOpen = true;
        }

        private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (var childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(obj); ++childIndex)
            {
                var child = VisualTreeHelper.GetChild(obj, childIndex);
                if (child is T variable)
                    return variable;
                var visualChild = FindVisualChild<T>(child);
                if (visualChild != null)
                    return visualChild;
            }
            return default;
        }

        private void DialogSplitDropDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDropDownButtonVisible)
                return;
            OpenDropDown();
            e.Handled = true;
        }
    }
}
