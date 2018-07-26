using System;

namespace ModernApplicationFramework.TextEditor
{
    public static class TextViewHostOptionExtensions
    {
        public static bool IsVerticalScrollBarEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.VerticalScrollBarId);
        }

        public static bool IsHorizontalScrollBarEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.HorizontalScrollBarId);
        }

        public static bool IsGlyphMarginEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.GlyphMarginId);
        }

        public static bool IsSelectionMarginEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.SelectionMarginId);
        }

        public static bool IsLineNumberMarginEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.LineNumberMarginId);
        }

        public static bool IsChangeTrackingEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.ChangeTrackingId);
        }

        public static bool IsOutliningMarginEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.OutliningMarginId);
        }

        public static bool IsZoomControlEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.ZoomControlId);
        }

        public static bool IsInContrastMode(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.IsInContrastModeId);
        }
    }
}