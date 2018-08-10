using System;
using System.Windows;

namespace ModernApplicationFramework.Text.Ui.Classification
{
    public interface IEditorFormatMap
    {
        event EventHandler<FormatItemsEventArgs> FormatMappingChanged;

        bool IsInBatchUpdate { get; }

        void AddProperties(string key, ResourceDictionary properties);

        void BeginBatchUpdate();

        void EndBatchUpdate();
        ResourceDictionary GetProperties(string key);

        void SetProperties(string key, ResourceDictionary properties);
    }
}