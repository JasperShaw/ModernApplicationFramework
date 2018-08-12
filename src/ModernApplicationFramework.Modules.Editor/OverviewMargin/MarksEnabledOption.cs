using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("OverviewMargin/ShowMarks")]
    public sealed class MarksEnabledOption : EditorOptionDefinition<bool>
    {
        public override EditorOptionKey<bool> Key => DefaultTextViewHostOptions.ShowMarksOptionId;

        public override bool Default => true;
    }
}
