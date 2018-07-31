using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("NewLineCharacter")]
    public sealed class NewLineCharacter : EditorOptionDefinition<string>
    {
        public override string Default => "\r\n";

        public override EditorOptionKey<string> Key => DefaultOptions.NewLineCharacterOptionId;
    }
}