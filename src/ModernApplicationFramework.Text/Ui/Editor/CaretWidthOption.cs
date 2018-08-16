using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name(DefaultTextViewOptions.CaretWidthOptionName)]
    public sealed class CaretWidthOption : EditorOptionDefinition<double>
    {
        public override double Default => 1.0;

        public override EditorOptionKey<double> Key => DefaultTextViewOptions.CaretWidthId;
    }
}