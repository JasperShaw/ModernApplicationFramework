using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.InfoBar.Internal
{
    internal class InfoBarTextViewModel
    {
        internal InfoBarTextViewModel(InfoBarViewModel owner, IInfoBarTextSpan textSpan)
        {
            Validate.IsNotNull(textSpan, nameof(textSpan));
            TextSpan = textSpan;
            Owner = owner;
        }

        public IInfoBarTextSpan TextSpan { get; }

        public InfoBarViewModel Owner { get; }

        public bool Bold => TextSpan.Bold;

        public bool Italic => TextSpan.Italic;

        public string Text => TextSpan.Text;

        public bool Underline => TextSpan.Underline;
    }
}
