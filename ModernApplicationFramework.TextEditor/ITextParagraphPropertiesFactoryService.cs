using System.Windows.Media.TextFormatting;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextParagraphPropertiesFactoryService
    {
        TextParagraphProperties Create(IFormattedLineSource formattedLineSource, TextFormattingRunProperties textProperties, IMappingSpan line, IMappingPoint lineStart, int lineSegment);
    }
}