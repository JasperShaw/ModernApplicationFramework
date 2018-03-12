using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.EditorBase.Controls.WindowSelectionDialog
{
    public partial class WindowSelectionDialogView
    {
        private static ListSortDirection _lastDirection;
        private static int? _lastSelectedColumn;


        public WindowSelectionDialogView()
        {
            InitializeComponent();
            PreviewKeyDown += WindowSelectionDialogView_PreviewKeyDown;
            Loaded += WindowSelectionDialogView_Loaded;
            Closing += WindowSelectionDialogView_Closing;
        }

        private void WindowSelectionDialogView_Loaded(object sender, RoutedEventArgs e)
        {
            ListView.Focus();
            if (!(ListView.View is GridView g))
                return;
            if (_lastSelectedColumn == null)
                return;

            var i = g.Columns.ElementAtOrDefault(_lastSelectedColumn.Value)?.Header as GridViewColumnHeader;
            ListViewSorter.ToggleSorting(ListView, i, _lastDirection); 
        }

        private void WindowSelectionDialogView_Closing(object sender, CancelEventArgs e)
        {
            var t = ListViewSorter.GetLastSelected(ListView);
            if (t.Item1 == null)
                return;
            if (!(ListView.View is GridView g))
                return;
            var pair = g.Columns.Select((value, index) => new { Value = value, Index = index })
                .Single(p => Equals(p.Value.Header, t.Item1));
            _lastSelectedColumn = pair.Index;
            _lastDirection = t.Item2;
        }


        private void WindowSelectionDialogView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
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
