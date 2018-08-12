using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.UrlTagger
{
    internal class UrlSpan
    {
        internal Span Address { get; set; }

        internal Span Protocol { get; set; }
        internal Span Url { get; set; }
    }
}