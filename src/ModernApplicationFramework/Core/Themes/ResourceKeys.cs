using System.Windows;
using ModernApplicationFramework.Controls.Windows;

namespace ModernApplicationFramework.Core.Themes
{
    public static class ResourceKeys
    {
        private static object _scrollBarStyleKey;
        private static object _scrollViewerStyleKey;
        private static object _customGridViewScrollViewerStyleKey;
        private static object _comboBoxStyleKey;
        private static object _themedDialogButtonStyleKey;
        private static object _themedListViewStyleKey;
        private static object _themedListViewItemStyleKey;
        private static object _formsListViewItemStyleKey;
        private static object _formsCheckBoxStyle;

        public static object UnthemedScrollBarStyleKey => "ResourceKeys.UnthemedScrollBarStyleKey";

        public static object UnthemedGridViewScrollViewerStyleKey => "ResourceKeys.UnthemedGridViewScrollViewerStyleKey";

        public static object UnthemedScrollViewerStyleKey => "ResourceKeys.UnthemedScrollViewerStyleKey";

        public static object ScrollViewerStyleKey => _scrollViewerStyleKey ?? (_scrollViewerStyleKey =
                                                         GetResourceKey(nameof(ScrollViewerStyleKey)));

        public static object ScrollBarStyleKey => _scrollBarStyleKey ??
                                                  (_scrollBarStyleKey = GetResourceKey(nameof(ScrollBarStyleKey)));

        public static object CustomGridViewScrollViewerStyleKey => _customGridViewScrollViewerStyleKey ?? (_customGridViewScrollViewerStyleKey =
                                                                       GetResourceKey(nameof(CustomGridViewScrollViewerStyleKey)));

        public static string ComboBoxStyleKey => "MafComboBoxStyleKey";

        public static string ThemedDialogButtonStyleKey => nameof(ThemedDialogButtonStyleKey);

        public static string ThemedListViewStyleKey => nameof(ThemedListViewStyleKey);

        public static string ThemedListViewItemStyleKey => nameof(ThemedListViewItemStyleKey);

        public static string FormsListViewItemStyleKey => nameof(FormsListViewItemStyleKey);

        public static string FormsCheckBoxStyle => nameof(FormsCheckBoxStyle);


        //public static object ThemedDialogButtonStyleKey => _themedDialogButtonStyleKey ??
        //                                                                 (_themedDialogButtonStyleKey = GetResourceKey(nameof(ThemedDialogButtonStyleKey)));

        //public static object ThemedListViewStyleKey => _themedListViewStyleKey ??
        //                                                             (_themedListViewStyleKey = GetResourceKey(nameof(ThemedListViewStyleKey)));

        //public static object ThemedListViewItemStyleKey => _themedListViewItemStyleKey ??
        //                                                                 (_themedListViewItemStyleKey = GetResourceKey(nameof(ThemedListViewItemStyleKey)));

        //public static object FormsListViewItemStyleKey => _formsListViewItemStyleKey ??
        //                                                                (_formsListViewItemStyleKey = GetResourceKey(nameof(FormsListViewItemStyleKey)));

        //public static object FormsCheckBoxStyle => _formsCheckBoxStyle ??
        //                                                         (_formsCheckBoxStyle = GetResourceKey(nameof(FormsCheckBoxStyle)));

        public static object GetScrollBarStyleKey(bool themed)
        {
            if (!themed)
                return UnthemedScrollBarStyleKey;
            return ScrollBarStyleKey;
        }

        public static object GetScrollViewerStyleKey(bool themed)
        {
            if (!themed)
                return UnthemedScrollViewerStyleKey;
            return ScrollViewerStyleKey;
        }

        public static object GetGridViewScrollViewerStyleKey(bool themed)
        {
            if (!themed)
                return UnthemedGridViewScrollViewerStyleKey;
            return CustomGridViewScrollViewerStyleKey;
        }

        private static object GetResourceKey(string resourceId)
        {
            return new ComponentResourceKey(typeof(MainWindow), resourceId);
        }
    }
}
