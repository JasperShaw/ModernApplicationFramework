using System.Collections.ObjectModel;
using ModernApplicationFramework.Basics.MostRecentlyUsedManager;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface IMruManager<T> where T : IMruItem
    {
        int MaxCount { get; set; }

        int Count { get; }

        ObservableCollection<T> Items { get; }

        void AddItem(object persistenceData);

        void OpenItem(int index);

        void RemoveItemAt(int index);
    }
}