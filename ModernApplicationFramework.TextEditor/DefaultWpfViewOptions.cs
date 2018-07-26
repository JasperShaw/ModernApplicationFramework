using System.Windows.Input;

namespace ModernApplicationFramework.TextEditor
{
    public static class DefaultWpfViewOptions
    {
        public static readonly EditorOptionKey<bool> EnableHighlightCurrentLineId = new EditorOptionKey<bool>("Adornments/HighlightCurrentLine/Enable");
        public static readonly EditorOptionKey<bool> EnableSimpleGraphicsId = new EditorOptionKey<bool>("Graphics/Simple/Enable");
        public static readonly EditorOptionKey<bool> UseReducedOpacityForHighContrastOptionId = new EditorOptionKey<bool>("UseReducedOpacityForHighContrast");
        public static readonly EditorOptionKey<bool> EnableMouseWheelZoomId = new EditorOptionKey<bool>("TextView/MouseWheelZoom");
        public static readonly EditorOptionKey<string> AppearanceCategory = new EditorOptionKey<string>("Appearance/Category");
        public static readonly EditorOptionKey<double> ZoomLevelId = new EditorOptionKey<double>("TextView/ZoomLevel");
        public static readonly EditorOptionKey<bool> ClickGoToDefEnabledId = new EditorOptionKey<bool>("TextView/ClickGoToDefEnabled");
        public static readonly EditorOptionKey<bool> ClickGoToDefOpensPeekId = new EditorOptionKey<bool>("TextView/ClickGoToDefOpensPeek");
        public static readonly EditorOptionKey<ModifierKeys> ClickGoToDefModifierKeyId = new EditorOptionKey<ModifierKeys>("TextView/ClickGoToDefModifierKey");
        public const string EnableHighlightCurrentLineName = "Adornments/HighlightCurrentLine/Enable";
        public const string EnableSimpleGraphicsName = "Graphics/Simple/Enable";
        public const string UseReducedOpacityForHighContrastOptionName = "UseReducedOpacityForHighContrast";
        public const string EnableMouseWheelZoomName = "TextView/MouseWheelZoom";
        public const string AppearanceCategoryName = "Appearance/Category";
        public const string ZoomLevelName = "TextView/ZoomLevel";
        public const string ClickGoToDefEnabledName = "TextView/ClickGoToDefEnabled";
        public const string ClickGoToDefOpensPeekName = "TextView/ClickGoToDefOpensPeek";
        public const string ClickGoToDefModifierKeyName = "TextView/ClickGoToDefModifierKey";
    }
}