namespace ModernApplicationFramework.TextEditor
{
    public static class DefaultTextViewOptions
    {
        public static readonly EditorOptionKey<bool> CutOrCopyBlankLineIfNoSelectionId = new EditorOptionKey<bool>(CutOrCopyBlankLineIfNoSelectionName);
        public static readonly EditorOptionKey<bool> ViewProhibitUserInputId = new EditorOptionKey<bool>(ViewProhibitUserInputName);
        public static readonly EditorOptionKey<WordWrapStyles> WordWrapStyleId = new EditorOptionKey<WordWrapStyles>(WordWrapStyleName);
        public static readonly EditorOptionKey<bool> UseVirtualSpaceId = new EditorOptionKey<bool>(UseVirtualSpaceName);
        public static readonly EditorOptionKey<bool> IsViewportLeftClippedId = new EditorOptionKey<bool>(IsViewportLeftClippedName);
        public static readonly EditorOptionKey<bool> OverwriteModeId = new EditorOptionKey<bool>(OverwriteModeName);
        public static readonly EditorOptionKey<bool> AutoScrollId = new EditorOptionKey<bool>(AutoScrollName);
        public static readonly EditorOptionKey<bool> UseVisibleWhitespaceId = new EditorOptionKey<bool>(UseVisibleWhitespaceName);
        //public static readonly EditorOptionKey<bool> ShowBlockStructureId = new EditorOptionKey<bool>(ShowBlockStructureName);
        //public static readonly EditorOptionKey<bool> ProduceScreenReaderFriendlyTextId = new EditorOptionKey<bool>(ProduceScreenReaderFriendlyTextName);
        //public static readonly EditorOptionKey<bool> OutliningUndoOptionId = new EditorOptionKey<bool>(OutliningUndoOptionName);
        //public static readonly EditorOptionKey<bool> DisplayUrlsAsHyperlinksId = new EditorOptionKey<bool>(DisplayUrlsAsHyperlinksName);
        public static readonly EditorOptionKey<bool> DragDropEditingId = new EditorOptionKey<bool>(DragDropEditingName);
       //public static readonly EditorOptionKey<bool> BraceCompletionEnabledOptionId = new EditorOptionKey<bool>(BraceCompletionEnabledOptionName);

        public const string CutOrCopyBlankLineIfNoSelectionName = "TextView/CutOrCopyBlankLineIfNoSelection";
        public const string ViewProhibitUserInputName = "TextView/ProhibitUserInput";
        public const string WordWrapStyleName = "TextView/WordWrapStyle";
        public const string UseVirtualSpaceName = "TextView/UseVirtualSpace";
        public const string IsViewportLeftClippedName = "TextView/IsViewportLeftClipped";
        public const string OverwriteModeName = "TextView/OverwriteMode";
        public const string AutoScrollName = "TextView/AutoScroll";
        public const string UseVisibleWhitespaceName = "TextView/UseVisibleWhitespace";
        //public const string ShowBlockStructureName = "TextView/ShowBlockStructure";
        //public const string ProduceScreenReaderFriendlyTextName = "TextView/ProduceScreenReaderFriendlyText";
        //public const string OutliningUndoOptionName = "TextView/OutliningUndo";
        //public const string DisplayUrlsAsHyperlinksName = "TextView/DisplayUrlsAsHyperlinks";
        public const string DragDropEditingName = "TextView/DragDrop";
        //public const string BraceCompletionEnabledOptionName = "BraceCompletion/Enabled";
    }
}