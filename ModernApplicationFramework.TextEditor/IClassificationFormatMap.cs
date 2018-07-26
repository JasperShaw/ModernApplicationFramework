using System;
using System.Collections.ObjectModel;

namespace ModernApplicationFramework.TextEditor
{
    public interface IClassificationFormatMap
    {
        TextFormattingRunProperties GetTextProperties(IClassificationType classificationType);

        TextFormattingRunProperties GetExplicitTextProperties(IClassificationType classificationType);

        string GetEditorFormatMapKey(IClassificationType classificationType);

        void AddExplicitTextProperties(IClassificationType classificationType, TextFormattingRunProperties properties);

        void AddExplicitTextProperties(IClassificationType classificationType, TextFormattingRunProperties properties, IClassificationType priority);

        void SetTextProperties(IClassificationType classificationType, TextFormattingRunProperties properties);

        void SetExplicitTextProperties(IClassificationType classificationType, TextFormattingRunProperties properties);

        ReadOnlyCollection<IClassificationType> CurrentPriorityOrder { get; }

        TextFormattingRunProperties DefaultTextProperties { get; set; }

        void SwapPriorities(IClassificationType firstType, IClassificationType secondType);

        void BeginBatchUpdate();

        void EndBatchUpdate();

        bool IsInBatchUpdate { get; }

        event EventHandler<EventArgs> ClassificationFormatMappingChanged;
    }
}