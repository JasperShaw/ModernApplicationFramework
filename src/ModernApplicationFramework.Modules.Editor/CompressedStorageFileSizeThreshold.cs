using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("CompressedStorageFileSizeThreshold")]
    public sealed class CompressedStorageFileSizeThreshold : EditorOptionDefinition<int>
    {
        public override int Default => 5242880;

        public override EditorOptionKey<int> Key => TextModelEditorOptions.CompressedStorageFileSizeThresholdOptionId;
    }
}