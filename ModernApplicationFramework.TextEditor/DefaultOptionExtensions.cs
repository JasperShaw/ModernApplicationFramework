using System;

namespace ModernApplicationFramework.TextEditor
{
    public static class DefaultOptionExtensions
    {
        public static bool IsConvertTabsToSpacesEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultOptions.ConvertTabsToSpacesOptionId);
        }

        public static int GetTabSize(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultOptions.TabSizeOptionId);
        }

        public static int GetIndentSize(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultOptions.IndentSizeOptionId);
        }

        public static bool GetReplicateNewLineCharacter(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultOptions.ReplicateNewLineCharacterOptionId);
        }

        public static string GetNewLineCharacter(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultOptions.NewLineCharacterOptionId);
        }

        public static bool GetTrimTrailingWhieSpace(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultOptions.TrimTrailingWhiteSpaceOptionId);
        }

        public static bool GetInsertFinalNewLine(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultOptions.InsertFinalNewLineOptionId);
        }
    }
}