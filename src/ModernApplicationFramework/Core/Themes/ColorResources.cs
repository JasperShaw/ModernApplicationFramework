using System.Windows;

namespace ModernApplicationFramework.Core.Themes
{
    public static class ColorResources
    {
        private static ComponentResourceKey _backgroundLowerRegionBrush;
        private static ComponentResourceKey _backgroundLowerRegionTextBrush;
        private static ComponentResourceKey _backgroundBrush;
        private static ComponentResourceKey _contentBrush;
        private static ComponentResourceKey _contentTextBrush;
        private static ComponentResourceKey _windowText;
        private static ComponentResourceKey _contentMouseOverBrush;
        private static ComponentResourceKey _contentMouseOverTextBrush;
        private static ComponentResourceKey _contentSelectedBrush;
        private static ComponentResourceKey _contentSelectedTextBrush;
        private static ComponentResourceKey _contentSelectedBorderBrush;
        private static ComponentResourceKey _contentInactiveSelectedBrush;
        private static ComponentResourceKey _contentInactiveSelectedTextBrush;
        private static ComponentResourceKey _detailsBrush;
        private static ComponentResourceKey _detailsTextBrush;
        private static ComponentResourceKey _wonderbarTextBrush;
        private static ComponentResourceKey _wonderbarBrush;
        private static ComponentResourceKey _wonderbarTreeSelectedBrush;
        private static ComponentResourceKey _wonderbarTreeSelectedTextBrush;
        private static ComponentResourceKey _wonderbarSelectedBorderBrush;
        private static ComponentResourceKey _wonderbarMouseOverTextBrush;
        private static ComponentResourceKey _wonderbarMouseOverBrush;
        private static ComponentResourceKey _wonderbarTreeInactiveSelectedBrush;
        private static ComponentResourceKey _wonderbarTreeInactiveSelectedTextBrush;


        public static ComponentResourceKey BackgroundLowerRegionBrush => _backgroundLowerRegionBrush ??
                                                                     (_backgroundLowerRegionBrush = new ComponentResourceKey(typeof(ColorResources), nameof(BackgroundLowerRegionBrush)));

        public static ComponentResourceKey BackgroundLowerRegionTextBrush => _backgroundLowerRegionTextBrush ??
                                                                         (_backgroundLowerRegionTextBrush = new ComponentResourceKey(typeof(ColorResources), nameof(BackgroundLowerRegionTextBrush)));

        public static ComponentResourceKey BackgroundBrush => _backgroundBrush ??
                                                                             (_backgroundBrush = new ComponentResourceKey(typeof(ColorResources), nameof(BackgroundBrush)));

        public static ComponentResourceKey ContentBrush => _contentBrush ??
                                                              (_contentBrush = new ComponentResourceKey(typeof(ColorResources), nameof(ContentBrush)));

        public static ComponentResourceKey ContentTextBrush => _contentTextBrush ??
                                                              (_contentTextBrush = new ComponentResourceKey(typeof(ColorResources), nameof(ContentTextBrush)));

        public static ComponentResourceKey WindowText => _windowText ??
                                                         (_windowText = new ComponentResourceKey(typeof(ColorResources),
                                                             nameof(WindowText)));


        public static ComponentResourceKey ContentMouseOverBrush => _contentMouseOverBrush ??
                                                         (_contentMouseOverBrush = new ComponentResourceKey(typeof(ColorResources),
                                                             nameof(ContentMouseOverBrush)));

        public static ComponentResourceKey ContentMouseOverTextBrush => _contentMouseOverTextBrush ??
                                                                    (_contentMouseOverTextBrush = new ComponentResourceKey(typeof(ColorResources),
                                                                        nameof(ContentMouseOverTextBrush)));

        public static ComponentResourceKey ContentSelectedBrush => _contentSelectedBrush ??
                                                                           (_contentSelectedBrush = new ComponentResourceKey(typeof(ColorResources),
                                                                               nameof(ContentSelectedBrush)));

        public static ComponentResourceKey ContentSelectedTextBrush => _contentSelectedTextBrush ??
                                                                      (_contentSelectedTextBrush = new ComponentResourceKey(typeof(ColorResources),
                                                                          nameof(ContentSelectedTextBrush)));

