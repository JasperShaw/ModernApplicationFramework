using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.TextEditor.Utilities;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(ITextViewCreationListener))]
    [ContentType("Any")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class InputControllerViewCreationListener : ITextViewCreationListener
    {
        [Import]
        internal InputControllerState State;
        [Import]
        internal GuardedOperations GuardedOperations;
        [Import]
        internal PerformanceBlockMarker PerformanceBlockMarker;

        public void TextViewCreated(ITextView textView)
        {
            var inputController = new InputController(textView, this);
        }
    }
}