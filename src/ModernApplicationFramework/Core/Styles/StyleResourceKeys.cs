using System.Windows;

namespace ModernApplicationFramework.Core.Styles
{
    public static class StyleResourceKeys
    {
        private static ComponentResourceKey _themedDialogButtonStyleKey;
        private static ComponentResourceKey _themedListViewStyleKey;
        private static ComponentResourceKey _themedListViewItemStyleKey;
        private static ComponentResourceKey _formsListViewItemStyleKey;
        private static ComponentResourceKey _formsCheckBoxStyle;

        public static ComponentResourceKey ThemedDialogButtonStyleKey => _themedDialogButtonStyleKey ??
                                                                     (_themedDialogButtonStyleKey = new ComponentResourceKey(typeof(StyleResourceKeys), nameof(ThemedDialogButtonStyleKey)));

        public static ComponentResourceKey ThemedListViewStyleKey => _themedListViewStyleKey ??
                                                                         (_themedListViewStyleKey = new ComponentResourceKey(typeof(StyleResourceKeys), nameof(ThemedListViewStyleKey)));

        public static ComponentResourceKey ThemedListViewItemStyleKey => _themedListViewItemStyleKey ??
                                                                     (_themedListViewItemStyleKey = new ComponentResourceKey(typeof(StyleResourceKeys), nameof(ThemedListViewItemStyleKey)));

        public static ComponentResourceKey FormsListViewItemStyleKey => _formsListViewItemStyleKey ??
                                                                         (_formsListViewItemStyleKey = new ComponentResourceKey(typeof(StyleResourceKeys), nameof(FormsListViewItemStyleKey)));

        public static ComponentResourceKey FormsCheckBoxStyle => _formsCheckBoxStyle ??
                                                                         (_formsCheckBoxStyle = new ComponentResourceKey(typeof(StyleResourceKeys), nameof(FormsCheckBoxStyle)));
    }
}
