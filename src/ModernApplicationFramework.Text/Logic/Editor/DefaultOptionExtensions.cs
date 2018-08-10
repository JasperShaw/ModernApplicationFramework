using System;

namespace ModernApplicationFramework.Text.Logic.Editor
{
    public static class DefaultOptionExtensions
    {
        public static int GetIndentSize(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultOptions.IndentSizeOptionId);
        }

        public static bool GetInsertFinalNewLine(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultOptions.InsertFinalNewLineOptionId);
        }

        public static string GetNewLineCharacter(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultOptions.NewLineCharacterOptionId);
        }

        public static bool GetReplicateNewLineCharacter(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultOptions.ReplicateNewLineCharacterOptionId);
        }

        public static int GetTabSize(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultOptions.TabSizeOptionId);
        }

        public static bool GetTrimTrailingWhieSpace(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultOptions.TrimTrailingWhiteSpaceOptionId);
        }

        public static bool IsConvertTabsToSpacesEnabled(this IEditorOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            return options.GetOptionValue(DefaultOptions.ConvertTabsToSpacesOptionId);
        }
    }
}