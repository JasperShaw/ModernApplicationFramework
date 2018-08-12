using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    [Export(typeof(ITextViewCreationListener))]
    [ContentType("Any")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class InputControllerViewCreationListener : ITextViewCreationListener
    {
        [Import] internal GuardedOperations GuardedOperations;

        [Import] internal PerformanceBlockMarker PerformanceBlockMarker;

        [Import] internal InputControllerState State;

        public void TextViewCreated(ITextView textView)
        {
            var inputController = new InputController(textView, this);
        }
    }
}