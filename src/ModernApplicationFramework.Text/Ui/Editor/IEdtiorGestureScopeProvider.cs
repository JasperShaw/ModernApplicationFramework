using System.Collections.Generic;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IEdtiorGestureScopeProvider
    {
        IEnumerable<GestureScope> GetAssociatedScopes();
    }
}