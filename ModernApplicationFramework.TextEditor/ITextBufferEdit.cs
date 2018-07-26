using System;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextBufferEdit : IDisposable
    {
        ITextSnapshot Snapshot { get; }

        ITextSnapshot Apply();

        void Cancel();

        bool Canceled { get; }
    }
}