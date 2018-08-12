using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.OverviewMargin
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("OverviewMargin/ShowErrors")]
    public sealed class ErrorsEnabledOption : EditorOptionDefinition<bool>
    {
        public override bool Default => true;
        public override EditorOptionKey<bool> Key => DefaultTextViewHostOptions.ShowErrorsOptionId;
    }
}