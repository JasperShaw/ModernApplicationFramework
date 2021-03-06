﻿using System;
using ModernApplicationFramework.Text.Logic.Editor;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public static class TextViewHostOptionExtensions
    {
        public static bool IsChangeTrackingEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.ChangeTrackingId);
        }

        public static bool IsGlyphMarginEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.GlyphMarginId);
        }

        public static bool IsHorizontalScrollBarEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.HorizontalScrollBarId);
        }

        public static bool IsInContrastMode(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.IsInContrastModeId);
        }

        public static bool IsLineNumberMarginEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.LineNumberMarginId);
        }

        public static bool IsOutliningMarginEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.OutliningMarginId);
        }

        public static bool IsSelectionMarginEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.SelectionMarginId);
        }

        public static bool IsVerticalScrollBarEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.VerticalScrollBarId);
        }

        public static bool IsZoomControlEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewHostOptions.ZoomControlId);
        }
    }
}