using System;
using System.ComponentModel;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Controls.Utilities
{
    public abstract class SortingItem<T> : IListViewCustomComparer
    {

        public delegate int CompareTemplates(T source, T target);

        public Func<T, T, int> Comparer;

        public int Compare(object x, object y)
        {

            if (!(x is T item1) || !(y is T item2))
                return 0;

            return Compare(item1, item2) * (SortDirection == ListSortDirection.Ascending ? 1 : -1);
        }

        public abstract int Compare(T x, T y);

        public string SortBy { get;  set; }

        public ListSortDirection SortDirection { get; set; }
    }
}