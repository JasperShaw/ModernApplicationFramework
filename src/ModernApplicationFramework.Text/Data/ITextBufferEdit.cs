using System;

namespace ModernApplicationFramework.Text.Data
{
    public interface ITextBufferEdit : IDisposable
    {
        ITextSnapshot Snapshot { get; }

        ITextSnapshot Apply();

        void Cancel();

        bool Canceled { get; }
    }
}