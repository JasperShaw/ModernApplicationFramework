using System.Collections;
using System.ComponentModel;

namespace ModernApplicationFramework.Interfaces.Controls
{
    public interface IListViewCustomComparer : IComparer
    {
        ListSortDirection SortDirection { get; set; }

        string SortBy { get; set; }
    }
}