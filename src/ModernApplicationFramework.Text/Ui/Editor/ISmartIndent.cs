using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface ISmartIndent : IDisposable
    {
        int? GetDesiredIndentation(ITextSnapshotLine line);
    }
}