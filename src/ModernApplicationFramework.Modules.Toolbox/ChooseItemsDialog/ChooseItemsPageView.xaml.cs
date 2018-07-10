using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    public partial class ChooseItemsPageView
    {
        private long _lastTypeAheadTickCount = int.MaxValue;
        private string _typeAheadString;

        public ChooseItemsPageView()
        {
            InitializeComponent();
            ListView.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnColumnHeaderDividerDragged), true);
        }

        private void ItemKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                ItemDoubleClicked(sender, e);
            else if (e.Key == Key.Add || e.Key == Key.OemPlus)
                CheckSelectedItems(e, true);
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
                CheckSelectedItems(e, false);
            else
            {
                var c = (char) NativeMethods.User32.MapVirtualKey((uint) KeyInterop.VirtualKeyFromKey(e.Key), 2);
                if (!char.IsLetterOrDigit(c) && !char.IsPunctuation(c) && !char.IsSymbol(c))
                    return;
                AddCharToTypeAhead(c);
                e.Handled = true;
            }
        }

        private void AddCharToTypeAhead(char c)
        {
            var tickCount = Environment.TickCount;
            bool skipCurrentItem;

            if (tickCount < _lastTypeAheadTickCount || tickCount - _lastTypeAheadTickCount >= 2000 ||
                _typeAheadString == null)
            {
                _typeAheadString = c.ToString();
                skipCurrentItem = true;
            }
            else if (_typeAheadString.Length == 1 && _typeAheadString[0] == c)
                skipCurrentItem = true;
            else
            {
                _typeAheadString += c.ToString();
                skipCurrentItem = false;
            }
            SelectNextItemStartingWith(_typeAheadString, skipCurrentItem);
            _lastTypeAheadTickCount = tickCount;

        }

        private void SelectNextItemStartingWith(string s, bool skipCurrentItem)
        {
            var flag = true;
            var num = ListView.SelectedIndex;
            if (skipCurrentItem)
                num = (num + 1) % ListView.Items.Count;
            for (var index = num; flag || index != ListView.SelectedIndex; index = (index + 1) % ListView.Items.Count)
            {
                if (((IItemInfo) ListView.Items[index]).Name.StartsWith(s,
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    ListView.SelectedIndex = index;
                    return;
                }
                flag = false;
            }
            if (s.Length != 1)
                return;
            SystemSounds.Beep.Play();
        }

        private void CheckSelectedItems(RoutedEventArgs e, bool shouldBeChecked)
        {
            foreach (IItemInfo selectedItem in ListView.SelectedItems)
                selectedItem.IsChecked = shouldBeChecked;
            e.Handled = true;
        }

        private void ItemDoubleClicked(object sender, RoutedEventArgs e)
        {
            var flag = ((IItemInfo)((ContentControl)sender).Content).IsChecked;
            foreach (IItemInfo selectedItem in ListView.SelectedItems)
                selectedItem.IsChecked = !flag;
            e.Handled = true;
        }

        private void ItemCheckboxClicked(object sender, RoutedEventArgs e)
        {
            var ancestor = ((Visual) sender).FindAncestor<ListViewItem>();
            if (!ancestor.IsSelected)
                return;
            var flag = ((IItemInfo) ancestor.Content).IsChecked;
            foreach (IItemInfo selectedItem in ListView.SelectedItems)
                selectedItem.IsChecked = flag;
        }

        internal void AddColumns(IEnumerable<HeaderInformation> columns)
        {
            foreach (var column in columns)
            {
                GridView.Columns.Add(new GridViewColumn
                {
                    Header = column.Text,
                    Width = double.NaN,
                    DisplayMemberBinding = new Binding(column.Name)
                });
            }
        }

        private static void OnColumnHeaderDividerDragged(object sender, DragDeltaEventArgs e)
        {
            var parent = ((FrameworkElement)e.OriginalSource).TemplatedParent as GridViewColumnHeader;
            if (parent == null || parent.Content != null)
                return;
            parent.Column.Width = double.NaN;
        }
    }
}
