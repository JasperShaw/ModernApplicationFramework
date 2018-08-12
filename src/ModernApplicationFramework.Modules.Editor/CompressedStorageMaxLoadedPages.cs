using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("CompressedStorageMaxLoadedPages")]
    public sealed class CompressedStorageMaxLoadedPages : EditorOptionDefinition<int>
    {
        public override int Default => 3;

        public override EditorOptionKey<int> Key => TextModelEditorOptions.CompressedStorageMaxLoadedPagesOptionId;
    }
}