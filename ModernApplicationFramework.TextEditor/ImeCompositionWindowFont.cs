using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("ImeCompositionWindowFont")]
    public sealed class ImeCompositionWindowFont : EditorOptionDefinition<string>
    {
        public static readonly EditorOptionKey<string> KeyId = new EditorOptionKey<string>(nameof(ImeCompositionWindowFont));
        public const string OptionName = "ImeCompositionWindowFont";

        public override string Default => "";

        public override EditorOptionKey<string> Key => KeyId;
    }
}