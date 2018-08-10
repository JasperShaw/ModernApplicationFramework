using System.IO;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextImageFactoryService
    {
        ITextImage CreateTextImage(string text);

        ITextImage CreateTextImage(TextReader reader, long length = -1);
    }
}