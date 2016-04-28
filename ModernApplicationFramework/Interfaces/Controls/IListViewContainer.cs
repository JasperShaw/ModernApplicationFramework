using System;
using System.Windows.Controls.Primitives;
using ModernApplicationFramework.Core.Events;

namespace ModernApplicationFramework.Interfaces.Controls
{
    public interface IListViewContainer
    {
        event EventHandler<ItemsChangedEventArgs> OnSelectedItemChanged;

        event EventHandler<ItemDoubleClickedEventArgs> ItemDoubledClicked;
    }
}
