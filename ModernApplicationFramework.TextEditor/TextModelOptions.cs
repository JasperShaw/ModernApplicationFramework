namespace ModernApplicationFramework.TextEditor
{
    public static class TextModelOptions
    {
        public static int CompressedStorageFileSizeThreshold = 5242880;
        public static int CompressedStoragePageSize = 1048576;
        public static int CompressedStorageMaxLoadedPages = 3;
        public static bool CompressedStorageGlobalManagement = false;
        public static bool CompressedStorageRetainWeakReferences = true;
        public static int StringRebuilderMaxCharactersToConsolidate = 200;
        public static int StringRebuilderMaxLinesToConsolidate = 8;
        public static int DiffSizeThreshold = 26214400;
    }
}