using System.Collections;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextVersion
    {
        ITextVersion Next { get; }

        INormalizedTextChangeCollection Changes { get; }

        int Length { get; }

        ITextBuffer TextBuffer { get; }

        int VersionNumber { get; }

        int ReiteratedVersionNumber { get; }
    }

    public interface INormalizedTextChangeCollection : IList<ITextChange>
    {
        bool IncludesLineChanges { get; }
    }
}