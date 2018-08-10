using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Data
{
    public interface INormalizedTextChangeCollection : IList<ITextChange>
    {
        bool IncludesLineChanges { get; }
    }
}