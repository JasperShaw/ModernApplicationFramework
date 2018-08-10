using System;

namespace ModernApplicationFramework.Text.Utilities
{
    public interface IUiThreadOperationScope : IDisposable
    {
        bool AllowCancellation { get; set; }

        IUiThreadOperationContext Context { get; }

        string Description { get; set; }

        IProgress<ProgressInfo> Progress { get; }
    }
}