using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("ImeCompositionWindowTopOffset")]
    public sealed class ImeCompositionWindowTopOffset : EditorOptionDefinition<double>
    {
        public static readonly EditorOptionKey<double> KeyId = new EditorOptionKey<double>(nameof(ImeCompositionWindowTopOffset));
        public const string OptionName = "ImeCompositionWindowTopOffset";

        public override double Default => double.NaN;

        public override EditorOptionKey<double> Key => KeyId;
    }
}