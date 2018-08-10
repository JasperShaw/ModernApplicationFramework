using ModernApplicationFramework.Text.Logic.Editor;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public static class DefaultTextViewHostOptions
    {
        public static readonly EditorOptionKey<bool> VerticalScrollBarId = new EditorOptionKey<bool>(VerticalScrollBarName);
        public static readonly EditorOptionKey<bool> HorizontalScrollBarId = new EditorOptionKey<bool>(HorizontalScrollBarName);
        public static readonly EditorOptionKey<bool> GlyphMarginId = new EditorOptionKey<bool>(GlyphMarginName);
        public static readonly EditorOptionKey<bool> SuggestionMarginId = new EditorOptionKey<bool>(SuggestionMarginName);
        public static readonly EditorOptionKey<bool> SelectionMarginId = new EditorOptionKey<bool>(SelectionMarginName);
        public static readonly EditorOptionKey<bool> LineNumberMarginId = new EditorOptionKey<bool>(LineNumberMarginName);
        public static readonly EditorOptionKey<bool> ChangeTrackingId = new EditorOptionKey<bool>(ChangeTrackingName);
        public static readonly EditorOptionKey<bool> OutliningMarginId = new EditorOptionKey<bool>(OutliningMarginName);
        public static readonly EditorOptionKey<bool> ZoomControlId = new EditorOptionKey<bool>(ZoomControlName);
        public static readonly EditorOptionKey<bool> IsInContrastModeId = new EditorOptionKey<bool>(IsInContrastModeName);
        public static readonly EditorOptionKey<bool> ShowScrollBarAnnotationsOptionId = new EditorOptionKey<bool>(ShowScrollBarAnnotationsOptionName);
        public static readonly EditorOptionKey<bool> ShowEnhancedScrollBarOptionId = new EditorOptionKey<bool>(ShowEnhancedScrollBarOptionName);
        public static readonly EditorOptionKey<bool> ShowChangeTrackingMarginOptionId = new EditorOptionKey<bool>(ShowChangeTrackingMarginOptionName);
        public static readonly EditorOptionKey<double> ChangeTrackingMarginWidthOptionId = new EditorOptionKey<double>(ChangeTrackingMarginWidthOptionName);
        public static readonly EditorOptionKey<bool> ShowPreviewOptionId = new EditorOptionKey<bool>(ShowPreviewOptionName);
        public static readonly EditorOptionKey<int> PreviewSizeOptionId = new EditorOptionKey<int>(PreviewSizeOptionName);
        public static readonly EditorOptionKey<bool> ShowCaretPositionOptionId = new EditorOptionKey<bool>(ShowCaretPositionOptionName);
        public static readonly EditorOptionKey<bool> SourceImageMarginEnabledOptionId = new EditorOptionKey<bool>(SourceImageMarginEnabledOptionName);
        public static readonly EditorOptionKey<double> SourceImageMarginWidthOptionId = new EditorOptionKey<double>(SourceImageMarginWidthOptionName);
        public static readonly EditorOptionKey<bool> ShowMarksOptionId = new EditorOptionKey<bool>(SourceImageMarginWidthOptionName);
        public static readonly EditorOptionKey<bool> ShowErrorsOptionId = new EditorOptionKey<bool>(ShowErrorsOptionName);
        public static readonly EditorOptionKey<double> MarkMarginWidthOptionId = new EditorOptionKey<double>(MarkMarginWidthOptionName);
        public static readonly EditorOptionKey<double> ErrorMarginWidthOptionId = new EditorOptionKey<double>(ErrorMarginWidthOptionName);
        public const string VerticalScrollBarName = "TextViewHost/VerticalScrollBar";
        public const string HorizontalScrollBarName = "TextViewHost/HorizontalScrollBar";
        public const string GlyphMarginName = "TextViewHost/GlyphMargin";
        public const string SuggestionMarginName = "TextViewHost/SuggestionMargin";
        public const string SelectionMarginName = "TextViewHost/SelectionMargin";
        public const string LineNumberMarginName = "TextViewHost/LineNumberMargin";
        public const string ChangeTrackingName = "TextViewHost/ChangeTracking";
        public const string OutliningMarginName = "TextViewHost/OutliningMargin";
        public const string ZoomControlName = "TextViewHost/ZoomControl";
        public const string IsInContrastModeName = "TextViewHost/IsInContrastMode";
        public const string ShowScrollBarAnnotationsOptionName = "OverviewMargin/ShowScrollBarAnnotationsOption";
        public const string ShowEnhancedScrollBarOptionName = "OverviewMargin/ShowEnhancedScrollBar";
        public const string ShowChangeTrackingMarginOptionName = "OverviewMargin/ShowChangeTracking";
        public const string ChangeTrackingMarginWidthOptionName = "OverviewMargin/ChangeTrackingWidth";
        public const string ShowPreviewOptionName = "OverviewMargin/ShowPreview";
        public const string PreviewSizeOptionName = "OverviewMargin/PreviewSize";
        public const string ShowCaretPositionOptionName = "OverviewMargin/ShowCaretPosition";
        public const string SourceImageMarginEnabledOptionName = "OverviewMargin/ShowSourceImageMargin";
        public const string SourceImageMarginWidthOptionName = "OverviewMargin/SourceImageMarginWidth";
        public const string ShowMarksOptionName = "OverviewMargin/ShowMarks";
        public const string ShowErrorsOptionName = "OverviewMargin/ShowErrors";
        public const string MarkMarginWidthOptionName = "OverviewMargin/MarkMarginWidth";
        public const string ErrorMarginWidthOptionName = "OverviewMargin/ErrorMarginWidth";
    }
}