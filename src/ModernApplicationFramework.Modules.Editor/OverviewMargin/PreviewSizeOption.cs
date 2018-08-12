using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("OverviewMargin/PreviewSize")]
    public sealed class PreviewSizeOption : EditorOptionDefinition<int>
    {
        public override EditorOptionKey<int> Key => DefaultTextViewHostOptions.PreviewSizeOptionId;

        public override int Default => 7;

        public override bool IsValid(ref int proposedValue)
        {
            proposedValue = Math.Min(Math.Max(proposedValue, 0), 20);
            return true;
        }
    }
}