        public static ComponentResourceKey ContentSelectedBorderBrush => _contentSelectedBorderBrush ??
                                                                          (_contentSelectedBorderBrush = new ComponentResourceKey(typeof(ColorResources),
                                                                              nameof(ContentSelectedBorderBrush)));

        public static ComponentResourceKey ContentInactiveSelectedBrush => _contentInactiveSelectedBrush ??
                                                                            (_contentInactiveSelectedBrush = new ComponentResourceKey(typeof(ColorResources),
                                                                                nameof(ContentInactiveSelectedBrush)));

        public static ComponentResourceKey ContentInactiveSelectedTextBrush => _contentInactiveSelectedTextBrush ??
                                                                              (_contentInactiveSelectedTextBrush = new ComponentResourceKey(typeof(ColorResources),
                                                                                  nameof(ContentInactiveSelectedTextBrush)));

        public static ComponentResourceKey DetailsBrush => _detailsBrush ?? (_detailsBrush = new ComponentResourceKey(
                                                               typeof(ColorResources),
                                                               nameof(DetailsBrush)));

        public static ComponentResourceKey DetailsTextBrush => _detailsTextBrush ?? (_detailsTextBrush = new ComponentResourceKey(
                                                               typeof(ColorResources),
                                                               nameof(DetailsTextBrush)));

        public static ComponentResourceKey WonderbarTextBrush => _wonderbarTextBrush ?? (_wonderbarTextBrush = new ComponentResourceKey(
                                                                   typeof(ColorResources),
                                                                   nameof(WonderbarTextBrush)));

        public static ComponentResourceKey WonderbarBrush => _wonderbarBrush ?? (_wonderbarBrush = new ComponentResourceKey(
                                                                     typeof(ColorResources),
                                                                     nameof(WonderbarBrush)));

        public static ComponentResourceKey WonderbarSelectedBorderBrush => _wonderbarSelectedBorderBrush ?? (_wonderbarSelectedBorderBrush = new ComponentResourceKey(
                                                                     typeof(ColorResources),
                                                                     nameof(WonderbarSelectedBorderBrush)));

        public static ComponentResourceKey WonderbarMouseOverTextBrush => _wonderbarMouseOverTextBrush ?? (_wonderbarMouseOverTextBrush = new ComponentResourceKey(
                                                                     typeof(ColorResources),
                                                                     nameof(WonderbarMouseOverTextBrush)));

        public static ComponentResourceKey WonderbarMouseOverBrush => _wonderbarMouseOverBrush ?? (_wonderbarMouseOverBrush = new ComponentResourceKey(
                                                                     typeof(ColorResources),
                                                                     nameof(WonderbarMouseOverBrush)));

        public static ComponentResourceKey WonderbarTreeInactiveSelectedTextBrush => _wonderbarTreeInactiveSelectedTextBrush ?? (_wonderbarTreeInactiveSelectedTextBrush = new ComponentResourceKey(
                                                                     typeof(ColorResources),
                                                                     nameof(WonderbarTreeInactiveSelectedTextBrush)));

        public static ComponentResourceKey WonderbarTreeInactiveSelectedBrush => _wonderbarTreeInactiveSelectedBrush ?? (_wonderbarTreeInactiveSelectedBrush = new ComponentResourceKey(
                                                                     typeof(ColorResources),
                                                                     nameof(WonderbarTreeInactiveSelectedBrush)));

        public static ComponentResourceKey WonderbarTreeSelectedTextBrush => _wonderbarTreeSelectedTextBrush ?? (_wonderbarTreeSelectedTextBrush = new ComponentResourceKey(
                                                                     typeof(ColorResources),
                                                                     nameof(WonderbarTreeSelectedTextBrush)));

        public static ComponentResourceKey WonderbarTreeSelectedBrush => _wonderbarTreeSelectedBrush ?? (_wonderbarTreeSelectedBrush = new ComponentResourceKey(
                                                                     typeof(ColorResources),
                                                                     nameof(WonderbarTreeSelectedBrush)));
    }
}
