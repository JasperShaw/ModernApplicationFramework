using System;
using System.Collections.Generic;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextEditorFactoryService
    {
        ITextViewRoleSet NoRoles { get; }

        ITextViewRoleSet AllPredefinedRoles { get; }

        ITextViewRoleSet DefaultRoles { get; }

        ITextViewRoleSet CreateTextViewRoleSet(IEnumerable<string> roles);

        ITextViewRoleSet CreateTextViewRoleSet(params string[] roles);

        ITextView CreateTextView();

        ITextView CreateTextView(ITextViewModel viewModel, ITextViewRoleSet roles, IEditorOptions parentOptions);

        ITextViewHost CreateTextViewHost(ITextView textView, bool setFocus);

        ITextViewHost CreateTextViewHostWithoutInitialization(ITextView textView, bool setFocus);

        ITextView CreateTextViewWithoutInitialization(ITextDataModel dataModel, ITextViewRoleSet roles, IEditorOptions parentOptions);

        void InitializeTextView(ITextView view);

        void InitializeTextViewHost(ITextViewHost host);

        event EventHandler<TextViewCreatedEventArgs> TextViewCreated;
    }
}
