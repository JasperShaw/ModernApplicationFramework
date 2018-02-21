using System;
using ModernApplicationFramework.Core.Events;

namespace ModernApplicationFramework.Interfaces.Controls
{
    public interface IItemDoubleClickable
    {
        event EventHandler<ItemDoubleClickedEventArgs> ItemDoubledClicked;
    }
}
