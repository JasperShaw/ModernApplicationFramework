using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Operations;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    [Export(typeof(ITextViewMarginProvider))]
    [Name("LeftSelection")]
    [MarginContainer("Left")]
    [ContentType("text")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class LeftSelectionMarginProvider : ITextViewMarginProvider
    {
        [Import] private TextViewMarginState _marginState;

        [Import] internal IEditorOperationsFactoryService EditorOperationsFactoryService { get; set; }

        [Import] internal GuardedOperations GuardedOperations { get; set; }

        public ITextViewMargin CreateMargin(ITextViewHost textViewHost, ITextViewMargin containerMargin)
        {
            return LeftSelectionMargin.Create(textViewHost,
                EditorOperationsFactoryService.GetEditorOperations(textViewHost.TextView), GuardedOperations,
                _marginState);
        }
    }
}