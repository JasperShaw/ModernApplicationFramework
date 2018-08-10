using System.Windows;

namespace ModernApplicationFramework.Text.Storage
{
    public interface IDataStorage
    {
        bool TryGetItemValue(string itemKey, out ResourceDictionary itemValue);
    }
}