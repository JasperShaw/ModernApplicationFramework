using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface INormalizedTextChangeCollection : IList<ITextChange>
    {
        bool IncludesLineChanges { get; }
    }
}