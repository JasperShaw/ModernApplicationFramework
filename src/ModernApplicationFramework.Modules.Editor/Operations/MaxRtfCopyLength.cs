using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Operations
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("MaxRtfCopyLength")]
    public sealed class MaxRtfCopyLength : EditorOptionDefinition<int>
    {
        public static readonly EditorOptionKey<int> OptionKey = new EditorOptionKey<int>(nameof(MaxRtfCopyLength));
        public const string OptionName = "MaxRtfCopyLength";

        public override int Default => 10240;

        public override EditorOptionKey<int> Key => OptionKey;
    }
}
