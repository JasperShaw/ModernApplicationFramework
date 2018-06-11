using System;
using Caliburn.Micro;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxCategory : IToolboxNode
    {
        Type TargetType { get; }

        IObservableCollection<IToolboxItem> Items { get; set; }

        bool HasItems { get; }

        bool HasVisibleItems { get; }

        bool HasEnabledItems { get; }

        void Refresh(Type targetType, bool forceVisibile = false);

        bool RemoveItem(IToolboxItem item);
    }
}