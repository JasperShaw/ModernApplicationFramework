using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.EditorBase.Dialogs.WindowSelectionDialog
{
    public partial class WindowSelectionDialogView : IRestorableGridColumnControl
    {
        private bool _firstInitCompleted;
        private bool _listViewLoaded;

        private Action _defferedLoad;

        public WindowSelectionDialogView()
        {
            InitializeComponent();
            PreviewKeyDown += WindowSelectionDialogView_PreviewKeyDown;
            Loaded += WindowSelectionDialogView_Loaded;
            ListView.SelectionChanged += ListView_SelectionChanged;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView.SelectionChanged -= ListView_SelectionChanged;
            _defferedLoad?.Invoke();
            _listViewLoaded = true;
        }

        private void WindowSelectionDialogView_Loaded(object sender, RoutedEventArgs e)
        {
            ListView.Focus();
            if (!(ListView.View is GridView g))
                return;

            var i = g.Columns.ElementAtOrDefault(0)?.Header as GridViewColumnHeader;
            ListViewSorter.ToggleSorting(ListView, i, ListSortDirection.Ascending);
        }

        private void WindowSelectionDialogView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
            if (_firstInitCompleted)
                return;
            var selectedElement = (UIElement)ListView.ItemContainerGenerator.ContainerFromItem(ListView.SelectedItem);
            selectedElement?.Focus();
            _firstInitCompleted = true;
        }

        private void ListView_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeColumn();
        }

        private void ListView_OnLoaded(object sender, RoutedEventArgs e)
        {
            ResizeColumn();
        }

        private void ResizeColumn()
        {
            if (!(ListView.View is GridView gridView) || gridView.Columns.Count != 2)
                return;
            var width = ListView.ActualWidth - gridView.Columns[0].ActualWidth - 12.0;
            if (width < 0)
                width = 0;
            gridView.Columns[1].Width = width;
        }

        public void Reset(int? index, ListSortDirection direction)
        {
            if (!(ListView.View is GridView g))
                return;
            if (index == null)
                return;
            var i = g.Columns.ElementAtOrDefault(index.Value)?.Header as GridViewColumnHeader;
            if (_listViewLoaded)
                ListViewSorter.ToggleSorting(ListView, i, direction);
            else
                _defferedLoad = () => ListViewSorter.ToggleSorting(ListView, i, direction);
        }

        public Tuple<int?, ListSortDirection> Save()
        {
            var tuple = new Tuple<int?, ListSortDirection>(null, ListSortDirection.Ascending);
            var t = ListViewSorter.GetLastSelected(ListView);
            if (t.Item1 == null)
                return tuple;
            if (!(ListView.View is GridView g))
                return tuple;
            var pair = g.Columns.Select((value, index) => new { Value = value, Index = index })
                .Single(p => Equals(p.Value.Header, t.Item1));
            return new Tuple<int?, ListSortDirection>(pair.Index, t.Item2);
        }
    }

    internal interface IRestorableGridColumnControl
    {
        void Reset(int? index, ListSortDirection description);

        Tuple<int?, ListSortDirection> Save();
    }

    internal class LayoutItemComparer : SortingItem<ILayoutItemBase>
    {
        public static readonly Func<ILayoutItemBase, ILayoutItemBase, int> NameCompare = (s, t) =>
            string.Compare(s.DisplayName, t.DisplayName, StringComparison.CurrentCulture);

        public static readonly Func<IEditor, IEditor, int> PathComare = (s, t) =>
            string.Compare(s.Document.Path, t.Document.Path, StringComparison.CurrentCulture);

        public override int Compare(ILayoutItemBase x, ILayoutItemBase y)
        {
            switch (SortBy)
            {
                case "Document.Path":
                    var editorX = x as IEditor;
                    var editorY = y as IEditor;

                    if (editorX != null && editorY != null)
                        return PathComare(editorX, editorY);
                    if (editorX == null)
                    {
                        if (editorY == null)
                            return 0;
                        return string.Compare(string.Empty, editorY.Document.Path,
                            StringComparison.CurrentCultureIgnoreCase);
                    }
                    return string.Compare(string.Empty, editorX.Document.Path,
                        StringComparison.CurrentCultureIgnoreCase);
                default:
                    return NameCompare(x, y);
            }
        }
    }
}
