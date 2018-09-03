using System.Windows;

namespace ModernApplicationFramework.Docking
{
    public static class DockingColors
    {
        private static ComponentResourceKey _anchorPaneTitleBackgroundActive;
        private static ComponentResourceKey _anchorPaneTitleGlyph;
        private static ComponentResourceKey _anchorPaneTitleGlyphHover;
        private static ComponentResourceKey _anchorPaneTitleGlyphDown;
        private static ComponentResourceKey _anchorPaneTitleGlyphActive;
        private static ComponentResourceKey _anchorPaneTitleGlyphActiveHover;
        private static ComponentResourceKey _anchorPaneTitleGlyphActiveDown;
        private static ComponentResourceKey _anchorPaneTitleButtonBackground;
        private static ComponentResourceKey _anchorPaneTitleButtonBackgroundHover;
        private static ComponentResourceKey _anchorPaneTitleButtonBackgroundDown;
        private static ComponentResourceKey _anchorPaneTitleButtonBackgroundActive;
        private static ComponentResourceKey _anchorPaneTitleButtonBackgroundActiveHover;
        private static ComponentResourceKey _anchorPaneTitleButtonBackgroundActiveDown;
        private static ComponentResourceKey _anchorPaneTitleButtonBorder;
        private static ComponentResourceKey _anchorPaneTitleButtonBorderHover;
        private static ComponentResourceKey _anchorPaneTitleButtonBorderDown;
        private static ComponentResourceKey _anchorPaneTitleButtonBorderActive;
        private static ComponentResourceKey _anchorPaneTitleButtonBorderActiveHover;
        private static ComponentResourceKey _anchorPaneTitleButtonBorderActiveDown;
        private static ComponentResourceKey _anchorPaneTitleGrip;
        private static ComponentResourceKey _anchorPaneTitleGripActive;
        private static ComponentResourceKey _anchorPaneTitleText;
        private static ComponentResourceKey _anchorPaneTitleTextActive;
        private static ComponentResourceKey _anchorSideItemForegroundHover;
        private static ComponentResourceKey _anchorSideItemBackground;
        private static ComponentResourceKey _anchorSideItemBackgroundHover;
        private static ComponentResourceKey _anchorSideItemBorder;
        private static ComponentResourceKey _anchorSideItemBorderHover;
        private static ComponentResourceKey _anchorSideItemForeground;
        private static ComponentResourceKey _dockingManagerBackground;
        private static ComponentResourceKey _anchorableControlBackground;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarBackground;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarBackgroundActive;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarGlyph;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarGlyphActive;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarGlyphHover;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarGlyphDown;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarGlyphActiveHover;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarGlyphActiveDown;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarForeground;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarForegroundActive;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBackground;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBackgroundHover;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBackgroundDown;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBorder;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBorderHover;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBorderDown;
        private static ComponentResourceKey _anchorableFloatingWindowGrip;
        private static ComponentResourceKey _anchorableFloatingWindowGripActive;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBackgroundActive;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBackgroundActiveHover;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBackgroundActiveDown;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBorderActive;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBorderActiveHover;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBorderActiveDown;
        private static ComponentResourceKey _anchorTabItemBackground;
        private static ComponentResourceKey _anchorTabItemBackgroundHover;
        private static ComponentResourceKey _anchorTabItemBackgroundActive;
        private static ComponentResourceKey _anchorTabItemBackgroundDisabled;
        private static ComponentResourceKey _anchorTabItemBorder;
        private static ComponentResourceKey _anchorTabItemBorderHover;
        private static ComponentResourceKey _anchorTabItemBorderActive;
        private static ComponentResourceKey _anchorTabItemBorderDisabled;
        private static ComponentResourceKey _anchorTabItemText;
        private static ComponentResourceKey _anchorTabItemTextHover;
        private static ComponentResourceKey _anchorTabItemTextActive;
        private static ComponentResourceKey _anchorTabItemTextDisabled;
        private static ComponentResourceKey _anchorPaneControlBorder;
        private static ComponentResourceKey _layoutAutoHideWindowBorder;
        private static ComponentResourceKey _layoutAutoHideWindowBackground;
        private static ComponentResourceKey _floatingWindowBackground;
        private static ComponentResourceKey _floatingWindowTitleBarBackground;
        private static ComponentResourceKey _floatingWindowTitleBarGlyph;
        private static ComponentResourceKey _floatingWindowTitleBarGlyphHover;
        private static ComponentResourceKey _floatingWindowTitleBarGlyphDown;
        private static ComponentResourceKey _floatingWindowTitleBarForeground;
        private static ComponentResourceKey _floatingWindowTitleBarForegroundActive;
        private static ComponentResourceKey _documentTabItemBorderDisabled;
        private static ComponentResourceKey _documentTabItemText;
        private static ComponentResourceKey _documentTabItemTextHover;
        private static ComponentResourceKey _documentTabItemTextActive;
        private static ComponentResourceKey _documentTabItemTextLastActive;
        private static ComponentResourceKey _documentTabItemGlyph;
        private static ComponentResourceKey _documentTabItemGlyphHover;
        private static ComponentResourceKey _documentTabItemGlyphDown;
        private static ComponentResourceKey _documentTabItemGlyphActive;
        private static ComponentResourceKey _documentTabItemGlyphActiveHover;
        private static ComponentResourceKey _documentTabItemGlyphActiveDown;
        private static ComponentResourceKey _documentTabItemGlyphLastActive;
        private static ComponentResourceKey _documentTabItemGlyphLastActiveHover;
        private static ComponentResourceKey _documentTabItemGlyphLastActiveDown;
        private static ComponentResourceKey _documentTabItemButtonBackground;
        private static ComponentResourceKey _documentTabItemButtonBackgroundHover;
        private static ComponentResourceKey _documentTabItemButtonBackgroundDown;
        private static ComponentResourceKey _documentTabItemButtonBackgroundActive;
        private static ComponentResourceKey _documentTabItemButtonBackgroundActiveHover;
        private static ComponentResourceKey _documentTabItemButtonBackgroundActiveDown;
        private static ComponentResourceKey _documentTabItemButtonBackgroundLastActive;
        private static ComponentResourceKey _documentTabItemButtonBackgroundLastActiveHover;
        private static ComponentResourceKey _documentTabItemButtonBackgroundLastActiveDown;
        private static ComponentResourceKey _documentTabItemButtonBorder;
        private static ComponentResourceKey _documentTabItemButtonBorderHover;
        private static ComponentResourceKey _documentTabItemButtonBorderDown;
        private static ComponentResourceKey _documentTabItemButtonBorderActive;
        private static ComponentResourceKey _documentTabItemButtonBorderActiveHover;
        private static ComponentResourceKey _documentTabItemButtonBorderActiveDown;
        private static ComponentResourceKey _documentTabItemButtonBorderLastActive;
        private static ComponentResourceKey _documentTabItemButtonBorderLastActiveHover;
        private static ComponentResourceKey _documentTabItemButtonBorderLastActiveDown;
        private static ComponentResourceKey _documentTabItemBackground;
        private static ComponentResourceKey _documentTabItemBackgroundHover;
        private static ComponentResourceKey _documentTabItemBackgroundActive;
        private static ComponentResourceKey _documentTabItemBackgroundLastActive;
        private static ComponentResourceKey _documentTabItemBackgroundDisabled;
        private static ComponentResourceKey _documentTabItemBorder;
        private static ComponentResourceKey _documentTabItemBorderHover;
        private static ComponentResourceKey _documentTabItemBorderActive;
        private static ComponentResourceKey _documentTabItemBorderLastActive;
        private static ComponentResourceKey _layoutGridResizerBackground;
        private static ComponentResourceKey _layoutGridResizerBackgroundHorizontal;
        private static ComponentResourceKey _navigatorWindowItemBorderChecked;
        private static ComponentResourceKey _navigatorWindowItemForeground;
        private static ComponentResourceKey _navigatorWindowBorder;
        private static ComponentResourceKey _navigatorWindowBackground;
        private static ComponentResourceKey _overlayWindowPreviewBoxBorder;
        private static ComponentResourceKey _overlayWindowPreviewBoxBackground;
        private static ComponentResourceKey _navigatorWindowItemBackground;
        private static ComponentResourceKey _navigatorWindowTextForeground;
        private static ComponentResourceKey _navigatorWindowItemBackgroundChecked;
        private static ComponentResourceKey _navigatorWindowItemForegroundChecked;
        private static ComponentResourceKey _navigatorWindowTitleText;
        private static ComponentResourceKey _navigatorWindowItemBorder;
        private static ComponentResourceKey _documentPaneControlBackground;
        private static ComponentResourceKey _documentPaneControlBackgroundLastActive;
        private static ComponentResourceKey _documentPaneControlGlyph;
        private static ComponentResourceKey _documentPaneControlGlyphHover;
        private static ComponentResourceKey _documentPaneControlGlyphDown;
        private static ComponentResourceKey _documentPaneControlButtonBackground;
        private static ComponentResourceKey _documentPaneControlButtonBackgroundHover;
        private static ComponentResourceKey _documentPaneControlButtonBackgroundDown;
        private static ComponentResourceKey _documentPaneControlButtonBorder;
        private static ComponentResourceKey _documentPaneControlButtonBorderHover;
        private static ComponentResourceKey _documentPaneControlButtonBorderDown;
        private static ComponentResourceKey _documentPaneControlBorder;
        private static ComponentResourceKey _anchorFloatingWindowTitleBarButtonBackground;
        private static ComponentResourceKey _anchorFloatingWindowTitleBarButtonBorder;
        private static ComponentResourceKey _anchorFloatingWindowTitleBarButtonBackgroundHover;
        private static ComponentResourceKey _anchorFloatingWindowTitleBarButtonBorderHover;
        private static ComponentResourceKey _anchorFloatingWindowTitleBarButtonBackgroundPressed;
        private static ComponentResourceKey _anchorFloatingWindowTitleBarButtonBorderPressed;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarGlyphDisabled;
        private static ComponentResourceKey _documentPaneControlFileTabBorderInactive;
        private static ComponentResourceKey _documentPaneControlFileTabBorder;
        private static ComponentResourceKey _docWellOverflowButtonMouseDownGlyph;
        private static ComponentResourceKey _docWellOverflowButtonGlyph;
        private static ComponentResourceKey _docWellOverflowButtonMouseOverBackground;
        private static ComponentResourceKey _docWellOverflowButtonMouseOverBorder;
        private static ComponentResourceKey _docWellOverflowButtonMouseOverGlyph;
        private static ComponentResourceKey _docWellOverflowButtonMouseDownBackground;
        private static ComponentResourceKey _docWellOverflowButtonMouseDownBorder;
        private static ComponentResourceKey _documentTabGlyphHot;
        private static ComponentResourceKey _documentTabGlyphInactive;
        private static ComponentResourceKey _anchorFloatingWindowTitleBarButtonGlyph;
        private static ComponentResourceKey _anchorFloatingWindowTitleBarButtonGlyphHover;
        private static ComponentResourceKey _anchorFloatingWindowTitleBarButtonGlyphPressed;

