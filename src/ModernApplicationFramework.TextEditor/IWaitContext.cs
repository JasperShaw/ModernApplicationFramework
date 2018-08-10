using System;
using System.Threading;

namespace ModernApplicationFramework.TextEditor
{
    public interface IWaitContext : IDisposable
    {
        CancellationToken CancellationToken { get; }

        bool AllowCancel { get; set; }

        string Message { get; set; }

        void UpdateProgress();
    }
}