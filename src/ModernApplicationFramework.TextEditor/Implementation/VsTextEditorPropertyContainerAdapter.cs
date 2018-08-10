namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal class VsTextEditorPropertyContainerAdapter : ITextEditorPropertyContainer
    {
        private readonly SimpleTextViewWindow _viewAdapter;

        internal VsTextEditorPropertyContainerAdapter(SimpleTextViewWindow viewAdapter)
        {
            _viewAdapter = viewAdapter;
        }

        public int GetProperty(VsEditPropId idProp, out object pvar)
        {
            return _viewAdapter.GetPropertyFromPropertyContainer(idProp, out pvar);
        }

        public int RemoveProperty(VsEditPropId idProp)
        {
            return _viewAdapter.RemovePropertyFromPropertyContainer(idProp);
        }

        public int SetProperty(VsEditPropId idProp, object var)
        {
            return _viewAdapter.SetPropertyInPropertyContainer(idProp, var);
        }
    }
}