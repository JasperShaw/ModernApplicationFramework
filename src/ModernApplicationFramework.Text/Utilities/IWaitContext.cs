using System;
using System.Threading;

namespace ModernApplicationFramework.Text.Utilities
{
    public interface IWaitContext : IDisposable
    {
        CancellationToken CancellationToken { get; }

        bool AllowCancel { get; set; }

        string Message { get; set; }

        void UpdateProgress();
    }
}