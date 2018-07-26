using System;
using System.Windows;

namespace ModernApplicationFramework.TextEditor
{
    public interface IEditorFormatMap
    {
        ResourceDictionary GetProperties(string key);

        void AddProperties(string key, ResourceDictionary properties);

        void SetProperties(string key, ResourceDictionary properties);

        void BeginBatchUpdate();

        void EndBatchUpdate();

        bool IsInBatchUpdate { get; }

        event EventHandler<FormatItemsEventArgs> FormatMappingChanged;
    }
}