using System.ComponentModel.Composition;
using ModernApplicationFramework.Threading;

namespace ModernApplicationFramework.Basics.Threading
{
    internal class JoinableTaskContextMefHost
    {
        [Export]
        internal JoinableTaskContext JoinableTaskContext => ThreadHelper.JoinableTaskContext;
    }
}
