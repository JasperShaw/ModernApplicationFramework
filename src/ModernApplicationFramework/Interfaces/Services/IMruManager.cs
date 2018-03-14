using System.Collections.ObjectModel;
using ModernApplicationFramework.Basics.MostRecentlyUsedManager;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface IMruManager<T> where T : MruItem
    {
        int MaxCount { get; set; }

        int Count { get; }

        ObservableCollection<T> Items { get; }

        void AddItem(string persistenceData);

        void OpenItem(int index);

        void RemoveItemAt(int index);
    }
}