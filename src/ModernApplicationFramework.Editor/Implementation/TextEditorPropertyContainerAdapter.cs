using ModernApplicationFramework.Editor.TextManager;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class TextEditorPropertyContainerAdapter : ITextEditorPropertyContainer
    {
        private readonly SimpleTextViewWindow _viewAdapter;

        internal TextEditorPropertyContainerAdapter(SimpleTextViewWindow viewAdapter)
        {
            _viewAdapter = viewAdapter;
        }

        public int GetProperty(EditPropId idProp, out object pvar)
        {
            return _viewAdapter.GetPropertyFromPropertyContainer(idProp, out pvar);
        }

        public int RemoveProperty(EditPropId idProp)
        {
            return _viewAdapter.RemovePropertyFromPropertyContainer(idProp);
        }

        public int SetProperty(EditPropId idProp, object var)
        {
            return _viewAdapter.SetPropertyInPropertyContainer(idProp, var);
        }
    }
}