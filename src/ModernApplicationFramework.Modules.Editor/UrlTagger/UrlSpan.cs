using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.UrlTagger
{
    internal class UrlSpan
    {
        internal Span Url { get; set; }

        internal Span Protocol { get; set; }

        internal Span Address { get; set; }
    }
}