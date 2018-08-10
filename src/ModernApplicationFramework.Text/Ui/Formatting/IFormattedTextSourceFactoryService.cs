using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Ui.Classification;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface IFormattedTextSourceFactoryService
    {
        IFormattedLineSource Create(ITextSnapshot sourceTextSnapshot, ITextSnapshot visualBufferSnapshot, int tabSize,
            double baseIndent, double wordWrapWidth, double maxAutoIndent, bool useDisplayMode,
            IClassifier aggregateClassifier, ITextAndAdornmentSequencer sequencer,
            IClassificationFormatMap classificationFormatMap);

        IFormattedLineSource Create(ITextSnapshot sourceTextSnapshot, ITextSnapshot visualBufferSnapshot, int tabSize,
            double baseIndent, double wordWrapWidth, double maxAutoIndent, bool useDisplayMode,
            IClassifier aggregateClassifier, ITextAndAdornmentSequencer sequencer,
            IClassificationFormatMap classificationFormatMap, bool isViewWrapEnabled);

        IFormattedLineSource Create(ITextSnapshot sourceTextSnapshot, ITextSnapshot visualBufferSnapshot, int tabSize,
            double baseIndent, double wordWrapWidth, double maxAutoIndent, bool useDisplayMode,
            ITextAndAdornmentSequencer sequencer, IClassificationFormatMap classificationFormatMap);
    }
}