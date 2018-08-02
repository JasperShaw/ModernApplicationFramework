using System;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextEditorFactoryService
    {
        ITextViewRoleSet NoRoles { get; }

        ITextViewRoleSet AllPredefinedRoles { get; }

        ITextViewRoleSet DefaultRoles { get; }

        ITextView CreateTextView();

        ITextViewHost CreateTextViewHost(ITextView textView, bool setFocus);

        ITextViewHost CreateTextViewHostWithoutInitialization(ITextView textView, bool setFocus);

        ITextView CreateTextViewWithoutInitialization(ITextDataModel dataModel, ITextViewRoleSet roles, IEditorOptions parentOptions);

        void InitializeTextView(ITextView view);

        void InitializeTextViewHost(ITextViewHost host);

        event EventHandler<TextViewCreatedEventArgs> TextViewCreated;
    }
}
