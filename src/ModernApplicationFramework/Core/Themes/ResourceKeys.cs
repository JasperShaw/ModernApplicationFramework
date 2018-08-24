using System;
using System.Reflection;
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

        static ResourceKeys()
        {
            Application.LoadComponent(new Uri(
                "/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/" + "Themes/Generic.xaml",
                UriKind.Relative));
        }

        public static object UnthemedScrollBarStyleKey => "ResourceKeys.UnthemedScrollBarStyleKey";

        public static object UnthemedGridViewScrollViewerStyleKey => "ResourceKeys.UnthemedGridViewScrollViewerStyleKey";

        public static object UnthemedScrollViewerStyleKey => "ResourceKeys.UnthemedScrollViewerStyleKey";

        public static object ScrollViewerStyleKey => _scrollViewerStyleKey ?? (_scrollViewerStyleKey =
                                                         GetResourceKey(nameof(ScrollViewerStyleKey)));

        public static object ScrollBarStyleKey => _scrollBarStyleKey ??
                                                  (_scrollBarStyleKey = GetResourceKey(nameof(ScrollBarStyleKey)));

        public static object CustomGridViewScrollViewerStyleKey => _customGridViewScrollViewerStyleKey ?? (_customGridViewScrollViewerStyleKey =
                                                                       GetResourceKey(nameof(CustomGridViewScrollViewerStyleKey)));

        public static object ComboBoxStyleKey => _comboBoxStyleKey ?? (_comboBoxStyleKey =
                                                     GetResourceKey(nameof(_comboBoxStyleKey)));

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
