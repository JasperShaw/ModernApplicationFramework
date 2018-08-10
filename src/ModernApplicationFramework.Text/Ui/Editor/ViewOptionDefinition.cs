using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public abstract class ViewOptionDefinition<T> : EditorOptionDefinition<T>
    {
        public override bool IsApplicableToScope(IPropertyOwner scope)
        {
            return scope is ITextView;
        }
    }
}