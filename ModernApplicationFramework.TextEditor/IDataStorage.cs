using System.Windows;

namespace ModernApplicationFramework.TextEditor
{
    public interface IDataStorage
    {
        bool TryGetItemValue(string itemKey, out ResourceDictionary itemValue);
    }
}