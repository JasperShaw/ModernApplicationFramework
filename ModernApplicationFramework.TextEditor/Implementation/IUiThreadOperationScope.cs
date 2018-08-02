using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface IUiThreadOperationScope : IDisposable
    {
        bool AllowCancellation { get; set; }

        string Description { get; set; }

        IUiThreadOperationContext Context { get; }

        IProgress<ProgressInfo> Progress { get; }
    }
}