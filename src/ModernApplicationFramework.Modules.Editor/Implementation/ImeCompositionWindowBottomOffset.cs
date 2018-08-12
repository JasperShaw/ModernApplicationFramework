using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("ImeCompositionWindowBottomOffset")]
    public sealed class ImeCompositionWindowBottomOffset : EditorOptionDefinition<double>
    {
        public static readonly EditorOptionKey<double> KeyId = new EditorOptionKey<double>(nameof(ImeCompositionWindowBottomOffset));
        public const string OptionName = "ImeCompositionWindowBottomOffset";

        public override double Default => double.NaN;

        public override EditorOptionKey<double> Key => KeyId;
    }
}