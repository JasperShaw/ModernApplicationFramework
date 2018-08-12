using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("OverviewMargin/ShowEnhancedScrollBar")]
    public sealed class UseEnhancedScrollBarOption : EditorOptionDefinition<bool>
    {
        public override bool Default => false;
        public override EditorOptionKey<bool> Key => DefaultTextViewHostOptions.ShowEnhancedScrollBarOptionId;
    }
}