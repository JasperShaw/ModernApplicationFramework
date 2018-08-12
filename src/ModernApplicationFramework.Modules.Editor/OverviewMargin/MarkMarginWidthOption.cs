using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("OverviewMargin/MarkMarginWidth")]
    public sealed class MarkMarginWidthOption : EditorOptionDefinition<double>
    {
        public override double Default => 6.0;
        public override EditorOptionKey<double> Key => DefaultTextViewHostOptions.MarkMarginWidthOptionId;

        public override bool IsValid(ref double proposedValue)
        {
            proposedValue = Math.Min(Math.Max(proposedValue, 3.0), 20.0);
            return true;
        }
    }
}