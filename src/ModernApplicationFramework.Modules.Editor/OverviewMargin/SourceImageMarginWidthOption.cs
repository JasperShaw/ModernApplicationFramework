using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("OverviewMargin/SourceImageMarginWidth")]
    public sealed class SourceImageMarginWidthOption : EditorOptionDefinition<double>
    {
        public override double Default => 100.0;
        public override EditorOptionKey<double> Key => DefaultTextViewHostOptions.SourceImageMarginWidthOptionId;

        public override bool IsValid(ref double proposedValue)
        {
            proposedValue = Math.Min(Math.Max(proposedValue, 0.0), 150.0);
            return true;
        }
    }
}