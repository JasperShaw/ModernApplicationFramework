using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("OverviewMargin/ShowCaretPosition")]
    public sealed class ShowCaretPositionOption : EditorOptionDefinition<bool>
    {
        public override bool Default => true;
        public override EditorOptionKey<bool> Key => DefaultTextViewHostOptions.ShowCaretPositionOptionId;
    }
}