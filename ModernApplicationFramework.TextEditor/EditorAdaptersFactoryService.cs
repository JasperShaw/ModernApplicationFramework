using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IEditorAdaptersFactoryService))]
    internal class EditorAdaptersFactoryService : IEditorAdaptersFactoryService
    {
        public static IMafTextView GetVsTextViewFromTextView(ITextView textView)
        {
            if (textView == null)
                throw new ArgumentNullException(nameof(textView));
            if (!textView.Properties.TryGetProperty(typeof(IMafTextView), out IMafTextView property))
                return null;
            return property;
        }

        public static SimpleTextViewWindow GetSimpleTextViewWindowFromTextView(ITextView textView)
        {
            return GetVsTextViewFromTextView(textView) as SimpleTextViewWindow;
        }
    }
}