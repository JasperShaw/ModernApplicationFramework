using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("Appearance/Category")]
    public sealed class AppearanceCategoryOption : ViewOptionDefinition<string>
    {
        public override string Default => "text";

        public override EditorOptionKey<string> Key => DefaultViewOptions.AppearanceCategory;
    }
}