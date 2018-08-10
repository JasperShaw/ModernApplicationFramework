using System;
using System.Collections.Generic;
using System.Threading;
using ModernApplicationFramework.Text.Utilities;
using ModernApplicationFramework.TextEditor.Utilities;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.TextEditor.Implementation
{

    //TODO: Implement from VSUIThreadOperationContext
    internal sealed class UiThreadOperationContext : IUiThreadOperationContext
    {
        public UiThreadOperationContext(string title, string defaultDescription, bool allowCancellation, bool showProgress)
        {
            throw new NotImplementedException();
        }

        public PropertyCollection Properties { get; }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public CancellationToken UserCancellationToken { get; }
        public bool AllowCancellation { get; }
        public string Description { get; }
        public IEnumerable<IUiThreadOperationScope> Scopes { get; }
        public IUiThreadOperationScope AddScope(bool allowCancellation, string description)
        {
            throw new NotImplementedException();
        }

        public void TakeOwnership()
        {
            throw new NotImplementedException();
        }
    }

    //internal sealed class UiThreadOperationContext : AbstractUiThreadOperationContext
    //{
        // TODO: Implement
    //}
}