namespace ModernApplicationFramework.TextEditor
{
    public abstract class ViewOptionDefinition<T> : EditorOptionDefinition<T>
    {
        public override bool IsApplicableToScope(IPropertyOwner scope)
        {
            return scope is ITextView;
        }
    }
}