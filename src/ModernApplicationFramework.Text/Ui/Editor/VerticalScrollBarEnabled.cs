using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextViewHost/VerticalScrollBar")]
    public sealed class VerticalScrollBarEnabled : ViewOptionDefinition<bool>
    {
        public override bool Default => true;

        public override EditorOptionKey<bool> Key => DefaultTextViewHostOptions.VerticalScrollBarId;
    }
}