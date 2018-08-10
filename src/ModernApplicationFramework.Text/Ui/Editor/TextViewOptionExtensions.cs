using System;
using ModernApplicationFramework.Text.Logic.Editor;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public static class TextViewOptionExtensions
    {
        public static bool IsVirtualSpaceEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewOptions.UseVirtualSpaceId);
        }

        public static bool IsOverwriteModeEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewOptions.OverwriteModeId);
        }

        public static bool IsAutoScrollEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewOptions.AutoScrollId);
        }

        public static WordWrapStyles WordWrapStyle(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewOptions.WordWrapStyleId);
        }

        public static bool IsVisibleWhitespaceEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewOptions.UseVisibleWhitespaceId);
        }

        public static bool DoesViewProhibitUserInput(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewOptions.ViewProhibitUserInputId);
        }

        //public static bool IsOutliningUndoEnabled(this IEditorOptions options)
        //{
        //    if (options == null)
        //        throw new ArgumentNullException(nameof(options));
        //    return options.GetOptionValue<bool>(DefaultTextViewOptions.OutliningUndoOptionId);
        //}

        public static bool IsDragDropEditingEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewOptions.DragDropEditingId);
        }

        public static bool IsViewportLeftClipped(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultTextViewOptions.IsViewportLeftClippedId);
        }
    }
}