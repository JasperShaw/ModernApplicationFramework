namespace ModernApplicationFramework.TextEditor
{
    public static class DefaultViewOptions
    {
        public static readonly EditorOptionKey<bool> EnableHighlightCurrentLineId = new EditorOptionKey<bool>(EnableHighlightCurrentLineName);
        public static readonly EditorOptionKey<bool> EnableSimpleGraphicsId = new EditorOptionKey<bool>(EnableSimpleGraphicsName);
        public static readonly EditorOptionKey<bool> UseReducedOpacityForHighContrastOptionId = new EditorOptionKey<bool>(UseReducedOpacityForHighContrastOptionName);
        public static readonly EditorOptionKey<bool> EnableMouseWheelZoomId = new EditorOptionKey<bool>(EnableMouseWheelZoomName);
        public static readonly EditorOptionKey<string> AppearanceCategory = new EditorOptionKey<string>(AppearanceCategoryName);
        public static readonly EditorOptionKey<double> ZoomLevelId = new EditorOptionKey<double>(ZoomLevelName);
        //public static readonly EditorOptionKey<bool> ClickGoToDefEnabledId = new EditorOptionKey<bool>(ClickGoToDefEnabledName);
        //public static readonly EditorOptionKey<bool> ClickGoToDefOpensPeekId = new EditorOptionKey<bool>(ClickGoToDefOpensPeekName);
        //public static readonly EditorOptionKey<ModifierKeys> ClickGoToDefModifierKeyId = new EditorOptionKey<ModifierKeys>(ClickGoToDefModifierKeyName);


        public const string EnableHighlightCurrentLineName = "Adornments/HighlightCurrentLine/Enable";
        public const string EnableSimpleGraphicsName = "Graphics/Simple/Enable";
        public const string UseReducedOpacityForHighContrastOptionName = "UseReducedOpacityForHighContrast";
        public const string EnableMouseWheelZoomName = "TextView/MouseWheelZoom";
        public const string AppearanceCategoryName = "Appearance/Category";
        public const string ZoomLevelName = "TextView/ZoomLevel";
        //public const string ClickGoToDefEnabledName = "TextView/ClickGoToDefEnabled";
        //public const string ClickGoToDefOpensPeekName = "TextView/ClickGoToDefOpensPeek";
        //public const string ClickGoToDefModifierKeyName = "TextView/ClickGoToDefModifierKey";
    }
}