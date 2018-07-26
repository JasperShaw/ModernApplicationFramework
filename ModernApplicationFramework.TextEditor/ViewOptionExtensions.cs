using System;

namespace ModernApplicationFramework.TextEditor
{
    public static class ViewOptionExtensions
    {
        public static bool IsHighlightCurrentLineEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultWpfViewOptions.EnableHighlightCurrentLineId);
        }

        public static bool IsSimpleGraphicsEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultWpfViewOptions.EnableSimpleGraphicsId);
        }

        public static bool IsMouseWheelZoomEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultWpfViewOptions.EnableMouseWheelZoomId);
        }

        public static string AppearanceCategory(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultWpfViewOptions.AppearanceCategory);
        }

        public static double ZoomLevel(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultWpfViewOptions.ZoomLevelId);
        }
    }
}