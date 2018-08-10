using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("ImeCompositionWindowHeightOffset")]
    public sealed class ImeCompositionWindowHeightOffset : EditorOptionDefinition<double>
    {
        public static readonly EditorOptionKey<double> KeyId = new EditorOptionKey<double>(nameof(ImeCompositionWindowHeightOffset));
        public const string OptionName = "ImeCompositionWindowHeightOffset";

        public override double Default => double.NaN;

        public override EditorOptionKey<double> Key => KeyId;
    }
}