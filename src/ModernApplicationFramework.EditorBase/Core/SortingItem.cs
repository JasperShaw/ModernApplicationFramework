using System;
using System.Collections;

namespace ModernApplicationFramework.EditorBase.Core
{
    public class SortingItem<T> : IComparer
    {

        public delegate int CompareTemplates(T source, T target);

        private readonly CompareTemplates _compareTemplates;

        public Func<T, T, int> Comparer;

        public SortingItem(ListSortDirection direction, Func<T, T, int> sortLogic)
        {
            var sortDir = direction == ListSortDirection.Ascending ? 1 : -1;
            _compareTemplates = (source, target) => sortLogic(source, target) * sortDir;
        }

        public int Compare(object x, object y)
        {
            return _compareTemplates((T) x, (T) y);
        }
    }
}