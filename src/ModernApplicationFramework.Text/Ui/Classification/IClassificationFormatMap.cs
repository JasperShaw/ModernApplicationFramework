using System;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Text.Ui.Classification
{
    public interface IClassificationFormatMap
    {
        event EventHandler<EventArgs> ClassificationFormatMappingChanged;

        ReadOnlyCollection<IClassificationType> CurrentPriorityOrder { get; }

        TextFormattingRunProperties DefaultTextProperties { get; set; }

        bool IsInBatchUpdate { get; }

        void AddExplicitTextProperties(IClassificationType classificationType, TextFormattingRunProperties properties);

        void AddExplicitTextProperties(IClassificationType classificationType, TextFormattingRunProperties properties,
            IClassificationType priority);

        void BeginBatchUpdate();

        void EndBatchUpdate();

        string GetEditorFormatMapKey(IClassificationType classificationType);

        TextFormattingRunProperties GetExplicitTextProperties(IClassificationType classificationType);
        TextFormattingRunProperties GetTextProperties(IClassificationType classificationType);

        void SetExplicitTextProperties(IClassificationType classificationType, TextFormattingRunProperties properties);

        void SetTextProperties(IClassificationType classificationType, TextFormattingRunProperties properties);

        void SwapPriorities(IClassificationType firstType, IClassificationType secondType);
    }
}