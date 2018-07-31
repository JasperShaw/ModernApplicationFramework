using System.ComponentModel.Composition;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IBufferPrimitivesFactoryService))]
    internal sealed class DefaultBufferPrimitivesFactoryService : IBufferPrimitivesFactoryService
    {
        [Import] internal IEditorOptionsFactoryService EditorOptionsFactoryService { get; set; }

        [Import] internal ITextSearchService TextSearchService { get; set; }

        [Import] internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelectorService { get; set; }

        public PrimitiveTextBuffer CreateTextBuffer(ITextBuffer textBuffer)
        {
            if (!textBuffer.Properties.TryGetProperty<PrimitiveTextBuffer>("Editor.TextBuffer", out var property))
            {
                property = new DefaultBufferPrimitive(textBuffer, this);
                textBuffer.Properties.AddProperty("Editor.TextBuffer", property);
            }

            return property;
        }

        public TextPoint CreateTextPoint(PrimitiveTextBuffer textBuffer, int position)
        {
            return new DefaultTextPointPrimitive(textBuffer, position, TextSearchService,
                EditorOptionsFactoryService.GetOptions(textBuffer.AdvancedTextBuffer),
                TextStructureNavigatorSelectorService.GetTextStructureNavigator(textBuffer.AdvancedTextBuffer), this);
        }

        public TextRange CreateTextRange(PrimitiveTextBuffer textBuffer, TextPoint startPoint, TextPoint endPoint)
        {
            return new DefaultTextRangePrimitive(startPoint, endPoint, EditorOptionsFactoryService);
        }
    }
}