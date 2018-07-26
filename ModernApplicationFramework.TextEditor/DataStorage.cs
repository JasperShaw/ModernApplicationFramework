using System.Windows;

namespace ModernApplicationFramework.TextEditor
{
    internal class DataStorage : IDataStorage
    {
        public bool TryGetItemValue(string itemKey, out ResourceDictionary itemValue)
        {
            itemValue = default(ResourceDictionary);
            return false;
        }
    }
}