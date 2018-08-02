using System.IO;
using ModernApplicationFramework.TextEditor.Text;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextImageFactoryService
    {
        ITextImage CreateTextImage(string text);

        ITextImage CreateTextImage(TextReader reader, long length = -1);
    }
}