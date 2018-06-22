using System;

namespace ModernApplicationFramework.Interfaces.Search
{
    internal interface ISearchMruItemStore
    {
        void AddMruItem(ref Guid category, string text);
        uint GetMruItems(ref Guid categoryGuid, string prefix, uint maxResults, string[] strArray);
        void SetMruItem(ref Guid category, string itemText);
        void DeleteMruItem(ref Guid category, string itemText);
    }
}