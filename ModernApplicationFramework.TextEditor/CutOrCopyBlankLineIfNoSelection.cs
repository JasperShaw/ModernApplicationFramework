using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextView/CutOrCopyBlankLineIfNoSelection")]
    public sealed class CutOrCopyBlankLineIfNoSelection : ViewOptionDefinition<bool>
    {
        public override bool Default => true;

        public override EditorOptionKey<bool> Key => DefaultTextViewOptions.CutOrCopyBlankLineIfNoSelectionId;
    }
}