using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextViewHost/VerticalScrollBar")]
    public sealed class VerticalScrollBarEnabled : ViewOptionDefinition<bool>
    {
        public override bool Default => true;

        public override EditorOptionKey<bool> Key => DefaultTextViewHostOptions.VerticalScrollBarId;
    }
}