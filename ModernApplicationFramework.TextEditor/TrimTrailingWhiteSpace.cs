using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TrimTrailingWhiteSpace")]
    public sealed class TrimTrailingWhiteSpace : EditorOptionDefinition<bool>
    {
        public override bool Default => false;

        public override EditorOptionKey<bool> Key => DefaultOptions.TrimTrailingWhiteSpaceOptionId;
    }
}