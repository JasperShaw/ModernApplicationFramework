using System.ComponentModel.Composition;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Editor.KeyGestureScope
{
    [Export(typeof(ITextViewCreationListener))]
    [ContentType("Any")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class GestureScopeCreationListener : ITextViewCreationListener
    {
        [Import] internal KeyGestureScopeState State;

        [Import] internal IKeyGestureService GestureService;

        public void TextViewCreated(ITextView textView)
        {
            new GestureScopeManager(textView, this);
        }
    }
}