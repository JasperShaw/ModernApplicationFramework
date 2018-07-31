using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextView/DragDrop")]
    public sealed class DragDropEditing : ViewOptionDefinition<bool>
    {
        public override bool Default => true;

        public override EditorOptionKey<bool> Key => DefaultTextViewOptions.DragDropEditingId;
    }
}