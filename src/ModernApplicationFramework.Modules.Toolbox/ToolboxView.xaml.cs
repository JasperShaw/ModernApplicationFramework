using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public partial class ToolboxView
    {
        private bool _draggingItem;
        private Point _mouseStartPosition;

        public ToolboxView()
        {
            InitializeComponent();
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var listBoxItem = e.GetOriginalSource<ListBoxItem>();
            _draggingItem = listBoxItem != null;

            _mouseStartPosition = e.GetPosition(ListBox);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_draggingItem)
                return;
            var mousePosition = e.GetPosition(null);
            var diff = _mouseStartPosition - mousePosition;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var dpo = e.OriginalSource as Visual;
                var listBoxItem = dpo?.FindAncestorOrSelf<ListBoxItem>();

                if (listBoxItem == null)
                    return;

                var itemViewModel = (ToolboxItemViewModel)ListBox.ItemContainerGenerator.
                    ItemFromContainer(listBoxItem);

                var dragData = new DataObject(ToolboxDragDrop.DataFormat, itemViewModel.Model);
                DragDrop.DoDragDrop(listBoxItem, dragData, DragDropEffects.Move);
            }
        }
    }
}
