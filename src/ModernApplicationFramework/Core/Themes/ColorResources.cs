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
    }
}
