using System;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextEditorFactoryService
    {
        ITextView CreateTextView();

        ITextViewHost CreateTextViewHost(ITextView textView, bool setFocus);

        ITextViewHost CreateTextViewHostWithoutInitialization(ITextView textView, bool setFocus);

        event EventHandler<TextViewCreatedEventArgs> TextViewCreated;
    }
}
