using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextView/AutoScroll")]
    public sealed class AutoScrollEnabled : ViewOptionDefinition<bool>
    {
        public override bool Default => false;

        public override EditorOptionKey<bool> Key => DefaultTextViewOptions.AutoScrollId;
    }
}