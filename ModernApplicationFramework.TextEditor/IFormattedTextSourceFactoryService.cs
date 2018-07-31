namespace ModernApplicationFramework.TextEditor
{
    public interface IFormattedTextSourceFactoryService
    {
        IFormattedLineSource Create(ITextSnapshot sourceTextSnapshot, ITextSnapshot visualBufferSnapshot, int tabSize, double baseIndent, double wordWrapWidth, double maxAutoIndent, bool useDisplayMode, IClassifier aggregateClassifier, ITextAndAdornmentSequencer sequencer, IClassificationFormatMap classificationFormatMap);

        IFormattedLineSource Create(ITextSnapshot sourceTextSnapshot, ITextSnapshot visualBufferSnapshot, int tabSize, double baseIndent, double wordWrapWidth, double maxAutoIndent, bool useDisplayMode, IClassifier aggregateClassifier, ITextAndAdornmentSequencer sequencer, IClassificationFormatMap classificationFormatMap, bool isViewWrapEnabled);

        IFormattedLineSource Create(ITextSnapshot sourceTextSnapshot, ITextSnapshot visualBufferSnapshot, int tabSize, double baseIndent, double wordWrapWidth, double maxAutoIndent, bool useDisplayMode, ITextAndAdornmentSequencer sequencer, IClassificationFormatMap classificationFormatMap);
    }
}