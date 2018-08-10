using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("Enable Autonomous Find")]
    public sealed class EnableFindOptionDefinition : EditorOptionDefinition<bool>
    {
        public static readonly EditorOptionKey<bool> KeyId = new EditorOptionKey<bool>("Enable Autonomous Find");
        public const string OptionName = "Enable Autonomous Find";

        public override bool Default => false;

        public override EditorOptionKey<bool> Key => KeyId;
    }
}