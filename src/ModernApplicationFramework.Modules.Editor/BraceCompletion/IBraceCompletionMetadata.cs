using System.Collections.Generic;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    public interface IBraceCompletionMetadata
    {
        IEnumerable<char> OpeningBraces { get; }

        IEnumerable<char> ClosingBraces { get; }

        IEnumerable<string> ContentTypes { get; }
    }
}