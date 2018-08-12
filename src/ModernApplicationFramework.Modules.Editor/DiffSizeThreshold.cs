using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("DiffSizeThreshold")]
    public sealed class DiffSizeThreshold : EditorOptionDefinition<int>
    {
        public override int Default => 26214400;

        public override EditorOptionKey<int> Key => TextModelEditorOptions.DiffSizeThresholdOptionId;
    }
}