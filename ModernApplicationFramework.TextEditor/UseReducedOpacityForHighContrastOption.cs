using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("UseReducedOpacityForHighContrast")]
    public sealed class UseReducedOpacityForHighContrastOption : EditorOptionDefinition<bool>
    {
        public override bool Default => false;

        public override EditorOptionKey<bool> Key => DefaultViewOptions.UseReducedOpacityForHighContrastOptionId;
    }
}