using System.Collections.Generic;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.TextEditor
{
    public interface IClassificationFormatMetadata : IEditorFormatMetadata, IOrderable
    {
        IEnumerable<string> ClassificationTypeNames { get; }
    }
}