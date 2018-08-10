using System;
using System.Collections.Generic;
using System.Threading;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Utilities
{
    public interface IUiThreadOperationContext : IPropertyOwner, IDisposable
    {
        bool AllowCancellation { get; }

        string Description { get; }

        IEnumerable<IUiThreadOperationScope> Scopes { get; }
        CancellationToken UserCancellationToken { get; }

        IUiThreadOperationScope AddScope(bool allowCancellation, string description);

        void TakeOwnership();
    }
}