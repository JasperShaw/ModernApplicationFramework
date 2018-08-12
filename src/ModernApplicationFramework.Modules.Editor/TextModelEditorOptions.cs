using ModernApplicationFramework.Text.Logic.Editor;

namespace ModernApplicationFramework.Modules.Editor
{
    public static class TextModelEditorOptions
    {
        public static readonly EditorOptionKey<int> CompressedStorageFileSizeThresholdOptionId = new EditorOptionKey<int>(CompressedStorageFileSizeThresholdOptionName);
        public static readonly EditorOptionKey<int> CompressedStoragePageSizeOptionId = new EditorOptionKey<int>(CompressedStoragePageSizeOptionName);
        public static readonly EditorOptionKey<int> CompressedStorageMaxLoadedPagesOptionId = new EditorOptionKey<int>(CompressedStorageMaxLoadedPagesOptionName);
        public static readonly EditorOptionKey<bool> CompressedStorageRetainWeakReferencesOptionId = new EditorOptionKey<bool>(CompressedStorageRetainWeakReferencesOptionName);
        public static readonly EditorOptionKey<int> DiffSizeThresholdOptionId = new EditorOptionKey<int>(DiffSizeThresholdOptionName);
        public const string CompressedStorageFileSizeThresholdOptionName = "CompressedStorageFileSizeThreshold";
        public const string CompressedStoragePageSizeOptionName = "CompressedStoragePageSize";
        public const string CompressedStorageMaxLoadedPagesOptionName = "CompressedStorageMaxLoadedPages";
        public const string CompressedStorageRetainWeakReferencesOptionName = "CompressedStorageRetainWeakReferences";
        public const string DiffSizeThresholdOptionName = "DiffSizeThreshold";
    }
}
