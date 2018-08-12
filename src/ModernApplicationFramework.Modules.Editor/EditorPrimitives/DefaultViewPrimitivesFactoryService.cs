using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.EditorPrimitives
{
    [Export(typeof(IViewPrimitivesFactoryService))]
    internal sealed class DefaultViewPrimitivesFactoryService : IViewPrimitivesFactoryService
    {
        [Import] internal IBufferPrimitivesFactoryService BufferPrimitivesFactoryService { get; set; }

        [Import] internal IEditorOptionsFactoryService EditorOptionsFactoryService { get; set; }

        public Caret CreateCaret(PrimitiveTextView textView)
        {
            if (textView.Caret == null)
                return new DefaultCaretPrimitive(textView,
                    EditorOptionsFactoryService.GetOptions(textView.AdvancedTextView));
            return textView.Caret;
        }

        public DisplayTextPoint CreateDisplayTextPoint(PrimitiveTextView textView, int position)
        {
            return new DefaultDisplayTextPointPrimitive(textView, position,
                EditorOptionsFactoryService.GetOptions(textView.AdvancedTextView));
        }

        public DisplayTextRange CreateDisplayTextRange(PrimitiveTextView textView, TextRange textRange)
        {
            return new DefaultDisplayTextRangePrimitive(textView, textRange);
        }

        public Selection CreateSelection(PrimitiveTextView textView)
        {
            if (textView.Selection == null)
                return new DefaultSelectionPrimitive(textView,
                    EditorOptionsFactoryService.GetOptions(textView.AdvancedTextView));
            return textView.Selection;
        }

        public PrimitiveTextView CreateTextView(ITextView textView)
        {
            if (!textView.Properties.TryGetProperty("Editor.View", out PrimitiveTextView property))
            {
                property = new DefaultTextViewPrimitive(textView, this, BufferPrimitivesFactoryService);
                textView.Properties.AddProperty("Editor.View", property);
            }

            return property;
        }
    }
}