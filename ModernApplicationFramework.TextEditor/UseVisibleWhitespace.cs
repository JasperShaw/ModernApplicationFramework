using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextView/UseVisibleWhitespace")]
    public sealed class UseVisibleWhitespace : ViewOptionDefinition<bool>
    {
        public override bool Default => false;

        public override EditorOptionKey<bool> Key => DefaultTextViewOptions.UseVisibleWhitespaceId;
    }
}