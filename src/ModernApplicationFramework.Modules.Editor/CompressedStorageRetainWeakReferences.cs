using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("CompressedStorageRetainWeakReferences")]
    public sealed class CompressedStorageRetainWeakReferences : EditorOptionDefinition<bool>
    {
        public override bool Default => true;

        public override EditorOptionKey<bool> Key => TextModelEditorOptions.CompressedStorageRetainWeakReferencesOptionId;
    }
}