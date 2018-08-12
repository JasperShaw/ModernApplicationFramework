using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Operations
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("UseAccurateClassificationForRtfCopy")]
    public sealed class UseAccurateClassificationForRtfCopy : EditorOptionDefinition<bool>
    {
        public static readonly EditorOptionKey<bool> OptionKey = new EditorOptionKey<bool>(nameof(UseAccurateClassificationForRtfCopy));
        public const string OptionName = "UseAccurateClassificationForRtfCopy";

        public override bool Default => false;

        public override EditorOptionKey<bool> Key => OptionKey;
    }
}