        public static ComponentResourceKey AnchorFloatingWindowTitleBarButtonGlyph
            =>
                _anchorFloatingWindowTitleBarButtonGlyph ??
                (_anchorFloatingWindowTitleBarButtonGlyph =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorFloatingWindowTitleBarButtonGlyph"));

        public static ComponentResourceKey AnchorFloatingWindowTitleBarButtonGlyphHover
            =>
                _anchorFloatingWindowTitleBarButtonGlyphHover ??
                (_anchorFloatingWindowTitleBarButtonGlyphHover =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorFloatingWindowTitleBarButtonGlyphHover"));

        public static ComponentResourceKey AnchorFloatingWindowTitleBarButtonGlyphPressed
            =>
                _anchorFloatingWindowTitleBarButtonGlyphPressed ??
                (_anchorFloatingWindowTitleBarButtonGlyphPressed =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorFloatingWindowTitleBarButtonGlyphPressed"));


        public static ComponentResourceKey DocumentTabGlyphInactive
            =>
                _documentTabGlyphInactive ??
                (_documentTabGlyphInactive =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentTabGlyphInactive"));

        public static ComponentResourceKey DocumentTabGlyphHot
            =>
                _documentTabGlyphHot ??
                (_documentTabGlyphHot =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentTabGlyphHot"));



        public static ComponentResourceKey DocWellOverflowButtonGlyph
            =>
                _docWellOverflowButtonGlyph ??
                (_docWellOverflowButtonGlyph =
                    new ComponentResourceKey(typeof(DockingColors), "DocWellOverflowButtonGlyph"));

        public static ComponentResourceKey DocWellOverflowButtonMouseOverBackground
            =>
                _docWellOverflowButtonMouseOverBackground ??
                (_docWellOverflowButtonMouseOverBackground =
                    new ComponentResourceKey(typeof(DockingColors), "DocWellOverflowButtonMouseOverBackground"));

        public static ComponentResourceKey DocWellOverflowButtonMouseOverBorder
            =>
                _docWellOverflowButtonMouseOverBorder ??
                (_docWellOverflowButtonMouseOverBorder =
                    new ComponentResourceKey(typeof(DockingColors), "DocWellOverflowButtonMouseOverBorder"));

        public static ComponentResourceKey DocWellOverflowButtonMouseOverGlyph
            =>
                _docWellOverflowButtonMouseOverGlyph ??
                (_docWellOverflowButtonMouseOverGlyph =
                    new ComponentResourceKey(typeof(DockingColors), "DocWellOverflowButtonMouseOverGlyph"));

        public static ComponentResourceKey DocWellOverflowButtonMouseDownBackground
            =>
                _docWellOverflowButtonMouseDownBackground ??
                (_docWellOverflowButtonMouseDownBackground =
                    new ComponentResourceKey(typeof(DockingColors), "DocWellOverflowButtonMouseDownBackground"));

        public static ComponentResourceKey DocWellOverflowButtonMouseDownBorder
            =>
                _docWellOverflowButtonMouseDownBorder ??
                (_docWellOverflowButtonMouseDownBorder =
                    new ComponentResourceKey(typeof(DockingColors), "DocWellOverflowButtonMouseDownBorder"));

        public static ComponentResourceKey DocWellOverflowButtonMouseDownGlyph
            =>
                _docWellOverflowButtonMouseDownGlyph ??
                (_docWellOverflowButtonMouseDownGlyph =
                    new ComponentResourceKey(typeof(DockingColors), "DocWellOverflowButtonMouseDownGlyph"));

        public static ComponentResourceKey DocumentPaneControlBackground
            =>
                _documentPaneControlBackground ??
                (_documentPaneControlBackground =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentPaneControlBackground"));

        public static ComponentResourceKey DocumentPaneControlBackgroundLastActive
            =>
                _documentPaneControlBackgroundLastActive ??
                (_documentPaneControlBackgroundLastActive =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentPaneControlBackgroundLastActive"));

        public static ComponentResourceKey DocumentPaneControlFileTabBorderInactive
            =>
                _documentPaneControlFileTabBorderInactive ??
                (_documentPaneControlFileTabBorderInactive =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentPaneControlFileTabBorderInactive"));

        public static ComponentResourceKey DocumentPaneControlFileTabBorder
            =>
                _documentPaneControlFileTabBorder ??
                (_documentPaneControlFileTabBorder =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentPaneControlFileTabBorder"));

        public static ComponentResourceKey DocumentPaneControlGlyph
            =>
                _documentPaneControlGlyph ??
                (_documentPaneControlGlyph =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentPaneControlGlyph"));

        public static ComponentResourceKey DocumentPaneControlGlyphHover
            =>
                _documentPaneControlGlyphHover ??
                (_documentPaneControlGlyphHover =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentPaneControlGlyphHover"));

        public static ComponentResourceKey DocumentPaneControlGlyphDown
            =>
                _documentPaneControlGlyphDown ??
                (_documentPaneControlGlyphDown =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentPaneControlGlyphDown"));

        public static ComponentResourceKey DocumentPaneControlButtonBackground
            =>
                _documentPaneControlButtonBackground ??
                (_documentPaneControlButtonBackground =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentPaneControlButtonBackground"));

        public static ComponentResourceKey DocumentPaneControlButtonBackgroundHover
            =>
                _documentPaneControlButtonBackgroundHover ??
                (_documentPaneControlButtonBackgroundHover =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentPaneControlButtonBackgroundHover"));

        public static ComponentResourceKey DocumentPaneControlButtonBackgroundDown
            =>
                _documentPaneControlButtonBackgroundDown ??
                (_documentPaneControlButtonBackgroundDown =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentPaneControlButtonBackgroundDown"));

        public static ComponentResourceKey DocumentPaneControlButtonBorder
            =>
                _documentPaneControlButtonBorder ??
                (_documentPaneControlButtonBorder =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentPaneControlButtonBorder"));

        public static ComponentResourceKey DocumentPaneControlButtonBorderHover
            =>
                _documentPaneControlButtonBorderHover ??
                (_documentPaneControlButtonBorderHover =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentPaneControlButtonBorderHover"));

        public static ComponentResourceKey DocumentPaneControlButtonBorderDown
            =>
                _documentPaneControlButtonBorderDown ??
                (_documentPaneControlButtonBorderDown =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentPaneControlButtonBorderDown"));

        public static ComponentResourceKey DocumentPaneControlBorder
            =>
                _documentPaneControlBorder ??
                (_documentPaneControlBorder = new ComponentResourceKey(typeof(DockingColors), "DocumentPaneControlBorder"));

        public static ComponentResourceKey OverlayWindowPreviewBoxBackground
            =>
                _overlayWindowPreviewBoxBackground ??
                (_overlayWindowPreviewBoxBackground =
                    new ComponentResourceKey(typeof(DockingColors), "OverlayWindowPreviewBoxBackground"));

        public static ComponentResourceKey OverlayWindowPreviewBoxBorder
            =>
                _overlayWindowPreviewBoxBorder ??
                (_overlayWindowPreviewBoxBorder =
                    new ComponentResourceKey(typeof(DockingColors), "OverlayWindowPreviewBoxBorder"));

        public static ComponentResourceKey NavigatorWindowBackground
    =>
        _navigatorWindowBackground ??
        (_navigatorWindowBackground = new ComponentResourceKey(typeof(DockingColors), "NavigatorWindowBackground"));

        public static ComponentResourceKey NavigatorWindowBorder
            =>
                _navigatorWindowBorder ??
                (_navigatorWindowBorder = new ComponentResourceKey(typeof(DockingColors), "NavigatorWindowBorder"));

        public static ComponentResourceKey NavigatorWindowTextForeground
            =>
                _navigatorWindowTextForeground ??
                (_navigatorWindowTextForeground = new ComponentResourceKey(typeof(DockingColors), "NavigatorWindowTextForeground"));

        public static ComponentResourceKey NavigatorWindowItemBackground
            =>
                _navigatorWindowItemBackground ??
                (_navigatorWindowItemBackground = new ComponentResourceKey(typeof(DockingColors), "NavigatorWindowItemBackground"));

        public static ComponentResourceKey NavigatorWindowItemForeground
            =>
                _navigatorWindowItemForeground ??
                (_navigatorWindowItemForeground = new ComponentResourceKey(typeof(DockingColors), "NavigatorWindowItemForeground"));

        public static ComponentResourceKey NavigatorWindowItemBackgroundChecked
            =>
                _navigatorWindowItemBackgroundChecked ??
                (_navigatorWindowItemBackgroundChecked = new ComponentResourceKey(typeof(DockingColors), "NavigatorWindowItemBackgroundChecked"));

        public static ComponentResourceKey NavigatorWindowItemForegroundChecked
            =>
                _navigatorWindowItemForegroundChecked ??
                (_navigatorWindowItemForegroundChecked = new ComponentResourceKey(typeof(DockingColors), "NavigatorWindowItemForegroundChecked"));

        public static ComponentResourceKey NavigatorWindowTitleText
    =>
        _navigatorWindowTitleText ??
        (_navigatorWindowTitleText = new ComponentResourceKey(typeof(DockingColors), "NavigatorWindowTitleText"));

        public static ComponentResourceKey NavigatorWindowItemBorder
            =>
                _navigatorWindowItemBorder ??
                (_navigatorWindowItemBorder = new ComponentResourceKey(typeof(DockingColors), "NavigatorWindowItemBorder"));

        public static ComponentResourceKey NavigatorWindowItemBorderChecked
            =>
                _navigatorWindowItemBorderChecked ??
                (_navigatorWindowItemBorderChecked = new ComponentResourceKey(typeof(DockingColors), "NavigatorWindowItemBorderChecked"));

        public static ComponentResourceKey LayoutGridResizerBackground
            =>
                _layoutGridResizerBackground ??
                (_layoutGridResizerBackground =
                    new ComponentResourceKey(typeof(DockingColors), "LayoutGridResizerBackground"));

        public static ComponentResourceKey LayoutGridResizerBackgroundHorizontal
            =>
                _layoutGridResizerBackgroundHorizontal ??
                (_layoutGridResizerBackgroundHorizontal =
                    new ComponentResourceKey(typeof(DockingColors), "LayoutGridResizerBackgroundHorizontal"));

        public static ComponentResourceKey DocumentTabItemText
    =>
        _documentTabItemText ??
        (_documentTabItemText = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemText"));

        public static ComponentResourceKey DocumentTabItemTextHover
            =>
                _documentTabItemTextHover ??
                (_documentTabItemTextHover
            = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemTextHover"));

        public static ComponentResourceKey DocumentTabItemTextActive
            =>
                _documentTabItemTextActive ??
                (_documentTabItemTextActive = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemTextActive"));

        public static ComponentResourceKey DocumentTabItemTextLastActive
            =>
                _documentTabItemTextLastActive ??
                (_documentTabItemTextLastActive = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemTextLastActive"));

        public static ComponentResourceKey DocumentTabItemGlyph
            =>
                _documentTabItemGlyph ??
                (_documentTabItemGlyph = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemGlyph"));

        public static ComponentResourceKey DocumentTabItemGlyphHover
            =>
                _documentTabItemGlyphHover ??
                (_documentTabItemGlyphHover = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemGlyphHover"));

        public static ComponentResourceKey DocumentTabItemGlyphDown
            =>
                _documentTabItemGlyphDown ??
                (_documentTabItemGlyphDown = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemGlyphDown"));

        public static ComponentResourceKey DocumentTabItemGlyphActive
            =>
                _documentTabItemGlyphActive ??
                (_documentTabItemGlyphActive = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemGlyphActive"));

        public static ComponentResourceKey DocumentTabItemGlyphActiveHover
            =>
                _documentTabItemGlyphActiveHover ??
                (_documentTabItemGlyphActiveHover = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemGlyphActiveHover"));

        public static ComponentResourceKey DocumentTabItemGlyphActiveDown
            =>
                _documentTabItemGlyphActiveDown ??
                (_documentTabItemGlyphActiveDown = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemGlyphActiveDown"));

        public static ComponentResourceKey DocumentTabItemGlyphLastActive
            =>
                _documentTabItemGlyphLastActive ??
                (_documentTabItemGlyphLastActive = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemGlyphLastActive"));

        public static ComponentResourceKey DocumentTabItemGlyphLastActiveHover
            =>
                _documentTabItemGlyphLastActiveHover ??
                (_documentTabItemGlyphLastActiveHover = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemGlyphLastActiveHover"));

        public static ComponentResourceKey DocumentTabItemGlyphLastActiveDown
            =>
                _documentTabItemGlyphLastActiveDown ??
                (_documentTabItemGlyphLastActiveDown = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemGlyphLastActiveDown"));

        public static ComponentResourceKey DocumentTabItemButtonBackground
           =>
               _documentTabItemButtonBackground ??
               (_documentTabItemButtonBackground =
                   new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBackground"));

        public static ComponentResourceKey DocumentTabItemButtonBackgroundHover
            =>
                _documentTabItemButtonBackgroundHover ??
                (_documentTabItemButtonBackgroundHover =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBackgroundHover"));

        public static ComponentResourceKey DocumentTabItemButtonBackgroundDown
            =>
                _documentTabItemButtonBackgroundDown ??
                (_documentTabItemButtonBackgroundDown =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBackgroundDown"));

        public static ComponentResourceKey DocumentTabItemButtonBackgroundActive
            =>
                _documentTabItemButtonBackgroundActive ??
                (_documentTabItemButtonBackgroundActive =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBackgroundActive"));

        public static ComponentResourceKey DocumentTabItemButtonBackgroundActiveHover
            =>
                _documentTabItemButtonBackgroundActiveHover ??
                (_documentTabItemButtonBackgroundActiveHover =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBackgroundActiveHover"));

        public static ComponentResourceKey DocumentTabItemButtonBackgroundActiveDown
            =>
                _documentTabItemButtonBackgroundActiveDown ??
                (_documentTabItemButtonBackgroundActiveDown =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBackgroundActiveDown"));

        public static ComponentResourceKey DocumentTabItemButtonBackgroundLastActive
    =>
        _documentTabItemButtonBackgroundLastActive ??
        (_documentTabItemButtonBackgroundLastActive =
            new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBackgroundLastActive"));

        public static ComponentResourceKey DocumentTabItemButtonBackgroundLastActiveHover
            =>
                _documentTabItemButtonBackgroundLastActiveHover ??
                (_documentTabItemButtonBackgroundLastActiveHover =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBackgroundLastActiveHover"));

        public static ComponentResourceKey DocumentTabItemButtonBackgroundLastActiveDown
            =>
                _documentTabItemButtonBackgroundLastActiveDown ??
                (_documentTabItemButtonBackgroundLastActiveDown =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBackgroundLastActiveDown"));

        public static ComponentResourceKey DocumentTabItemButtonBorder
    =>
        _documentTabItemButtonBorder ??
        (_documentTabItemButtonBorder =
            new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBorder"));

        public static ComponentResourceKey DocumentTabItemButtonBorderHover
            =>
                _documentTabItemButtonBorderHover ??
                (_documentTabItemButtonBorderHover =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBorderHover"));

        public static ComponentResourceKey DocumentTabItemButtonBorderDown
            =>
                _documentTabItemButtonBorderDown ??
                (_documentTabItemButtonBorderDown =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBorderDown"));


        public static ComponentResourceKey DocumentTabItemButtonBorderActive
            =>
                _documentTabItemButtonBorderActive ??
                (_documentTabItemButtonBorderActive =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBorderActive"));

        public static ComponentResourceKey DocumentTabItemButtonBorderActiveHover
            =>
                _documentTabItemButtonBorderActiveHover ??
                (_documentTabItemButtonBorderActiveHover =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBorderActiveHover"));

        public static ComponentResourceKey DocumentTabItemButtonBorderActiveDown
            =>
                _documentTabItemButtonBorderActiveDown ??
                (_documentTabItemButtonBorderActiveDown =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBorderActiveDown"));


        public static ComponentResourceKey DocumentTabItemButtonBorderLastActive
    =>
        _documentTabItemButtonBorderLastActive ??
        (_documentTabItemButtonBorderLastActive =
            new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBorderLastActive"));

        public static ComponentResourceKey DocumentTabItemButtonBorderLastActiveHover
            =>
                _documentTabItemButtonBorderLastActiveHover ??
                (_documentTabItemButtonBorderLastActiveHover =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBorderLastActiveHover"));

        public static ComponentResourceKey DocumentTabItemButtonBorderLastActiveDown
            =>
                _documentTabItemButtonBorderLastActiveDown ??
                (_documentTabItemButtonBorderLastActiveDown =
                    new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemButtonBorderLastActiveDown"));

        public static ComponentResourceKey DocumentTabItemBackground
            =>
                _documentTabItemBackground ??
                (_documentTabItemBackground = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemBackground"));

        public static ComponentResourceKey DocumentTabItemBackgroundHover
            =>
                _documentTabItemBackgroundHover ??
                (_documentTabItemBackgroundHover = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemBackgroundHover"));

        public static ComponentResourceKey DocumentTabItemBackgroundActive
            =>
                _documentTabItemBackgroundActive ??
                (_documentTabItemBackgroundActive = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemBackgroundActive"));

        public static ComponentResourceKey DocumentTabItemBackgroundLastActive
            =>
                _documentTabItemBackgroundLastActive ??
                (_documentTabItemBackgroundLastActive = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemBackgroundLastActive"));

        public static ComponentResourceKey DocumentTabItemBackgroundDisabled
            =>
                _documentTabItemBackgroundDisabled ??
                (_documentTabItemBackgroundDisabled = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemBackgroundDisabled"));

        public static ComponentResourceKey DocumentTabItemBorder
           =>
               _documentTabItemBorder ??
               (_documentTabItemBorder = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemBorder"));

        public static ComponentResourceKey DocumentTabItemBorderHover
            =>
                _documentTabItemBorderHover ??
                (_documentTabItemBorderHover = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemBorderHover"));

        public static ComponentResourceKey DocumentTabItemBorderActive
            =>
                _documentTabItemBorderActive ??
                (_documentTabItemBorderActive = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemBorderActive"));

        public static ComponentResourceKey DocumentTabItemBorderLastActive
            =>
                _documentTabItemBorderLastActive ??
                (_documentTabItemBorderLastActive = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemBorderLastActive"));

        public static ComponentResourceKey DocumentTabItemBorderDisabled
            =>
                _documentTabItemBorderDisabled ??
                (_documentTabItemBorderDisabled = new ComponentResourceKey(typeof(DockingColors), "DocumentTabItemBorderDisabled"));

        public static ComponentResourceKey FloatingWindowBackground
           =>
               _floatingWindowBackground ??
               (_floatingWindowBackground = new ComponentResourceKey(typeof(DockingColors), "FloatingWindowBackground"));


        public static ComponentResourceKey FloatingWindowTitleBarBackground
            =>
                _floatingWindowTitleBarBackground ??
                (_floatingWindowTitleBarBackground = new ComponentResourceKey(typeof(DockingColors), "FloatingWindowTitleBarBackground"));

        public static ComponentResourceKey FloatingWindowTitleBarGlyph
            =>
                _floatingWindowTitleBarGlyph ??
                (_floatingWindowTitleBarGlyph = new ComponentResourceKey(typeof(DockingColors), "FloatingWindowTitleBarGlyph"));

        public static ComponentResourceKey FloatingWindowTitleBarGlyphHover
            =>
                _floatingWindowTitleBarGlyphHover ??
                (_floatingWindowTitleBarGlyphHover = new ComponentResourceKey(typeof(DockingColors), "FloatingWindowTitleBarGlyphHover"));

        public static ComponentResourceKey FloatingWindowTitleBarGlyphDown
            =>
                _floatingWindowTitleBarGlyphDown ??
                (_floatingWindowTitleBarGlyphDown = new ComponentResourceKey(typeof(DockingColors), "FloatingWindowTitleBarGlyphDown"));

        public static ComponentResourceKey FloatingWindowTitleBarForeground
            =>
                _floatingWindowTitleBarForeground ??
                (_floatingWindowTitleBarForeground = new ComponentResourceKey(typeof(DockingColors), "FloatingWindowTitleBarForeground"));

        public static ComponentResourceKey FloatingWindowTitleBarForegroundActive
            =>
                _floatingWindowTitleBarForegroundActive ??
                (_floatingWindowTitleBarForegroundActive = new ComponentResourceKey(typeof(DockingColors), "FloatingWindowTitleBarForegroundActive"));

        public static ComponentResourceKey LayoutAutoHideWindowBackground
            =>
                _layoutAutoHideWindowBackground ?? (_layoutAutoHideWindowBackground =
                    (new ComponentResourceKey(typeof(DockingColors), "LayoutAutoHideWindowBackground")));

        public static ComponentResourceKey LayoutAutoHideWindowBorder
            =>
                _layoutAutoHideWindowBorder ??
                (_layoutAutoHideWindowBorder =
                    new ComponentResourceKey(typeof(DockingColors), "LayoutAutoHideWindowBorder"));

        public static ComponentResourceKey AnchorPaneControlBorder
            =>
                _anchorPaneControlBorder ??
                (_anchorPaneControlBorder = new ComponentResourceKey(typeof(DockingColors), "AnchorPaneControlBorder"));

        public static ComponentResourceKey AnchorTabItemBackground
            =>
                _anchorTabItemBackground ??
                (_anchorTabItemBackground = new ComponentResourceKey(typeof(DockingColors), "AnchorTabItemBackground"));

        public static ComponentResourceKey AnchorTabItemBackgroundHover
            =>
                _anchorTabItemBackgroundHover ??
                (_anchorTabItemBackgroundHover = new ComponentResourceKey(typeof(DockingColors), "AnchorTabItemBackgroundHover"));

        public static ComponentResourceKey AnchorTabItemBackgroundActive
            =>
                _anchorTabItemBackgroundActive ??
                (_anchorTabItemBackgroundActive = new ComponentResourceKey(typeof(DockingColors), "AnchorTabItemBackgroundActive"));

        public static ComponentResourceKey AnchorTabItemBackgroundDisabled
            =>
                _anchorTabItemBackgroundDisabled ??
                (_anchorTabItemBackgroundDisabled = new ComponentResourceKey(typeof(DockingColors), "AnchorTabItemBackgroundDisabled"));

        public static ComponentResourceKey AnchorTabItemBorder
           =>
               _anchorTabItemBorder ??
               (_anchorTabItemBorder = new ComponentResourceKey(typeof(DockingColors), "AnchorTabItemBorder"));

        public static ComponentResourceKey AnchorTabItemBorderHover
           =>
               _anchorTabItemBorderHover ??
               (_anchorTabItemBorderHover = new ComponentResourceKey(typeof(DockingColors), "AnchorTabItemBorderHover"));

        public static ComponentResourceKey AnchorTabItemBorderActive
           =>
               _anchorTabItemBorderActive ??
               (_anchorTabItemBorderActive = new ComponentResourceKey(typeof(DockingColors), "AnchorTabItemBorderActive"));

        public static ComponentResourceKey AnchorTabItemBorderDisabled
           =>
               _anchorTabItemBorderDisabled ??
               (_anchorTabItemBorderDisabled = new ComponentResourceKey(typeof(DockingColors), "AnchorTabItemBorderDisabled"));

        public static ComponentResourceKey AnchorTabItemText
           =>
               _anchorTabItemText ??
               (_anchorTabItemText = new ComponentResourceKey(typeof(DockingColors), "AnchorTabItemText"));

        public static ComponentResourceKey AnchorTabItemTextHover
           =>
               _anchorTabItemTextHover ??
               (_anchorTabItemTextHover = new ComponentResourceKey(typeof(DockingColors), "AnchorTabItemTextHover"));

        public static ComponentResourceKey AnchorTabItemTextActive
           =>
               _anchorTabItemTextActive ??
               (_anchorTabItemTextActive = new ComponentResourceKey(typeof(DockingColors), "AnchorTabItemTextActive"));

        public static ComponentResourceKey AnchorTabItemTextDisabled
           =>
               _anchorTabItemTextDisabled ??
               (_anchorTabItemTextDisabled = new ComponentResourceKey(typeof(DockingColors), "AnchorTabItemTextDisabled"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarBackground
            =>
                _anchorableFloatingWindowTitleBarBackground ??
                (_anchorableFloatingWindowTitleBarBackground =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarBackground"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarBackgroundActive
            =>
                _anchorableFloatingWindowTitleBarBackgroundActive ??
                (_anchorableFloatingWindowTitleBarBackgroundActive =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarBackgroundActive"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarGlyph
            =>
                _anchorableFloatingWindowTitleBarGlyph ??
                (_anchorableFloatingWindowTitleBarGlyph =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarGlyph"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarGlyphActive
            =>
                _anchorableFloatingWindowTitleBarGlyphActive ??
                (_anchorableFloatingWindowTitleBarGlyphActive =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarGlyphActive"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarGlyphHover
            =>
                _anchorableFloatingWindowTitleBarGlyphHover ??
                (_anchorableFloatingWindowTitleBarGlyphHover =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarGlyphHover"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarGlyphDown
            =>
                _anchorableFloatingWindowTitleBarGlyphDown ??
                (_anchorableFloatingWindowTitleBarGlyphDown =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarGlyphDown"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarGlyphActiveHover
            =>
                _anchorableFloatingWindowTitleBarGlyphActiveHover ??
                (_anchorableFloatingWindowTitleBarGlyphActiveHover =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarGlyphActiveHover"));



        public static ComponentResourceKey AnchorableFloatingWindowTitleBarGlyphActiveDown
            =>
                _anchorableFloatingWindowTitleBarGlyphActiveDown ??
                (_anchorableFloatingWindowTitleBarGlyphActiveDown =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarGlyphActiveDown"));

        public static ComponentResourceKey AnchorFloatingWindowTitleBarButtonBackground
            =>
                _anchorFloatingWindowTitleBarButtonBackground ??
                (_anchorFloatingWindowTitleBarButtonBackground =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorFloatingWindowTitleBarButtonBackground"));

        public static ComponentResourceKey AnchorFloatingWindowTitleBarButtonBorder
            =>
                _anchorFloatingWindowTitleBarButtonBorder ??
                (_anchorFloatingWindowTitleBarButtonBorder =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorFloatingWindowTitleBarButtonBorder"));

        public static ComponentResourceKey AnchorFloatingWindowTitleBarButtonBackgroundHover
            =>
                _anchorFloatingWindowTitleBarButtonBackgroundHover ??
                (_anchorFloatingWindowTitleBarButtonBackgroundHover =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorFloatingWindowTitleBarButtonBackgroundHover"));

        public static ComponentResourceKey AnchorFloatingWindowTitleBarButtonBorderHover
            =>
                _anchorFloatingWindowTitleBarButtonBorderHover ??
                (_anchorFloatingWindowTitleBarButtonBorderHover =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorFloatingWindowTitleBarButtonBorderHover"));

        public static ComponentResourceKey AnchorFloatingWindowTitleBarButtonBackgroundPressed
            =>
                _anchorFloatingWindowTitleBarButtonBackgroundPressed ??
                (_anchorFloatingWindowTitleBarButtonBackgroundPressed =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorFloatingWindowTitleBarButtonBackgroundPressed"));

        public static ComponentResourceKey AnchorFloatingWindowTitleBarButtonBorderPressed
            =>
                _anchorFloatingWindowTitleBarButtonBorderPressed ??
                (_anchorFloatingWindowTitleBarButtonBorderPressed =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorFloatingWindowTitleBarButtonBorderPressed"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarGlyphDisabled
            =>
                _anchorableFloatingWindowTitleBarGlyphDisabled ??
                (_anchorableFloatingWindowTitleBarGlyphDisabled =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarGlyphDisabled"));








        public static ComponentResourceKey AnchorableFloatingWindowTitleBarForeground
            =>
                _anchorableFloatingWindowTitleBarForeground ??
                (_anchorableFloatingWindowTitleBarForeground =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarForeground"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarForegroundActive
            =>
                _anchorableFloatingWindowTitleBarForegroundActive ??
                (_anchorableFloatingWindowTitleBarForegroundActive =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarForegroundActive"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBackground
            =>
                _anchorableFloatingWindowTitleBarButtonBackground ??
                (_anchorableFloatingWindowTitleBarButtonBackground =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarButtonBackground"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBackgroundHover
            =>
                _anchorableFloatingWindowTitleBarButtonBackgroundHover ??
                (_anchorableFloatingWindowTitleBarButtonBackgroundHover
            =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarButtonBackgroundHover"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBackgroundDown
            =>
                _anchorableFloatingWindowTitleBarButtonBackgroundDown ??
                (_anchorableFloatingWindowTitleBarButtonBackgroundDown =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarButtonBackgroundDown"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBorder
            =>
                _anchorableFloatingWindowTitleBarButtonBorder ??
                (_anchorableFloatingWindowTitleBarButtonBorder =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarButtonBorder"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBorderHover
            =>
                _anchorableFloatingWindowTitleBarButtonBorderHover ??
                (_anchorableFloatingWindowTitleBarButtonBorderHover =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarButtonBorderHover"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBorderDown
            =>
                _anchorableFloatingWindowTitleBarButtonBorderDown ??
                (_anchorableFloatingWindowTitleBarButtonBorderDown =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarButtonBorderDown"));

        public static ComponentResourceKey AnchorableFloatingWindowGrip
            =>
                _anchorableFloatingWindowGrip ??
                (_anchorableFloatingWindowGrip =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowGrip"));

        public static ComponentResourceKey AnchorableFloatingWindowGripActive
            =>
                _anchorableFloatingWindowGripActive ??
                (_anchorableFloatingWindowGripActive =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowGripActive"));


        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBackgroundActive
            =>
                _anchorableFloatingWindowTitleBarButtonBackgroundActive ??
                (_anchorableFloatingWindowTitleBarButtonBackgroundActive =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarButtonBackgroundActive"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBackgroundActiveHover
            =>
                _anchorableFloatingWindowTitleBarButtonBackgroundActiveHover ??
                (_anchorableFloatingWindowTitleBarButtonBackgroundActiveHover =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarButtonBackgroundActiveHover"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBackgroundActiveDown
            =>
                _anchorableFloatingWindowTitleBarButtonBackgroundActiveDown ??
                (_anchorableFloatingWindowTitleBarButtonBackgroundActiveDown =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarButtonBackgroundActiveDown"));


        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBorderActive
            =>
                _anchorableFloatingWindowTitleBarButtonBorderActive ??
                (_anchorableFloatingWindowTitleBarButtonBorderActive =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarButtonBorderActive"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBorderActiveHover
            =>
                _anchorableFloatingWindowTitleBarButtonBorderActiveHover ??
                (_anchorableFloatingWindowTitleBarButtonBorderActiveHover =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarButtonBorderActiveHover"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBorderActiveDown
            =>
                _anchorableFloatingWindowTitleBarButtonBorderActiveDown ??
                (_anchorableFloatingWindowTitleBarButtonBorderActiveDown =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableFloatingWindowTitleBarButtonBorderActiveDown"));

        public static ComponentResourceKey DockingManagerBackground
            =>
                _dockingManagerBackground ??
                (_dockingManagerBackground = new ComponentResourceKey(typeof(DockingColors), "DockingManagerBackground"));

        public static ComponentResourceKey AnchorPaneTitleBackgroundActive
                    =>
                        _anchorPaneTitleBackgroundActive ??
                        (_anchorPaneTitleBackgroundActive = new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleBackgroundActive"));

        public static ComponentResourceKey AnchorPaneTitleGlyph
            =>
                _anchorPaneTitleGlyph ??
                (_anchorPaneTitleGlyph = new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleGlyph"));

        public static ComponentResourceKey AnchorPaneTitleGlyphHover
            =>
                _anchorPaneTitleGlyphHover ??
                (_anchorPaneTitleGlyphHover = new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleGlyphHover"));

        public static ComponentResourceKey AnchorPaneTitleGlyphDown
            =>
                _anchorPaneTitleGlyphDown ??
                (_anchorPaneTitleGlyphDown = new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleGlyphDown"));

        public static ComponentResourceKey AnchorPaneTitleGlyphActive
           =>
               _anchorPaneTitleGlyphActive ??
               (_anchorPaneTitleGlyphActive = new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleGlyphActive"));

        public static ComponentResourceKey AnchorPaneTitleGlyphActiveHover
            =>
                _anchorPaneTitleGlyphActiveHover ??
                (_anchorPaneTitleGlyphActiveHover = new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleGlyphActiveHover"));

        public static ComponentResourceKey AnchorPaneTitleGlyphActiveDown
            =>
                _anchorPaneTitleGlyphActiveDown ??
                (_anchorPaneTitleGlyphActiveDown = new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleGlyphActiveDown"));

        public static ComponentResourceKey AnchorPaneTitleButtonBackground
            =>
                _anchorPaneTitleButtonBackground ??
                (_anchorPaneTitleButtonBackground =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleButtonBackground"));

        public static ComponentResourceKey AnchorPaneTitleButtonBackgroundHover
            =>
                _anchorPaneTitleButtonBackgroundHover ??
                (_anchorPaneTitleButtonBackgroundHover =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleButtonBackgroundHover"));

        public static ComponentResourceKey AnchorPaneTitleButtonBackgroundDown
            =>
                _anchorPaneTitleButtonBackgroundDown ??
                (_anchorPaneTitleButtonBackgroundDown =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleButtonBackgroundDown"));

        public static ComponentResourceKey AnchorPaneTitleButtonBackgroundActive
            =>
                _anchorPaneTitleButtonBackgroundActive ??
                (_anchorPaneTitleButtonBackgroundActive =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleButtonBackground"));

        public static ComponentResourceKey AnchorPaneTitleButtonBackgroundActiveHover
            =>
                _anchorPaneTitleButtonBackgroundActiveHover ??
                (_anchorPaneTitleButtonBackgroundActiveHover =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleButtonBackgroundActiveHover"));

        public static ComponentResourceKey AnchorPaneTitleButtonBackgroundActiveDown
            =>
                _anchorPaneTitleButtonBackgroundActiveDown ??
                (_anchorPaneTitleButtonBackgroundActiveDown =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleButtonBackgroundActiveDown"));

        public static ComponentResourceKey AnchorPaneTitleButtonBorder
            =>
                _anchorPaneTitleButtonBorder ??
                (_anchorPaneTitleButtonBorder =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleButtonBorder"));

        public static ComponentResourceKey AnchorPaneTitleButtonBorderHover
            =>
                _anchorPaneTitleButtonBorderHover ??
                (_anchorPaneTitleButtonBorderHover =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleButtonBorderHover"));

        public static ComponentResourceKey AnchorPaneTitleButtonBorderDown
            =>
                _anchorPaneTitleButtonBorderDown ??
                (_anchorPaneTitleButtonBorderDown =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleButtonBorderDown"));

        public static ComponentResourceKey AnchorPaneTitleButtonBorderActive
            =>
                _anchorPaneTitleButtonBorderActive ??
                (_anchorPaneTitleButtonBorderActive =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleButtonBorderActive"));

        public static ComponentResourceKey AnchorPaneTitleButtonBorderActiveHover
            =>
                _anchorPaneTitleButtonBorderActiveHover ??
                (_anchorPaneTitleButtonBorderActiveHover =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleButtonBorderActiveHover"));

        public static ComponentResourceKey AnchorPaneTitleButtonBorderActiveDown
            =>
                _anchorPaneTitleButtonBorderActiveDown ??
                (_anchorPaneTitleButtonBorderActiveDown =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleButtonBorderActiveDown"));

        public static ComponentResourceKey AnchorPaneTitleGrip
            =>
                _anchorPaneTitleGrip ??
                (_anchorPaneTitleGrip = new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleGrip"));

        public static ComponentResourceKey AnchorPaneTitleGripActive
            =>
                _anchorPaneTitleGripActive ??
                (_anchorPaneTitleGripActive = new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleGripActive"));

        public static ComponentResourceKey AnchorPaneTitleText
            =>
                _anchorPaneTitleText ??
                (_anchorPaneTitleText = new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleText"));

        public static ComponentResourceKey AnchorPaneTitleTextActive
            =>
                _anchorPaneTitleTextActive ??
                (_anchorPaneTitleTextActive = new ComponentResourceKey(typeof(DockingColors), "AnchorPaneTitleTextActive"));

        public static ComponentResourceKey AnchorSideItemBackground
            =>
                _anchorSideItemBackground ??
                (_anchorSideItemBackground = new ComponentResourceKey(typeof(DockingColors), "AnchorSideItemBackground"));

        public static ComponentResourceKey AnchorSideItemBackgroundHover
            =>
                _anchorSideItemBackgroundHover ??
                (_anchorSideItemBackgroundHover = new ComponentResourceKey(typeof(DockingColors), "AnchorSideItemBackgroundHover"));

        public static ComponentResourceKey AnchorSideItemBorder
            =>
                _anchorSideItemBorder ??
                (_anchorSideItemBorder = new ComponentResourceKey(typeof(DockingColors), "AnchorSideItemBorder"));

        public static ComponentResourceKey AnchorSideItemBorderHover
            =>
                _anchorSideItemBorderHover ??
                (_anchorSideItemBorderHover = new ComponentResourceKey(typeof(DockingColors), "AnchorSideItemBorderHover"));

        public static ComponentResourceKey AnchorSideItemForeground
            =>
                _anchorSideItemForeground ??
                (_anchorSideItemForeground = new ComponentResourceKey(typeof(DockingColors), "AnchorSideItemForeground"));
        public static ComponentResourceKey AnchorSideItemForegroundHover
            =>
                _anchorSideItemForegroundHover ??
                (_anchorSideItemForegroundHover = new ComponentResourceKey(typeof(DockingColors), "AnchorSideItemForegroundHover"));

        public static ComponentResourceKey AnchorableControlBackground
            =>
                _anchorableControlBackground ??
                (_anchorableControlBackground =
                    new ComponentResourceKey(typeof(DockingColors), "AnchorableControlBackground"));

    }
}
