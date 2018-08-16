using System;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public static class TextViewExtensions
    {
        public static bool IsEmbeddedTextView(this ITextView textView)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            return textView.Roles.Contains("EMBEDDED_PEEK_TEXT_VIEW");
        }

        public static bool TryGetContainingTextView(this ITextView textView, out ITextView containingTextView)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            if (textView.IsEmbeddedTextView())
            {
                if (!textView.Properties.TryGetProperty("PeekContainingTextView", out containingTextView) || containingTextView == null)
                    throw new InvalidOperationException("Unexpected failure to obtain containing text view of an embedded text view.");
                return true;
            }
            containingTextView = null;
            return false;
        }

        public static bool GetInOuterLayout(this ITextView textView)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            return (textView).InOuterLayout;
        }

        public static IMultiSelectionBroker GetMultiSelectionBroker(this ITextView textView)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            return textView.MultiSelectionBroker;
        }
    }
}
