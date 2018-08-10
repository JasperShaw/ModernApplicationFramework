using System;

namespace ModernApplicationFramework.Text.Data
{
    public interface ITextBufferEdit : IDisposable
    {
        bool Canceled { get; }
        ITextSnapshot Snapshot { get; }

        ITextSnapshot Apply();

        void Cancel();
    }
}