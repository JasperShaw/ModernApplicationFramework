namespace ModernApplicationFramework.TextEditor
{
    public static class DefaultOptions
    {
        public static readonly EditorOptionKey<bool> ConvertTabsToSpacesOptionId = new EditorOptionKey<bool>(ConvertTabsToSpacesOptionName);
        public static readonly EditorOptionKey<int> TabSizeOptionId = new EditorOptionKey<int>(TabSizeOptionName);
        public static readonly EditorOptionKey<int> IndentSizeOptionId = new EditorOptionKey<int>(IndentSizeOptionName);
        public static readonly EditorOptionKey<bool> ReplicateNewLineCharacterOptionId = new EditorOptionKey<bool>(ReplicateNewLineCharacterOptionName);
        public static readonly EditorOptionKey<string> NewLineCharacterOptionId = new EditorOptionKey<string>(NewLineCharacterOptionName);
        public static readonly EditorOptionKey<int> LongBufferLineThresholdId = new EditorOptionKey<int>(LongBufferLineThresholdOptionName);
        public static readonly EditorOptionKey<int> LongBufferLineChunkLengthId = new EditorOptionKey<int>(LongBufferLineChunkLengthOptionName);
        public static readonly EditorOptionKey<bool> TrimTrailingWhiteSpaceOptionId = new EditorOptionKey<bool>(TrimTrailingWhiteSpaceOptionName);
        public static readonly EditorOptionKey<bool> InsertFinalNewLineOptionId = new EditorOptionKey<bool>(InsertFinalNewLineOptionName);
        public const string ConvertTabsToSpacesOptionName = "Tabs/ConvertTabsToSpaces";
        public const string TabSizeOptionName = "Tabs/TabSize";
        public const string IndentSizeOptionName = "Tabs/IndentSize";
        public const string ReplicateNewLineCharacterOptionName = "ReplicateNewLineCharacter";
        public const string NewLineCharacterOptionName = "NewLineCharacter";
        public const string LongBufferLineThresholdOptionName = "LongBufferLineThreshold";
        public const string LongBufferLineChunkLengthOptionName = "LongBufferLineChunkLength";
        public const string TrimTrailingWhiteSpaceOptionName = "TrimTrailingWhiteSpace";
        public const string InsertFinalNewLineOptionName = "InsertFinalNewLine";
    }
}