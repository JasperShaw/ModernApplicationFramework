using System.Collections.Generic;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Text.Ui.Classification
{
    public interface IClassificationFormatMetadata : IEditorFormatMetadata, IOrderable
    {
        IEnumerable<string> ClassificationTypeNames { get; }
    }
}