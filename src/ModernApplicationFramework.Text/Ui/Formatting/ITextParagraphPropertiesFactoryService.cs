using System.Windows.Media.TextFormatting;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface ITextParagraphPropertiesFactoryService
    {
        TextParagraphProperties Create(IFormattedLineSource formattedLineSource,
            TextFormattingRunProperties textProperties, IMappingSpan line, IMappingPoint lineStart, int lineSegment);
    }
}