using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.EditorPrimitives
{
    internal sealed class ViewPrimitives : IViewPrimitives
    {
        public PrimitiveTextBuffer Buffer { get; }

        public Caret Caret { get; }

        public Selection Selection { get; }

        public PrimitiveTextView View { get; }

        internal ViewPrimitives(ITextView textView, IViewPrimitivesFactoryService viewPrimitivesFactory)
        {
            View = viewPrimitivesFactory.CreateTextView(textView);
            Buffer = View.TextBuffer;
            Selection = View.Selection;
            Caret = View.Caret;
        }
    }
}