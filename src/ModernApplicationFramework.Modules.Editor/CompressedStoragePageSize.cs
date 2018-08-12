using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("CompressedStoragePageSize")]
    public sealed class CompressedStoragePageSize : EditorOptionDefinition<int>
    {
        public override int Default => 1048576;

        public override EditorOptionKey<int> Key => TextModelEditorOptions.CompressedStoragePageSizeOptionId;
    }
}