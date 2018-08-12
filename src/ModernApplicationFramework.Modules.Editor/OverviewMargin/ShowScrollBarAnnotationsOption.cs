using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("OverviewMargin/ShowScrollBarAnnotationsOption")]
    public sealed class ShowScrollBarAnnotationsOption : EditorOptionDefinition<bool>
    {
        public override EditorOptionKey<bool> Key => DefaultTextViewHostOptions.ShowScrollBarAnnotationsOptionId;

        public override bool Default => false;
    }
}
