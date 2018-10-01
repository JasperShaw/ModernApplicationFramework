using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    internal interface IBraceCompletionStack
    {
        IBraceCompletionSession TopSession { get; }

        void PushSession(IBraceCompletionSession session);

        ReadOnlyObservableCollection<IBraceCompletionSession> Sessions { get; }

        void RemoveOutOfRangeSessions(SnapshotPoint point);

        void Clear();
    }
}
