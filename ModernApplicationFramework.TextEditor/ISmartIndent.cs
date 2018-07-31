using System;
using ModernApplicationFramework.TextEditor.Text;

namespace ModernApplicationFramework.TextEditor
{
    public interface ISmartIndent : IDisposable
    {
        int? GetDesiredIndentation(ITextSnapshotLine line);
    }
}