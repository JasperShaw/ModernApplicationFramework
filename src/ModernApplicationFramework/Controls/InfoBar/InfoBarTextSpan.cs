using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.InfoBar
{
    public class InfoBarTextSpan : IInfoBarTextSpan
    {
        public InfoBarTextSpan(string text, bool bold = false, bool italic = false, bool underline = false)
        {
            Validate.IsNotNullAndNotEmpty(text, nameof(text));
            Text = text;
            Bold = bold;
            Italic = italic;
            Underline = underline;
        }

        public string Text { get; }
        public bool Bold { get; }
        public bool Italic { get; }
        public bool Underline { get; }
    }
}
