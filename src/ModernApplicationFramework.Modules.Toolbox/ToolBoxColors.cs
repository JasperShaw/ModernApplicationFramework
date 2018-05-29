using System.Windows;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public static  class ToolBoxColors
    {
        private static ComponentResourceKey _toolboxBackground;
        private static ComponentResourceKey _toolboxBackgroundText;
        private static ComponentResourceKey _toolboxItemHover;
        private static ComponentResourceKey _toolboxItemHoverText;
        private static ComponentResourceKey _toolboxItemSelected;
        private static ComponentResourceKey _toolboxItemSelectedText;
        private static ComponentResourceKey _toolboxItemInactive;
        private static ComponentResourceKey _toolboxItemInactiveText;
        private static ComponentResourceKey _toolboxCategoryExtenderBackground;
        private static ComponentResourceKey _toolboxCategorySelectedExtenderBackground;
        private static ComponentResourceKey _toolboxCategoryExtenderBorder;
        private static ComponentResourceKey _toolboxCategorySelectedExtenderBorder;
        private static ComponentResourceKey _toolboxCategoryInactiveExtenderBorder;
        private static ComponentResourceKey _toolboxCategoryInactiveExtenderBackground;

        public static ComponentResourceKey ToolboxBackground => _toolboxBackground ??
                                                           (_toolboxBackground =
                                                               new ComponentResourceKey(typeof(ToolBoxColors),
                                                                   nameof(ToolboxBackground)));

        public static ComponentResourceKey ToolboxBackgroundText => _toolboxBackgroundText ??
                                                                (_toolboxBackgroundText =
                                                                    new ComponentResourceKey(typeof(ToolBoxColors),
                                                                        nameof(ToolboxBackgroundText)));

        public static ComponentResourceKey ToolboxItemHover => _toolboxItemHover ??
                                                                    (_toolboxItemHover =
                                                                        new ComponentResourceKey(typeof(ToolBoxColors),
                                                                            nameof(ToolboxItemHover)));

        public static ComponentResourceKey ToolboxItemHoverText => _toolboxItemHoverText ??
                                                                (_toolboxItemHoverText =
                                                                    new ComponentResourceKey(typeof(ToolBoxColors),
                                                                        nameof(ToolboxItemHoverText)));

        public static ComponentResourceKey ToolboxItemSelected => _toolboxItemSelected ??
                                                                (_toolboxItemSelected =
                                                                    new ComponentResourceKey(typeof(ToolBoxColors),
                                                                        nameof(ToolboxItemSelected)));

        public static ComponentResourceKey ToolboxItemSelectedText => _toolboxItemSelectedText ??
                                                                    (_toolboxItemSelectedText =
                                                                        new ComponentResourceKey(typeof(ToolBoxColors),
                                                                            nameof(ToolboxItemSelectedText)));

        public static ComponentResourceKey ToolboxItemInactive => _toolboxItemInactive ??
                                                                (_toolboxItemInactive =
                                                                    new ComponentResourceKey(typeof(ToolBoxColors),
                                                                        nameof(ToolboxItemInactive)));

        public static ComponentResourceKey ToolboxItemInactiveText => _toolboxItemInactiveText ??
                                                                    (_toolboxItemInactiveText =
                                                                        new ComponentResourceKey(typeof(ToolBoxColors),
                                                                            nameof(ToolboxItemInactiveText)));

        public static ComponentResourceKey ToolboxCategoryExtenderBackground => _toolboxCategoryExtenderBackground ??
                                                                    (_toolboxCategoryExtenderBackground =
                                                                        new ComponentResourceKey(typeof(ToolBoxColors),
                                                                            nameof(ToolboxCategoryExtenderBackground)));

        public static ComponentResourceKey ToolboxCategorySelectedExtenderBackground => _toolboxCategorySelectedExtenderBackground ??
                                                                                (_toolboxCategorySelectedExtenderBackground =
                                                                                    new ComponentResourceKey(typeof(ToolBoxColors),
                                                                                        nameof(ToolboxCategorySelectedExtenderBackground)));

        public static ComponentResourceKey ToolboxCategoryExtenderBorder => _toolboxCategoryExtenderBorder ??
                                                                                (_toolboxCategoryExtenderBorder =
                                                                                    new ComponentResourceKey(typeof(ToolBoxColors),
                                                                                        nameof(ToolboxCategoryExtenderBorder)));

        public static ComponentResourceKey ToolboxCategorySelectedExtenderBorder => _toolboxCategorySelectedExtenderBorder ??
                                                                            (_toolboxCategorySelectedExtenderBorder =
                                                                                new ComponentResourceKey(typeof(ToolBoxColors),
                                                                                    nameof(ToolboxCategorySelectedExtenderBorder)));

        public static ComponentResourceKey ToolboxCategoryInactiveExtenderBorder => _toolboxCategoryInactiveExtenderBorder ??
                                                                                    (_toolboxCategoryInactiveExtenderBorder =
                                                                                        new ComponentResourceKey(typeof(ToolBoxColors),
                                                                                            nameof(ToolboxCategoryInactiveExtenderBorder)));

        public static ComponentResourceKey ToolboxCategoryInactiveExtenderBackground => _toolboxCategoryInactiveExtenderBackground ??
                                                                                    (_toolboxCategoryInactiveExtenderBackground =
                                                                                        new ComponentResourceKey(typeof(ToolBoxColors),
                                                                                            nameof(ToolboxCategoryInactiveExtenderBackground)));
    }
}