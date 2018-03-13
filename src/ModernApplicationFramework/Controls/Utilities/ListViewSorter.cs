using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Controls.Utilities
{
    public static class ListViewSorter
    {
        private static readonly Dictionary<ListView, ListViewSortItem> ListViewDefinitions = new Dictionary<ListView, ListViewSortItem>();

        public static readonly DependencyProperty CustomListViewSorterProperty = DependencyProperty.RegisterAttached(
            "CustomListViewSorter",
            typeof(Type),
            typeof(ListViewSorter),
            new FrameworkPropertyMetadata(null, OnRegisterSortableGrid));

        public static readonly DependencyProperty SortExpressionProperty = DependencyProperty.RegisterAttached(
            "SortExpression", typeof(string), typeof(ListViewSorter), new PropertyMetadata(default(string)));

        public static void SetSortExpression(DependencyObject element, string value)
        {
            element.SetValue(SortExpressionProperty, value);
        }

        public static string GetSortExpression(DependencyObject element)
        {
            return (string) element.GetValue(SortExpressionProperty);
        }

        public static Type GetCustomListViewSorter(DependencyObject obj)
        {
            return (Type)obj.GetValue(CustomListViewSorterProperty);
        }

        public static void SetCustomListViewSorter(DependencyObject obj, Type value)
        {
            obj.SetValue(CustomListViewSorterProperty, value);
        }

        public static Tuple<GridViewColumnHeader, ListSortDirection> GetLastSelected(ListView view)
        {
            var listViewSortItem = ListViewDefinitions.ContainsKey(view) ? ListViewDefinitions[view] : null;
            if (listViewSortItem == null)
                return null;
            return new Tuple<GridViewColumnHeader, ListSortDirection>(listViewSortItem.LastColumnHeaderClicked,
                listViewSortItem.LastSortDirection);
        }

        private static void OnRegisterSortableGrid(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)
                return;
            if (d is ListView view)
            {
                ListViewDefinitions.Add(view,
                    new ListViewSortItem(
                        Activator.CreateInstance(GetCustomListViewSorter(d) ?? throw new InvalidOperationException()) as IListViewCustomComparer,
                         null, ListSortDirection.Ascending));

                view.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(GridViewColumnHeaderClickedHandler));
                view.AddHandler(FrameworkElement.UnloadedEvent, new RoutedEventHandler(ListViewUnloaded));
            }
        }

        private static void ListViewUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is ListView lv)
                ListViewDefinitions.Remove(lv);
        }

        public static void ToggleSorting(ListView listView, GridViewColumnHeader header, ListSortDirection direction)
        {
            if (listView == null)
                return;
            var listViewSortItem = ListViewDefinitions.ContainsKey(listView) ? ListViewDefinitions[listView] : null;
            if (listViewSortItem == null)
                return;
            if (header == null)
                return;

            if (!(CollectionViewSource.GetDefaultView(listView.ItemsSource) is ListCollectionView collectionView))
                return;

            // get header name
            var headerBinding = (header.Column.DisplayMemberBinding as Binding)?.Path.Path;
            if (string.IsNullOrEmpty(headerBinding))
            {
                headerBinding = (GetSortExpression(header.Column));
                if (string.IsNullOrEmpty(headerBinding))
                    return;
            }

            // sort listview
            if (listViewSortItem.Comparer != null)
            {
                listViewSortItem.Comparer.SortBy = headerBinding;
                listViewSortItem.Comparer.SortDirection = direction;
                collectionView.CustomSort = listViewSortItem.Comparer;
                listView.Items.Refresh();
            }
            else
            {
                listView.Items.SortDescriptions.Clear();
                listView.Items.Refresh();
            }

            // change datatemplate of previous and current column header
            SetHeader(header as SortingGridViewColumnHeader, listViewSortItem, direction);

            // Set current sort values as last sort values
            listViewSortItem.LastColumnHeaderClicked = header;
            listViewSortItem.LastSortDirection = direction;

        }

        private static void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            if (!(sender is ListView view))
                return;
            if (!(e.OriginalSource is GridViewColumnHeader headerClicked))
                return;
            var listViewSortItem = ListViewDefinitions.ContainsKey(view) ? ListViewDefinitions[view] : null;
            if (listViewSortItem == null)
                return;
            ToggleSorting(view, headerClicked, GetSortingDirection(headerClicked, listViewSortItem));
        }

        private static ListSortDirection GetSortingDirection(GridViewColumnHeader headerClicked, ListViewSortItem listViewSortItem)
        {
            if (!Equals(headerClicked, listViewSortItem.LastColumnHeaderClicked))
                return ListSortDirection.Ascending;
            return listViewSortItem.LastSortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
        }

        private static void SetHeader(SortingGridViewColumnHeader newHeader,ListViewSortItem listViewSortItem, ListSortDirection sortDirection)
        {
            if (listViewSortItem.LastColumnHeaderClicked != null &&
                listViewSortItem.LastColumnHeaderClicked is SortingGridViewColumnHeader sortingHeader)
                sortingHeader.IsSelected = false;
            if (newHeader == null)
                return;
            newHeader.IsSelected = true;
            newHeader.ListSortDirection = sortDirection;
        }

        private class ListViewSortItem
        {
            public IListViewCustomComparer Comparer { get; }

            public GridViewColumnHeader LastColumnHeaderClicked { get; set; }

            public ListSortDirection LastSortDirection { get; set; }

            public ListViewSortItem(IListViewCustomComparer comparer, GridViewColumnHeader lastColumnHeaderClicked, ListSortDirection lastSortDirection)
            {
                Comparer = comparer;
                LastColumnHeaderClicked = lastColumnHeaderClicked;
                LastSortDirection = lastSortDirection;
            }
        }
    }
}
