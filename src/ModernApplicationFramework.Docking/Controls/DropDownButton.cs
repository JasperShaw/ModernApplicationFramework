using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using ModernApplicationFramework.Controls.Buttons;
using ModernApplicationFramework.Interfaces;
using ContextMenu = System.Windows.Controls.ContextMenu;

namespace ModernApplicationFramework.Docking.Controls
{
    internal class DropDownButton : GlyphButton
    {
        public static readonly DependencyProperty DropDownContextMenuProperty =
            DependencyProperty.Register("DropDownContextMenu", typeof (ContextMenu), typeof (DropDownButton),
                new FrameworkPropertyMetadata(null,
                    OnDropDownContextMenuChanged));

        public static readonly DependencyProperty ContextMenuProviderProperty = DependencyProperty.Register(
            "ContextMenuProvider", typeof(IContextMenuProvider), typeof(DropDownButton), new PropertyMetadata(default(IValueConverter)));


        public DropDownButton()
        {
            Click += OnClick;
            ContextMenuOpening += OnContextMenuOpening;
            SetResourceReference(StyleProperty, typeof(GlyphButton));
        }

        private void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ContextMenu.IsOpen = false;
            e.Handled = true;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            ShowDropDown();
        }

        private void ShowDropDown()
        {
            if (DropDownContextMenu != null)
                ContextMenu = DropDownContextMenu;
            else
            {
                if (ContextMenuProvider == null)
                    return;
                ContextMenu = ContextMenuProvider.Provide(DataContext);
            }
            if (ContextMenu == null || ContextMenu.Items.Count == 0 && (ContextMenu.TemplatedParent == null || !(ContextMenu.TemplatedParent as ItemsControl).HasItems))
                return;
            (ContextMenu.ItemsSource as CollectionView)?.Refresh();
            ContextMenu.Placement = PlacementMode.Bottom;
            ContextMenu.PlacementTarget = this;
            ContextMenu.IsOpen = true;
        }

        public ContextMenu DropDownContextMenu
        {
            get => (ContextMenu) GetValue(DropDownContextMenuProperty);
            set => SetValue(DropDownContextMenuProperty, value);
        }

        public IContextMenuProvider ContextMenuProvider
        {
            get => (IContextMenuProvider)GetValue(ContextMenuProviderProperty);
            set => SetValue(ContextMenuProviderProperty, value);
        }

        protected virtual void OnDropDownContextMenuChanged(DependencyPropertyChangedEventArgs e)
        {
            ContextMenu = DropDownContextMenu;
        }

        private static void OnDropDownContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DropDownButton) d).OnDropDownContextMenuChanged(e);
        }
    }
}