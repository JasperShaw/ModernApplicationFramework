using System.Collections.Generic;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IEdtiorGestureScopeProvider
    {
        IEnumerable<GestureScope> GetAssociatedScopes();
    }
}