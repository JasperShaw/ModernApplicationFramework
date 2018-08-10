using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class ViewPrimitives : IViewPrimitives
    {
        internal ViewPrimitives(ITextView textView, IViewPrimitivesFactoryService viewPrimitivesFactory)
        {
            View = viewPrimitivesFactory.CreateTextView(textView);
            Buffer = View.TextBuffer;
            Selection = View.Selection;
            Caret = View.Caret;
        }

        public PrimitiveTextView View { get; }

        public Selection Selection { get; }

        public Caret Caret { get; }

        public PrimitiveTextBuffer Buffer { get; }
    }
}