using System.IO;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.TextEditor.Text
{
    public static class TextImageExtensions
    {
        public static string GetText(this ITextImage image, int startIndex, int length)
        {
            return image.GetText(new Span(startIndex, length));
        }

        public static string GetText(this ITextImage image)
        {
            return image.GetText(new Span(0, image.Length));
        }

        public static ITextImage GetSubText(this ITextImage image, int startIndex, int length)
        {
            return image.GetSubText(new Span(startIndex, length));
        }

        public static void Write(this ITextImage image, TextWriter writer)
        {
            image.Write(writer, new Span(0, image.Length));
        }
    }
}