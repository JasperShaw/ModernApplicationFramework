using System;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor.Utilities
{
    public interface IUiThreadOperationScope : IDisposable
    {
        bool AllowCancellation { get; set; }

        string Description { get; set; }

        IUiThreadOperationContext Context { get; }

        IProgress<ProgressInfo> Progress { get; }
    }
}