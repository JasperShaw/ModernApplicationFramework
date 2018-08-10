using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Logic.Editor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TrimTrailingWhiteSpace")]
    public sealed class TrimTrailingWhiteSpace : EditorOptionDefinition<bool>
    {
        public override bool Default => false;

        public override EditorOptionKey<bool> Key => DefaultOptions.TrimTrailingWhiteSpaceOptionId;
    }
}