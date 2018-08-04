using System;
using System.Collections.Generic;
using System.Threading;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor.Utilities
{
    public interface IUiThreadOperationContext : IPropertyOwner, IDisposable
    {
        CancellationToken UserCancellationToken { get; }

        bool AllowCancellation { get; }

        string Description { get; }

        IEnumerable<IUiThreadOperationScope> Scopes { get; }

        IUiThreadOperationScope AddScope(bool allowCancellation, string description);

        void TakeOwnership();
    }
}