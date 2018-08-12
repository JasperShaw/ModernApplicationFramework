using System.IO;

namespace ModernApplicationFramework.Text.Data
{
    public interface ITextImageFactoryService
    {
        ITextImage CreateTextImage(string text);

        ITextImage CreateTextImage(TextReader reader, long length = -1);
    }
}