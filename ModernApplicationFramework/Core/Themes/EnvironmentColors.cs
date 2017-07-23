using System.Windows;

namespace ModernApplicationFramework.Core.Themes
{
    /// <summary>
    /// Set of all available colors
    /// </summary>
    public static class EnvironmentColors
    {
        //Global
        private static ComponentResourceKey _gloablForeground;
        private static ComponentResourceKey _globalBackgroundColor;
        private static ComponentResourceKey _globalBackgroundColor2;

        //Button
        private static ComponentResourceKey _buttonBackground;
        private static ComponentResourceKey _buttonForeground;
        private static ComponentResourceKey _buttonBorder;

        private static ComponentResourceKey _buttonBackgroundHover;
        private static ComponentResourceKey _buttonForegroundHover;
        private static ComponentResourceKey _buttonBorderHover;

        private static ComponentResourceKey _buttonBackgroundDown;
        private static ComponentResourceKey _buttonForegroundDown;
        private static ComponentResourceKey _buttonBorderDown;

        private static ComponentResourceKey _buttonBackgroundDisabled;
        private static ComponentResourceKey _buttonForegroundDisabled;
        private static ComponentResourceKey _buttonBorderDisabled;

        //ComboBox -- New
        private static ComponentResourceKey _comboBoxBackground;
        private static ComponentResourceKey _comboBoxBorder;
        private static ComponentResourceKey _comboBoxButtonMouseDownBackground;
        private static ComponentResourceKey _comboBoxButtonMouseDownSeparator;
        private static ComponentResourceKey _comboBoxButtonMouseOverBackground;
        private static ComponentResourceKey _comboBoxButtonMouseOverSeparator;
        private static ComponentResourceKey _comboBoxDisabledBackground;
        private static ComponentResourceKey _comboBoxDisabledBorder;
        private static ComponentResourceKey _comboBoxDisabledGlyph;
        private static ComponentResourceKey _comboBoxDisabledText;
        private static ComponentResourceKey _comboBoxFocusedBackground;
        private static ComponentResourceKey _comboBoxFocusedBorder;
        private static ComponentResourceKey _comboBoxFocusedButtonBackground;
        private static ComponentResourceKey _comboBoxFocusedButtonSeparator;
        private static ComponentResourceKey _comboBoxFocusedGlyph;
        private static ComponentResourceKey _comboBoxFocusedText;
        private static ComponentResourceKey _comboBoxGlyph;
        private static ComponentResourceKey _comboBoxItemMouseOverBackground;
        private static ComponentResourceKey _comboBoxItemMouseOverBorder;
        private static ComponentResourceKey _comboBoxItemMouseOverText;
        private static ComponentResourceKey _comboBoxItemText;
        private static ComponentResourceKey _comboBoxItemTextInactive;
        private static ComponentResourceKey _comboBoxMouseDownBackground;
        private static ComponentResourceKey _comboBoxMouseDownBorder;
        private static ComponentResourceKey _comboBoxMouseDownGlyph;
        private static ComponentResourceKey _comboBoxMouseDownText;
        private static ComponentResourceKey _comboBoxMouseOverBackgroundBegin;
        private static ComponentResourceKey _comboBoxMouseOverBackgroundEnd;
        private static ComponentResourceKey _comboBoxMouseOverBackgroundMiddle1;
        private static ComponentResourceKey _comboBoxMouseOverBackgroundMiddle2;
        private static ComponentResourceKey _comboBoxMouseOverBorder;
        private static ComponentResourceKey _comboBoxMouseOverGlyph;
        private static ComponentResourceKey _comboBoxMouseOverText;
        private static ComponentResourceKey _comboBoxPopupBackgroundBegin;
        private static ComponentResourceKey _comboBoxPopupBackgroundEnd;
        private static ComponentResourceKey _comboBoxPopupBorder;
        private static ComponentResourceKey _comboBoxSelection;
        private static ComponentResourceKey _comboBoxText;

        //DropDownButton -- New
        private static ComponentResourceKey _dropDownBackground;
        private static ComponentResourceKey _dropDownBorder;
        private static ComponentResourceKey _dropDownButtonMouseDownBackground;
        private static ComponentResourceKey _dropDownButtonMouseDownSeparator;
        private static ComponentResourceKey _dropDownButtonMouseOverBackground;
        private static ComponentResourceKey _dropDownButtonMouseOverSeparator;
        private static ComponentResourceKey _dropDownDisabledBackground;
        private static ComponentResourceKey _dropDownDisabledBorder;
        private static ComponentResourceKey _dropDownDisabledGlyph;
        private static ComponentResourceKey _dropDownDisabledText;
        private static ComponentResourceKey _dropDownGlyph;
        private static ComponentResourceKey _dropDownMouseDownBackground;
        private static ComponentResourceKey _dropDownMouseDownBorder;
        private static ComponentResourceKey _dropDownMouseDownGlyph;
        private static ComponentResourceKey _dropDownMouseDownText;
        private static ComponentResourceKey _dropDownMouseOverBackgroundBegin;
        private static ComponentResourceKey _dropDownMouseOverBackgroundEnd;
        private static ComponentResourceKey _dropDownMouseOverBackgroundMiddle1;
        private static ComponentResourceKey _dropDownMouseOverBackgroundMiddle2;
        private static ComponentResourceKey _dropDownMouseOverBorder;
        private static ComponentResourceKey _dropDownMouseOverGlyph;
        private static ComponentResourceKey _dropDownMouseOverText;
        private static ComponentResourceKey _dropDownPopupBackgroundBegin;
        private static ComponentResourceKey _dropDownPopupBackgroundEnd;
        private static ComponentResourceKey _dropDownPopupBorder;
        private static ComponentResourceKey _dropDownText;
        private static ComponentResourceKey _dropShadowBackground;

        //Separator
        private static ComponentResourceKey _separatorBackground;

        //ScrollBar
        private static ComponentResourceKey _scrollBarArrowBackground;
        private static ComponentResourceKey _scrollBarArrowDisabledBackground;
        private static ComponentResourceKey _scrollBarArrowGlyph;
        private static ComponentResourceKey _scrollBarArrowGlyphDisabled;
        private static ComponentResourceKey _scrollBarArrowGlyphMouseOver;
        private static ComponentResourceKey _scrollBarArrowGlyphPressed;
        private static ComponentResourceKey _scrollBarArrowMouseOverBackground;
        private static ComponentResourceKey _scrollBarArrowPressedBackground;
        private static ComponentResourceKey _scrollBarBackground;
        private static ComponentResourceKey _scrollBarBorder;
        private static ComponentResourceKey _scrollBarDisabledBackground;
        private static ComponentResourceKey _scrollBarThumbBackground;
        private static ComponentResourceKey _scrollBarThumbBorder;
        private static ComponentResourceKey _scrollBarThumbDisabled;
        private static ComponentResourceKey _scrollBarThumbGlyph;
        private static ComponentResourceKey _scrollBarThumbGlyphMouseOverBorder;
        private static ComponentResourceKey _scrollBarThumbGlyphPressedBorder;
        private static ComponentResourceKey _scrollBarThumbMouseOverBackground;
        private static ComponentResourceKey _scrollBarThumbMouseOverBorder;
        private static ComponentResourceKey _scrollBarThumbPressedBackground;
        private static ComponentResourceKey _scrollBarThumbPressedBorder;

        //TextBox
        private static ComponentResourceKey _textBoxBorder;
        private static ComponentResourceKey _textBoxBackground;
        private static ComponentResourceKey _textBoxForeground;
        private static ComponentResourceKey _textBoxSelection;

        private static ComponentResourceKey _textBoxBorderHover;
        private static ComponentResourceKey _textBoxBackgroundHover;
        private static ComponentResourceKey _textBoxForegroundHover;

        private static ComponentResourceKey _textBoxBorderFocused;
        private static ComponentResourceKey _textBoxBackgroundFocused;
        private static ComponentResourceKey _textBoxForegroundFocused;

        private static ComponentResourceKey _textBoxBorderDisabled;
        private static ComponentResourceKey _textBoxBackgroundDisabled;
        private static ComponentResourceKey _textBoxForegroundDisabled;

        //TitleBarButton
        private static ComponentResourceKey _titleBarButtonBackground;
        private static ComponentResourceKey _titleBarButtonBorder;
        private static ComponentResourceKey _titleBarButtonForeground;

        private static ComponentResourceKey _titleBarButtonBackgroundHover;
        private static ComponentResourceKey _titleBarButtonBorderHover;
        private static ComponentResourceKey _titleBarButtonForegroundHover;

        private static ComponentResourceKey _titleBarButtonBackgroundDown;
        private static ComponentResourceKey _titleBarButtonBorderDown;
        private static ComponentResourceKey _titleBarButtonForegroundDown;

        private static ComponentResourceKey _titleBarButtonBackgroundDisabled;
        private static ComponentResourceKey _titleBarButtonBorderDisabled;
        private static ComponentResourceKey _titleBarButtonForegroundDisabled;

        //CheckBox
        private static ComponentResourceKey _checkBoxBackground;
        private static ComponentResourceKey _checkBoxBorder;
        private static ComponentResourceKey _checkBoxGlyph;
        private static ComponentResourceKey _checkBoxText;

        private static ComponentResourceKey _checkBoxBackgroundHover;
        private static ComponentResourceKey _checkBoxBorderHover;
        private static ComponentResourceKey _checkBoxGlyphHover;
        private static ComponentResourceKey _checkBoxTextHover;

        private static ComponentResourceKey _checkBoxBackgroundDown;
        private static ComponentResourceKey _checkBoxBorderDown;
        private static ComponentResourceKey _checkBoxGlyphDown;
        private static ComponentResourceKey _checkBoxTextDown;

        private static ComponentResourceKey _checkBoxBackgroundFocused;
        private static ComponentResourceKey _checkBoxBorderFocused;
        private static ComponentResourceKey _checkBoxGlyphFocused;
        private static ComponentResourceKey _checkBoxTextFocused;

        //MainWindow
        private static ComponentResourceKey _mainWindowBackground;
        private static ComponentResourceKey _mainWindowActiveShadowAndBorderColor;
        private static ComponentResourceKey _mainWindowInactiveShadowAndBorderColor;
        private static ComponentResourceKey _mainWindowTitleBarBackground;
        private static ComponentResourceKey _mainWindowTitleBarForeground;
        private static ComponentResourceKey _mainWindowTitleBarForegroundInactive;


        //ResizeGrip
        private static ComponentResourceKey _resizeGripColor1;
        private static ComponentResourceKey _resizeGripColor2;

        //WindowTitleBarButton
        private static ComponentResourceKey _windowTitleBarButtonBackground;
        private static ComponentResourceKey _windowTitleBarButtonBorder;
        private static ComponentResourceKey _windowTitleBarButtonForeground;
        private static ComponentResourceKey _windowTitleBarButtonHoverBackground;
        private static ComponentResourceKey _windowTitleBarButtonHoverBorder;
        private static ComponentResourceKey _windowTitleBarButtonHoverForeground;
        private static ComponentResourceKey _windowTitleBarButtonDownBackground;
        private static ComponentResourceKey _windowTitleBarButtonDownBorder;
        private static ComponentResourceKey _windowTitleBarButtonDownForeground;

        //WindowBase
        private static ComponentResourceKey _windowBaseBackground;
        private static ComponentResourceKey _windowBaseForeground;

        //LayoutAutoHideWindowControl
        private static ComponentResourceKey _layoutAutoHideWindowBackground;
        private static ComponentResourceKey _layoutAutoHideWindowBorder;

        //LayoutGridResizer
        private static ComponentResourceKey _layoutGridResizerBackground;
        private static ComponentResourceKey _layoutGridResizerBackgroundHorizontal;

        //DockingManager
        private static ComponentResourceKey _dockingManagerBackground;

        //OverlayWindow
        private static ComponentResourceKey _overlayWindowPreviewBoxBackground;
        private static ComponentResourceKey _overlayWindowPreviewBoxBorder;

        //NavigatorWindow
        private static ComponentResourceKey _navigatorWindowBackground;
        private static ComponentResourceKey _navigatorWindowBorder;
        private static ComponentResourceKey _navigatorWindowTitleText;
        private static ComponentResourceKey _navigatorWindowTextForeground;
        private static ComponentResourceKey _navigatorWindowItemBackground;
        private static ComponentResourceKey _navigatorWindowItemBorder;
        private static ComponentResourceKey _navigatorWindowItemForeground;
        private static ComponentResourceKey _navigatorWindowItemBackgroundChecked;
        private static ComponentResourceKey _navigatorWindowItemBorderChecked;
        private static ComponentResourceKey _navigatorWindowItemForegroundChecked;

        //LayoutAnchorableFloatingWindowControl
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarBackground;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarBackgroundActive;

        private static ComponentResourceKey _anchorableFloatingWindowTitleBarGlyph;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarGlyphHover;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarGlyphDown;

        private static ComponentResourceKey _anchorableFloatingWindowTitleBarGlyphActive;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarGlyphActiveHover;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarGlyphActiveDown;

        private static ComponentResourceKey _anchorableFloatingWindowTitleBarForeground;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarForegroundActive;

        private static ComponentResourceKey _anchorableFloatingWindowGrip;
        private static ComponentResourceKey _anchorableFloatingWindowGripActive;

        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBackground;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBackgroundHover;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBackgroundDown;

        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBorder;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBorderHover;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBorderDown;

        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBackgroundActive;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBackgroundActiveHover;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBackgroundActiveDown;

        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBorderActive;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBorderActiveHover;
        private static ComponentResourceKey _anchorableFloatingWindowTitleBarButtonBorderActiveDown;

        //FloatingWindow
        private static ComponentResourceKey _floatingWindowBackground;
        private static ComponentResourceKey _floatingWindowTitleBarBackground;
        private static ComponentResourceKey _floatingWindowTitleBarGlyph;
        private static ComponentResourceKey _floatingWindowTitleBarGlyphHover;
        private static ComponentResourceKey _floatingWindowTitleBarGlyphDown;
        private static ComponentResourceKey _floatingWindowTitleBarForeground;
        private static ComponentResourceKey _floatingWindowTitleBarForegroundActive;

        //DocumentTabItem
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
        private static ComponentResourceKey _documentTabItemButtonBorder;
        private static ComponentResourceKey _documentTabItemButtonBackgroundHover;
        private static ComponentResourceKey _documentTabItemButtonBorderHover;
        private static ComponentResourceKey _documentTabItemButtonBackgroundDown;
        private static ComponentResourceKey _documentTabItemButtonBorderDown;

        private static ComponentResourceKey _documentTabItemButtonBackgroundActive;
        private static ComponentResourceKey _documentTabItemButtonBorderActive;
        private static ComponentResourceKey _documentTabItemButtonBackgroundActiveHover;
        private static ComponentResourceKey _documentTabItemButtonBorderActiveHover;
        private static ComponentResourceKey _documentTabItemButtonBackgroundActiveDown;
        private static ComponentResourceKey _documentTabItemButtonBorderActiveDown;

        private static ComponentResourceKey _documentTabItemButtonBackgroundLastActive;
        private static ComponentResourceKey _documentTabItemButtonBorderLastActive;
        private static ComponentResourceKey _documentTabItemButtonBackgroundLastActiveHover;
        private static ComponentResourceKey _documentTabItemButtonBorderLastActiveHover;
        private static ComponentResourceKey _documentTabItemButtonBackgroundLastActiveDown;
        private static ComponentResourceKey _documentTabItemButtonBorderLastActiveDown;

        private static ComponentResourceKey _documentTabItemBackground;
        private static ComponentResourceKey _documentTabItemBorder;
        private static ComponentResourceKey _documentTabItemBackgroundHover;
        private static ComponentResourceKey _documentTabItemBorderHover;
        private static ComponentResourceKey _documentTabItemBackgroundActive;
        private static ComponentResourceKey _documentTabItemBorderActive;
        private static ComponentResourceKey _documentTabItemBackgroundLastActive;
        private static ComponentResourceKey _documentTabItemBorderLastActive;
        private static ComponentResourceKey _documentTabItemBackgroundDisabled;
        private static ComponentResourceKey _documentTabItemBorderDisabled;

        //AnchorTabItem
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

        //AnchorPaneControl
        private static ComponentResourceKey _anchorPaneControlBorder;

        //LayoutAnchorableControl
        private static ComponentResourceKey _anchorableControlBackground;

        //AnchorSideItem
        private static ComponentResourceKey _anchorSideItemBackground;
        private static ComponentResourceKey _anchorSideItemBorder;
        private static ComponentResourceKey _anchorSideItemForeground;
        private static ComponentResourceKey _anchorSideItemBackgroundHover;
        private static ComponentResourceKey _anchorSideItemBorderHover;
        private static ComponentResourceKey _anchorSideItemForegroundHover;

        //AnchorPaneTitle
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

        //DocumentPaneControl
        private static ComponentResourceKey _documentPaneControlBackground;
        private static ComponentResourceKey _documentPaneControlBackgroundLastActive;
        private static ComponentResourceKey _documentPaneControlBorder;
        private static ComponentResourceKey _documentPaneControlGlyph;
        private static ComponentResourceKey _documentPaneControlGlyphHover;
        private static ComponentResourceKey _documentPaneControlGlyphDown;
        private static ComponentResourceKey _documentPaneControlButtonBackground;
        private static ComponentResourceKey _documentPaneControlButtonBackgroundHover;
        private static ComponentResourceKey _documentPaneControlButtonBackgroundDown;
        private static ComponentResourceKey _documentPaneControlButtonBorder;
        private static ComponentResourceKey _documentPaneControlButtonBorderHover;
        private static ComponentResourceKey _documentPaneControlButtonBorderDown;

        //WaitDialog
        private static ComponentResourceKey _waitDialogActiveShadowAndBorderColor;
        private static ComponentResourceKey _waitDialogInactiveShadowAndBorderColor;
        private static ComponentResourceKey _waitDialogTitleBarBackground;
        private static ComponentResourceKey _waitDialogTitleBarForeground;
        private static ComponentResourceKey _waitDialogMessageForeground;
        private static ComponentResourceKey _waitDialogBackground;

        //ModernExpander
        private static ComponentResourceKey _modernExpanderBackground;
        private static ComponentResourceKey _modernExpanderForeground;
        private static ComponentResourceKey _modernExpanderForegroundDisabled;
        private static ComponentResourceKey _modernExpanderGlyph;


        //ListViewItem
        private static ComponentResourceKey _listViewItemBackground;
        private static ComponentResourceKey _listViewItemBackgroundHover;
        private static ComponentResourceKey _listViewItemBackgroundSelected;
        private static ComponentResourceKey _listViewItemBackgroundSelectedInactive;
        private static ComponentResourceKey _listViewItemForeground;
        private static ComponentResourceKey _listViewItemForegroundHover;
        private static ComponentResourceKey _listViewItemForegroundSelected;
        private static ComponentResourceKey _listViewItemForegroundSelectedInactive;

        //ListView
        private static ComponentResourceKey _listViewBackground;
        
        //CommandBar -- New
        private static ComponentResourceKey _commandBarShadow;
        private static ComponentResourceKey _commandBarBorder;
        private static ComponentResourceKey _commandBarGradientEnd;
        private static ComponentResourceKey _commandBarGradientMiddle;
        private static ComponentResourceKey _commandBarTextActive;
        private static ComponentResourceKey _commandBarTextHover;
        private static ComponentResourceKey _commandBarTextHoverOverSelected;
        private static ComponentResourceKey _commandBarTextInactive;
        private static ComponentResourceKey _commandBarTextMouseDown;
        private static ComponentResourceKey _commandBarTextSelected;
        private static ComponentResourceKey _commandBarSelected;
        private static ComponentResourceKey _commandBarSelectedBorder;
        private static ComponentResourceKey _commandBarSelectedIcon;
        private static ComponentResourceKey _commandBarSelectedIconBorder;
        private static ComponentResourceKey _commandBarSelectedIconDisabled;
        private static ComponentResourceKey _commandBarSelectedIconDisabledBorder;
        private static ComponentResourceKey _commandBarHover;
        private static ComponentResourceKey _commandBarHoverOverSelected;
        private static ComponentResourceKey _commandBarHoverOverSelectedIcon;
        private static ComponentResourceKey _commandBarHoverOverSelectedIconBorder;
        private static ComponentResourceKey _commandBarDragHandle;
        private static ComponentResourceKey _commandBarDragHandleShadow;
        private static ComponentResourceKey _commandBarGradientBegin;
        private static ComponentResourceKey _commandBarMouseDownBackgroundBegin;
        private static ComponentResourceKey _commandBarMouseDownBackgroundEnd;
        private static ComponentResourceKey _commandBarMouseDownBackgroundMiddle;
        private static ComponentResourceKey _commandBarMouseDownBorder;
        private static ComponentResourceKey _commandBarMouseOverBackgroundBegin;
        private static ComponentResourceKey _commandBarMouseOverBackgroundEnd;
        private static ComponentResourceKey _commandBarMouseOverBackgroundMiddle1;
        private static ComponentResourceKey _commandBarMouseOverBackgroundMiddle2;
        private static ComponentResourceKey _commandBarMouseOverUnfocused;

        private static ComponentResourceKey _commandBarSplitButtonGlyph;
        private static ComponentResourceKey _commandBarSplitButtonMouseDownGlyph;
        private static ComponentResourceKey _commandBarSplitButtonMouseOverGlyph;
        private static ComponentResourceKey _commandBarSplitButtonSeparator;
        private static ComponentResourceKey _commandBarToolBarBorder;
        private static ComponentResourceKey _commandBarToolBarSeparator;
        private static ComponentResourceKey _commandBarToolBarSeparatorHighlight;
        private static ComponentResourceKey _commandBarCheckBox;
        private static ComponentResourceKey _commandBarCheckBoxDisabled;
        private static ComponentResourceKey _commandBarCheckBoxMouseOver;
        private static ComponentResourceKey _commandBarMenuBackgroundGradientBegin;
        private static ComponentResourceKey _commandBarMenuBackgroundGradientEnd;
        private static ComponentResourceKey _commandBarMenuBorder;
        private static ComponentResourceKey _commandBarMenuGlyph;
        private static ComponentResourceKey _commandBarMenuGroupHeaderLinkText;
        private static ComponentResourceKey _commandBarMenuGroupHeaderLinkTextHover;
        private static ComponentResourceKey _commandBarMenuIconBackground;
        private static ComponentResourceKey _commandBarMenuItemMouseOver;
        private static ComponentResourceKey _commandBarMenuItemMouseOverText;
        private static ComponentResourceKey _commandBarMenuItemMouseOverBorder;
        private static ComponentResourceKey _commandBarMenuLinkText;
        private static ComponentResourceKey _commandBarMenuLinkTextHover;
        private static ComponentResourceKey _commandBarMenuMouseDownGlyph;
        private static ComponentResourceKey _commandBarMenuMouseOverGlyph;
        private static ComponentResourceKey _commandBarMenuMouseOverSeparator;
        private static ComponentResourceKey _commandBarMenuMouseOverSubmenuGlyph;
        private static ComponentResourceKey _commandBarMenuScrollGlyph;
        private static ComponentResourceKey _commandBarMenuSeparator;
        private static ComponentResourceKey _commandBarMenuSubmenuGlyph;
        private static ComponentResourceKey _commandBarMenuSubmenuGlyphHover;
        private static ComponentResourceKey _commandBarMenuWatermarkLinkText;
        private static ComponentResourceKey _commandBarMenuWatermarkLinkTextHover;
        private static ComponentResourceKey _commandBarMenuWatermarkText;
        private static ComponentResourceKey _commandBarMenuWatermarkTextHover;
        private static ComponentResourceKey _commandBarOptionsBackground;
        private static ComponentResourceKey _commandBarOptionsGlyph;
        private static ComponentResourceKey _commandBarOptionsMouseDownBackgroundBegin;
        private static ComponentResourceKey _commandBarOptionsMouseDownBackgroundEnd;
        private static ComponentResourceKey _commandBarOptionsMouseDownBackgroundMiddle;
        private static ComponentResourceKey _commandBarOptionsMouseDownGlyph;
        private static ComponentResourceKey _commandBarOptionsMouseOverBackgroundBegin;
        private static ComponentResourceKey _commandBarOptionsMouseOverBackgroundEnd;
        private static ComponentResourceKey _commandBarOptionsMouseOverBackgroundMiddle1;
        private static ComponentResourceKey _commandBarOptionsMouseOverBackgroundMiddle2;
        private static ComponentResourceKey _commandBarOptionsMouseOverGlyph;

        //CommandShelf
        private static ComponentResourceKey _commandShelfBackground;
        private static ComponentResourceKey _commandShelfHighlight;

        //QuickCustomizeButton
        private static ComponentResourceKey _qickCustomizeButtonText;
        private static ComponentResourceKey _qickCustomizeButtonGlyph;
        private static ComponentResourceKey _quickCustomizeButtonBorder;
        private static ComponentResourceKey _quickCustomizeButton;
        private static ComponentResourceKey _quickCustomizeButtonTextHover;
        private static ComponentResourceKey _quickCustomizeButtonGlyphHover;

        //SystemColors -- New
        private static ComponentResourceKey _systemWindow;
        private static ComponentResourceKey _systemGrayText;

        #region SystemColors

        public static ComponentResourceKey SystemWindow => _systemWindow ??
                                                                     (_systemWindow = new ComponentResourceKey(typeof(EnvironmentColors), "SystemWindow"));

        public static ComponentResourceKey SystemGrayText => _systemGrayText ??
                                                                    (_systemGrayText = new ComponentResourceKey(typeof(EnvironmentColors), "SystemGrayText"));

        #endregion

        public static ComponentResourceKey CommandShelfBackground => _commandShelfBackground ??
                                                                     (_commandShelfBackground = new ComponentResourceKey(typeof(EnvironmentColors), "CommandShelfBackground"));

        public static ComponentResourceKey CommandShelfHighlight => _commandShelfHighlight ??
                                                                    (_commandShelfHighlight = new ComponentResourceKey(typeof(EnvironmentColors), "CommandShelfHighlight"));

        #region QuickCustomizeButton

        public static ComponentResourceKey QuickCustomizeButtonText => _qickCustomizeButtonText ??
                                                                             (_qickCustomizeButtonText = new ComponentResourceKey(typeof(EnvironmentColors), "QuickCustomizeButtonText"));

        public static ComponentResourceKey QuickCustomizeButtonGlyph => _qickCustomizeButtonGlyph ??
                                                                             (_qickCustomizeButtonGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "QuickCustomizeButtonGlyph"));

        public static ComponentResourceKey QuickCustomizeButtonBorder => _quickCustomizeButtonBorder ??
                                                                     (_quickCustomizeButtonBorder = new ComponentResourceKey(typeof(EnvironmentColors), "QuickCustomizeButtonBorder"));

        public static ComponentResourceKey QuickCustomizeButton => _quickCustomizeButton ??
                                                                     (_quickCustomizeButton = new ComponentResourceKey(typeof(EnvironmentColors), "QuickCustomizeButton"));

        public static ComponentResourceKey QuickCustomizeButtonTextHover => _quickCustomizeButtonTextHover ??
                                                                     (_quickCustomizeButtonTextHover = new ComponentResourceKey(typeof(EnvironmentColors), "QuickCustomizeButtonTextHover"));

        public static ComponentResourceKey QuickCustomizeButtonGlyphHover => _quickCustomizeButtonGlyphHover ??
                                                                     (_quickCustomizeButtonGlyphHover = new ComponentResourceKey(typeof(EnvironmentColors), "QuickCustomizeButtonGlyphHover"));

        #endregion

        #region CommandBar

        public static ComponentResourceKey CommandBarShadow => _commandBarShadow ??
                                                               (_commandBarShadow = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarShadow"));

        public static ComponentResourceKey CommandBarBorder => _commandBarBorder ??
                                                               (_commandBarBorder = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarBorder"));

        public static ComponentResourceKey CommandBarGradientEnd => _commandBarGradientEnd ??
                                                                    (_commandBarGradientEnd = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarGradientEnd"));

        public static ComponentResourceKey CommandBarGradientMiddle => _commandBarGradientMiddle ??
                                                                       (_commandBarGradientMiddle = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarShadow"));

        public static ComponentResourceKey CommandBarTextActive => _commandBarTextActive ??
                                                                   (_commandBarTextActive = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarTextActive"));

        public static ComponentResourceKey CommandBarTextHover => _commandBarTextHover ??
                                                                  (_commandBarTextHover = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarTextHover"));

        public static ComponentResourceKey CommandBarTextHoverOverSelected => _commandBarTextHoverOverSelected ??
                                                                              (_commandBarTextHoverOverSelected = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarTextHoverOverSelected"));

        public static ComponentResourceKey CommandBarTextInactive => _commandBarTextInactive ??
                                                                     (_commandBarTextInactive = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarTextInactive"));

        public static ComponentResourceKey CommandBarTextMouseDown => _commandBarTextMouseDown ??
                                                                      (_commandBarTextMouseDown = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarTextMouseDown"));

        public static ComponentResourceKey CommandBarTextSelected => _commandBarTextSelected ??
                                                                     (_commandBarTextSelected = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarTextSelected"));

        public static ComponentResourceKey CommandBarSelected => _commandBarSelected ??
                                                                 (_commandBarSelected = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarSelected"));

        public static ComponentResourceKey CommandBarSelectedBorder => _commandBarSelectedBorder ??
                                                                       (_commandBarSelectedBorder = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarSelectedBorder"));

        public static ComponentResourceKey CommandBarSelectedIcon => _commandBarSelectedIcon ??
                                                                     (_commandBarSelectedIcon = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarSelectedIcon"));

        public static ComponentResourceKey CommandBarSelectedIconBorder => _commandBarSelectedIconBorder ??
                                                                           (_commandBarSelectedIconBorder = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarSelectedIconBorder"));

        public static ComponentResourceKey CommandBarSelectedIconDisabled => _commandBarSelectedIconDisabled ??
                                                                             (_commandBarSelectedIconDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarSelectedIconDisabled"));

        public static ComponentResourceKey CommandBarSelectedIconDisabledBorder => _commandBarSelectedIconDisabledBorder ??
                                                                                   (_commandBarSelectedIconDisabledBorder = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarSelectedIconDisabledBorder"));

        public static ComponentResourceKey CommandBarHover => _commandBarHover ??
                                                              (_commandBarHover = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarHover"));

        public static ComponentResourceKey CommandBarHoverOverSelected => _commandBarHoverOverSelected ??
                                                                          (_commandBarHoverOverSelected = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarHoverOverSelected"));

        public static ComponentResourceKey CommandBarHoverOverSelectedIcon => _commandBarHoverOverSelectedIcon ??
                                                                              (_commandBarHoverOverSelectedIcon = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarHoverOverSelectedIcon"));

        public static ComponentResourceKey CommandBarHoverOverSelectedIconBorder => _commandBarHoverOverSelectedIconBorder ??
                                                                                    (_commandBarHoverOverSelectedIconBorder = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarHoverOverSelectedIconBorder"));

        public static ComponentResourceKey CommandBarDragHandle => _commandBarDragHandle ??
                                                                   (_commandBarDragHandle = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarDragHandle"));

        public static ComponentResourceKey CommandBarDragHandleShadow => _commandBarDragHandleShadow ??
                                                                         (_commandBarDragHandleShadow = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarDragHandleShadow"));

        public static ComponentResourceKey CommandBarGradientBegin => _commandBarGradientBegin ??
                                                                      (_commandBarGradientBegin = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarGradientBegin"));

        public static ComponentResourceKey CommandBarMouseDownBackgroundBegin => _commandBarMouseDownBackgroundBegin ??
                                                                                 (_commandBarMouseDownBackgroundBegin = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMouseDownBackgroundBegin"));

        public static ComponentResourceKey CommandBarMouseDownBackgroundEnd => _commandBarMouseDownBackgroundEnd ??
                                                                               (_commandBarMouseDownBackgroundEnd = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMouseDownBackgroundEnd"));

        public static ComponentResourceKey CommandBarMouseDownBackgroundMiddle => _commandBarMouseDownBackgroundMiddle ??
                                                                                  (_commandBarMouseDownBackgroundMiddle = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMouseDownBackgroundMiddle"));

        public static ComponentResourceKey CommandBarMouseDownBorder => _commandBarMouseDownBorder ??
                                                                        (_commandBarMouseDownBorder = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMouseDownBorder"));

        public static ComponentResourceKey CommandBarMouseOverBackgroundBegin => _commandBarMouseOverBackgroundBegin ??
                                                                                 (_commandBarMouseOverBackgroundBegin = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMouseOverBackgroundBegin"));

        public static ComponentResourceKey CommandBarMouseOverBackgroundEnd => _commandBarMouseOverBackgroundEnd ??
                                                                               (_commandBarMouseOverBackgroundEnd = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMouseOverBackgroundEnd"));

        public static ComponentResourceKey CommandBarMouseOverBackgroundMiddle1 => _commandBarMouseOverBackgroundMiddle1 ??
                                                                                   (_commandBarMouseOverBackgroundMiddle1 = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMouseOverBackgroundMiddle1"));

        public static ComponentResourceKey CommandBarMouseOverBackgroundMiddle2 => _commandBarMouseOverBackgroundMiddle2 ??
                                                                                   (_commandBarMouseOverBackgroundMiddle2 = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMouseOverBackgroundMiddle2"));

        public static ComponentResourceKey CommandBarMouseOverUnfocused => _commandBarMouseOverUnfocused ??
                                                                           (_commandBarMouseOverUnfocused = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMouseOverUnfocused"));

        public static ComponentResourceKey CommandBarSplitButtonGlyph => _commandBarSplitButtonGlyph ??
                                                                         (_commandBarSplitButtonGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarSplitButtonGlyph"));

        public static ComponentResourceKey CommandBarSplitButtonMouseDownGlyph => _commandBarSplitButtonMouseDownGlyph ??
                                                                                  (_commandBarSplitButtonMouseDownGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarSplitButtonMouseDownGlyph"));

        public static ComponentResourceKey CommandBarSplitButtonMouseOverGlyph => _commandBarSplitButtonMouseOverGlyph ??
                                                                                  (_commandBarSplitButtonMouseOverGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarSplitButtonMouseOverGlyph"));

        public static ComponentResourceKey CommandBarSplitButtonSeparator => _commandBarSplitButtonSeparator ??
                                                                             (_commandBarSplitButtonSeparator = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarSplitButtonSeparator"));

        public static ComponentResourceKey CommandBarToolBarBorder => _commandBarToolBarBorder ??
                                                                      (_commandBarToolBarBorder = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarToolBarBorder"));

        public static ComponentResourceKey CommandBarToolBarSeparator => _commandBarToolBarSeparator ??
                                                                         (_commandBarToolBarSeparator = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarToolBarSeparator"));

        public static ComponentResourceKey CommandBarToolBarSeparatorHighlight => _commandBarToolBarSeparatorHighlight ??
                                                                                  (_commandBarToolBarSeparatorHighlight = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarToolBarSeparatorHighlight"));

        public static ComponentResourceKey CommandBarCheckBox => _commandBarCheckBox ??
                                                                 (_commandBarCheckBox = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarCheckBox"));

        public static ComponentResourceKey CommandBarCheckBoxDisabled => _commandBarCheckBoxDisabled ??
                                                                         (_commandBarCheckBoxDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarCheckBoxDisabled"));

        public static ComponentResourceKey CommandBarCheckBoxMouseOver => _commandBarCheckBoxMouseOver ??
                                                                          (_commandBarCheckBoxMouseOver = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarCheckBoxMouseOver"));

        public static ComponentResourceKey CommandBarMenuBackgroundGradientBegin => _commandBarMenuBackgroundGradientBegin ??
                                                                                    (_commandBarMenuBackgroundGradientBegin = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuBackgroundGradientBegin"));

        public static ComponentResourceKey CommandBarMenuBackgroundGradientEnd => _commandBarMenuBackgroundGradientEnd ??
                                                                                  (_commandBarMenuBackgroundGradientEnd = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuBackgroundGradientEnd"));

        public static ComponentResourceKey CommandBarMenuBorder => _commandBarMenuBorder ??
                                                                   (_commandBarMenuBorder = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuBorder"));

        public static ComponentResourceKey CommandBarMenuGlyph => _commandBarMenuGlyph ??
                                                                  (_commandBarMenuGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuGlyph"));

        public static ComponentResourceKey CommandBarMenuGroupHeaderLinkText => _commandBarMenuGroupHeaderLinkText ??
                                                                                (_commandBarMenuGroupHeaderLinkText = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuGroupHeaderLinkText"));

        public static ComponentResourceKey CommandBarMenuGroupHeaderLinkTextHover => _commandBarMenuGroupHeaderLinkTextHover ??
                                                                                     (_commandBarMenuGroupHeaderLinkTextHover = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuGroupHeaderLinkTextHover"));

        public static ComponentResourceKey CommandBarMenuIconBackground => _commandBarMenuIconBackground ??
                                                                           (_commandBarMenuIconBackground = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuIconBackground"));

        public static ComponentResourceKey CommandBarMenuItemMouseOver => _commandBarMenuItemMouseOver ??
                                                                          (_commandBarMenuItemMouseOver = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuItemMouseOver"));

        public static ComponentResourceKey CommandBarMenuItemMouseOverText => _commandBarMenuItemMouseOverText ??
                                                                              (_commandBarMenuItemMouseOverText = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuItemMouseOverText"));

        public static ComponentResourceKey CommandBarMenuItemMouseOverBorder => _commandBarMenuItemMouseOverBorder ??
                                                                                (_commandBarMenuItemMouseOverBorder = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuItemMouseOverBorder"));

        public static ComponentResourceKey CommandBarMenuLinkText => _commandBarMenuLinkText ??
                                                                     (_commandBarMenuLinkText = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuLinkText"));

        public static ComponentResourceKey CommandBarMenuLinkTextHover => _commandBarMenuLinkTextHover ??
                                                                          (_commandBarMenuLinkTextHover = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuLinkTextHover"));

        public static ComponentResourceKey CommandBarMenuMouseDownGlyph => _commandBarMenuMouseDownGlyph ??
                                                                           (_commandBarMenuMouseDownGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuMouseDownGlyph"));

        public static ComponentResourceKey CommandBarMenuMouseOverGlyph => _commandBarMenuMouseOverGlyph ??
                                                                           (_commandBarMenuMouseOverGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuMouseOverGlyph"));

        public static ComponentResourceKey CommandBarMenuMouseOverSeparator => _commandBarMenuMouseOverSeparator ??
                                                                               (_commandBarMenuMouseOverSeparator = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuMouseOverSeparator"));

        public static ComponentResourceKey CommandBarMenuMouseOverSubmenuGlyph => _commandBarMenuMouseOverSubmenuGlyph ??
                                                                                  (_commandBarMenuMouseOverSubmenuGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuMouseOverSubmenuGlyph"));

        public static ComponentResourceKey CommandBarMenuScrollGlyph => _commandBarMenuScrollGlyph ??
                                                                        (_commandBarMenuScrollGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuScrollGlyph"));

        public static ComponentResourceKey CommandBarMenuSeparator => _commandBarMenuSeparator ??
                                                                      (_commandBarMenuSeparator = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuSeparator"));

        public static ComponentResourceKey CommandBarMenuSubmenuGlyph => _commandBarMenuSubmenuGlyph ??
                                                                         (_commandBarMenuSubmenuGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuSubmenuGlyph"));

        public static ComponentResourceKey CommandBarMenuSubmenuGlyphHover => _commandBarMenuSubmenuGlyphHover ??
                                                                              (_commandBarMenuSubmenuGlyphHover = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuSubmenuGlyphHover"));

        public static ComponentResourceKey CommandBarMenuWatermarkLinkText => _commandBarMenuWatermarkLinkText ??
                                                                              (_commandBarMenuWatermarkLinkText = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuWatermarkLinkText"));

        public static ComponentResourceKey CommandBarMenuWatermarkLinkTextHover => _commandBarMenuWatermarkLinkTextHover ??
                                                                                   (_commandBarMenuWatermarkLinkTextHover = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuWatermarkLinkTextHover"));

        public static ComponentResourceKey CommandBarMenuWatermarkText => _commandBarMenuWatermarkText ??
                                                                          (_commandBarMenuWatermarkText = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuWatermarkText"));

        public static ComponentResourceKey CommandBarMenuWatermarkTextHover => _commandBarMenuWatermarkTextHover ??
                                                                               (_commandBarMenuWatermarkTextHover = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarMenuWatermarkTextHover"));

        public static ComponentResourceKey CommandBarOptionsBackground => _commandBarOptionsBackground ??
                                                                          (_commandBarOptionsBackground = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarOptionsBackground"));

        public static ComponentResourceKey CommandBarOptionsGlyph => _commandBarOptionsGlyph ??
                                                                     (_commandBarOptionsGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarOptionsGlyph"));

        public static ComponentResourceKey CommandBarOptionsMouseDownBackgroundBegin => _commandBarOptionsMouseDownBackgroundBegin ??
                                                                                        (_commandBarOptionsMouseDownBackgroundBegin = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarOptionsMouseDownBackgroundBegin"));

        public static ComponentResourceKey CommandBarOptionsMouseDownBackgroundEnd => _commandBarOptionsMouseDownBackgroundEnd ??
                                                                                      (_commandBarOptionsMouseDownBackgroundEnd = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarOptionsMouseDownBackgroundEnd"));

        public static ComponentResourceKey CommandBarOptionsMouseDownBackgroundMiddle => _commandBarOptionsMouseDownBackgroundMiddle ??
                                                                                         (_commandBarOptionsMouseDownBackgroundMiddle = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarOptionsMouseDownBackgroundMiddle"));

        public static ComponentResourceKey CommandBarOptionsMouseDownGlyph => _commandBarOptionsMouseDownGlyph ??
                                                                              (_commandBarOptionsMouseDownGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarOptionsMouseDownGlyph"));

        public static ComponentResourceKey CommandBarOptionsMouseOverBackgroundBegin => _commandBarOptionsMouseOverBackgroundBegin ??
                                                                                        (_commandBarOptionsMouseOverBackgroundBegin = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarOptionsMouseOverBackgroundBegin"));

        public static ComponentResourceKey CommandBarOptionsMouseOverBackgroundEnd => _commandBarOptionsMouseOverBackgroundEnd ??
                                                                                      (_commandBarOptionsMouseOverBackgroundEnd = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarOptionsMouseOverBackgroundEnd"));

        public static ComponentResourceKey CommandBarOptionsMouseOverBackgroundMiddle1 => _commandBarOptionsMouseOverBackgroundMiddle1 ??
                                                                                          (_commandBarOptionsMouseOverBackgroundMiddle1 = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarOptionsMouseOverBackgroundMiddle1"));

        public static ComponentResourceKey CommandBarOptionsMouseOverBackgroundMiddle2 => _commandBarOptionsMouseOverBackgroundMiddle2 ??
                                                                                          (_commandBarOptionsMouseOverBackgroundMiddle2 = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarOptionsMouseOverBackgroundMiddle2"));

        public static ComponentResourceKey CommandBarOptionsMouseOverGlyph => _commandBarOptionsMouseOverGlyph ??
                                                                              (_commandBarOptionsMouseOverGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "CommandBarOptionsMouseOverGlyph"));

        #endregion

        #region ListView
        public static ComponentResourceKey ListViewBackground => _listViewBackground ??
                                                            (_listViewBackground = new ComponentResourceKey(typeof(EnvironmentColors), "ListViewBackground"));
        #endregion

        #region ListViewItem
        public static ComponentResourceKey ListViewItemBackground => _listViewItemBackground ??
                                                            (_listViewItemBackground = new ComponentResourceKey(typeof(EnvironmentColors), "ListViewItemBackground"));
        public static ComponentResourceKey ListViewItemBackgroundHover => _listViewItemBackgroundHover ??
                                                            (_listViewItemBackgroundHover = new ComponentResourceKey(typeof(EnvironmentColors), "ListViewItemBackgroundHover"));
        public static ComponentResourceKey ListViewItemBackgroundSelected => _listViewItemBackgroundSelected ??
                                                                    (_listViewItemBackgroundSelected = new ComponentResourceKey(typeof(EnvironmentColors), "ListViewItemBackgroundSelected"));
        public static ComponentResourceKey ListViewItemBackgroundSelectedInactive => _listViewItemBackgroundSelectedInactive ??
                                                                    (_listViewItemBackgroundSelectedInactive = new ComponentResourceKey(typeof(EnvironmentColors), "ListViewItemBackgroundSelectedInactive"));

        public static ComponentResourceKey ListViewItemForeground => _listViewItemForeground ??
                                                                    (_listViewItemForeground = new ComponentResourceKey(typeof(EnvironmentColors), "ListViewItemForeground"));

        public static ComponentResourceKey ListViewItemForegroundSelected => _listViewItemForegroundSelected ??
                                                                    (_listViewItemForegroundSelected = new ComponentResourceKey(typeof(EnvironmentColors), "ListViewItemForegroundSelected"));

        public static ComponentResourceKey ListViewItemForegroundHover => _listViewItemForegroundHover ??
                                                                    (_listViewItemForegroundHover = new ComponentResourceKey(typeof(EnvironmentColors), "ListViewItemForegroundHover"));

        public static ComponentResourceKey ListViewItemForegroundSelectedInactive => _listViewItemForegroundSelectedInactive ??
                                                                    (_listViewItemForegroundSelectedInactive = new ComponentResourceKey(typeof(EnvironmentColors), "ListViewItemForegroundSelectedInactive"));

        #endregion

        #region ModernExpander
        public static ComponentResourceKey ModernExpanderBackground => _modernExpanderBackground ??
                                                            (_modernExpanderBackground = new ComponentResourceKey(typeof(EnvironmentColors), "ModernExpanderBackground"));

        public static ComponentResourceKey ModernExpanderForeground => _modernExpanderForeground ??
                                                           (_modernExpanderForeground = new ComponentResourceKey(typeof(EnvironmentColors), "ModernExpanderForeground"));

        public static ComponentResourceKey ModernExpanderForegroundDisabled => _modernExpanderForegroundDisabled ??
                                                           (_modernExpanderForegroundDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "ModernExpanderForegroundDisabled"));

        public static ComponentResourceKey ModernExpanderGlyph => _modernExpanderGlyph ??
                                                           (_modernExpanderGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "ModernExpanderGlyph"));

        #endregion

        #region WaitDialog

        public static ComponentResourceKey WaitDialogActiveShadowAndBorderColor => _waitDialogActiveShadowAndBorderColor ??
                                                            (_waitDialogActiveShadowAndBorderColor = new ComponentResourceKey(typeof(EnvironmentColors), "WaitDialogActiveShadowAndBorderColor"));

        public static ComponentResourceKey WaitDialogInactiveShadowAndBorderColor => _waitDialogInactiveShadowAndBorderColor ??
                                                            (_waitDialogInactiveShadowAndBorderColor = new ComponentResourceKey(typeof(EnvironmentColors), "WaitDialogInactiveShadowAndBorderColor"));

        public static ComponentResourceKey WaitDialogTitleBarBackground => _waitDialogTitleBarBackground ??
                                                            (_waitDialogTitleBarBackground = new ComponentResourceKey(typeof(EnvironmentColors), "WaitDialogTitleBarBackground"));

        public static ComponentResourceKey WaitDialogTitleBarForeground => _waitDialogTitleBarForeground ??
                                                            (_waitDialogTitleBarForeground = new ComponentResourceKey(typeof(EnvironmentColors), "WaitDialogTitleBarForeground"));

        public static ComponentResourceKey WaitDialogMessageForeground => _waitDialogMessageForeground ??
                                                            (_waitDialogMessageForeground = new ComponentResourceKey(typeof(EnvironmentColors), "WaitDialogMessageForeground"));

        public static ComponentResourceKey WaitDialogBackground => _waitDialogBackground ??
                                                            (_waitDialogBackground = new ComponentResourceKey(typeof(EnvironmentColors), "WaitDialogBackground"));

        #endregion

        #region Global

        public static ComponentResourceKey GloablForeground => _gloablForeground ??
                                                               (_gloablForeground = new ComponentResourceKey(typeof(EnvironmentColors), "GloablForeground"));

        public static ComponentResourceKey GlobalBackgroundColor => _globalBackgroundColor ??
                                                                    (_globalBackgroundColor = new ComponentResourceKey(typeof(EnvironmentColors), "GlobalBackgroundColor"));
        public static ComponentResourceKey GlobalBackgroundColor2 => _globalBackgroundColor2 ??
                                                                     (_globalBackgroundColor2 = new ComponentResourceKey(typeof(EnvironmentColors), "GlobalBackgroundColor2"));
        #endregion

        #region Button

        public static ComponentResourceKey ButtonBackground => _buttonBackground ??
                                                               (_buttonBackground = new ComponentResourceKey(typeof(EnvironmentColors), "ButtonBackground"));

        public static ComponentResourceKey ButtonForeground => _buttonForeground ??
                                                               (_buttonForeground = new ComponentResourceKey(typeof(EnvironmentColors), "ButtonForeground"));

        public static ComponentResourceKey ButtonBorder => _buttonBorder ??
                                                           (_buttonBorder = new ComponentResourceKey(typeof(EnvironmentColors), "ButtonBorder"));

        public static ComponentResourceKey ButtonBackgroundHover => _buttonBackgroundHover ??
                                                                    (_buttonBackgroundHover =
                                                                        new ComponentResourceKey(typeof(EnvironmentColors), "ButtonBackgroundHover"));

        public static ComponentResourceKey ButtonForegroundHover => _buttonForegroundHover ??
                                                                    (_buttonForegroundHover = new ComponentResourceKey(typeof(EnvironmentColors), "ButtonForegroundHover"));

        public static ComponentResourceKey ButtonBorderHover => _buttonBorderHover ??
                                                                (_buttonBorderHover = new ComponentResourceKey(typeof(EnvironmentColors), "ButtonBorderHover"));

        public static ComponentResourceKey ButtonBackgroundDown => _buttonBackgroundDown ??
                                                                   (_buttonBackgroundDown = new ComponentResourceKey(typeof(EnvironmentColors), "ButtonBackgroundDown"));

        public static ComponentResourceKey ButtonForegroundDown => _buttonForegroundDown ??
                                                                   (_buttonForegroundDown = new ComponentResourceKey(typeof(EnvironmentColors), "ButtonForegroundDown"));

        public static ComponentResourceKey ButtonBorderDown => _buttonBorderDown ??
                                                               (_buttonBorderDown = new ComponentResourceKey(typeof(EnvironmentColors), "ButtonBorderDown"));

        public static ComponentResourceKey ButtonBackgroundDisabled => _buttonBackgroundDisabled ??
                                                                       (_buttonBackgroundDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "ButtonBackgroundDisabled"));

        public static ComponentResourceKey ButtonForegroundDisabled => _buttonForegroundDisabled ??
                                                                       (_buttonForegroundDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "ButtonForegroundDisabled"));

        public static ComponentResourceKey ButtonBorderDisabled => _buttonBorderDisabled ??
                                                                   (_buttonBorderDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "ButtonBorderDisabled"));

        #endregion

        #region ComboBox

        public static ComponentResourceKey ComboBoxBackground => _comboBoxBackground ??
                                                                 (_comboBoxBackground = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxBackground"));

        public static ComponentResourceKey ComboBoxBorder => _comboBoxBorder ??
                                                             (_comboBoxBorder = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxBorder"));

        public static ComponentResourceKey ComboBoxButtonMouseDownBackground => _comboBoxButtonMouseDownBackground ??
                                                                                (_comboBoxButtonMouseDownBackground = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxButtonMouseDownBackground"));

        public static ComponentResourceKey ComboBoxButtonMouseDownSeparator => _comboBoxButtonMouseDownSeparator ??
                                                                               (_comboBoxButtonMouseDownSeparator = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxButtonMouseDownSeparator"));

        public static ComponentResourceKey ComboBoxButtonMouseOverBackground => _comboBoxButtonMouseOverBackground ??
                                                                                (_comboBoxButtonMouseOverBackground = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxButtonMouseOverBackground"));

        public static ComponentResourceKey ComboBoxButtonMouseOverSeparator => _comboBoxButtonMouseOverSeparator ??
                                                                               (_comboBoxButtonMouseOverSeparator = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxButtonMouseOverSeparator"));

        public static ComponentResourceKey ComboBoxDisabledBackground => _comboBoxDisabledBackground ??
                                                                         (_comboBoxDisabledBackground = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxDisabledBackground"));

        public static ComponentResourceKey ComboBoxDisabledBorder => _comboBoxDisabledBorder ??
                                                                     (_comboBoxDisabledBorder = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxDisabledBorder"));

        public static ComponentResourceKey ComboBoxDisabledGlyph => _comboBoxDisabledGlyph ??
                                                                    (_comboBoxDisabledGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxDisabledGlyph"));

        public static ComponentResourceKey ComboBoxDisabledText => _comboBoxDisabledText ??
                                                                   (_comboBoxDisabledText = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxDisabledText"));

        public static ComponentResourceKey ComboBoxFocusedBackground => _comboBoxFocusedBackground ??
                                                                        (_comboBoxFocusedBackground = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxFocusedBackground"));

        public static ComponentResourceKey ComboBoxFocusedBorder => _comboBoxFocusedBorder ??
                                                                    (_comboBoxFocusedBorder = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxFocusedBorder"));

        public static ComponentResourceKey ComboBoxFocusedButtonBackground => _comboBoxFocusedButtonBackground ??
                                                                              (_comboBoxFocusedButtonBackground = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxFocusedButtonBackground"));

        public static ComponentResourceKey ComboBoxFocusedButtonSeparator => _comboBoxFocusedButtonSeparator ??
                                                                             (_comboBoxFocusedButtonSeparator = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxFocusedButtonSeparator"));

        public static ComponentResourceKey ComboBoxFocusedGlyph => _comboBoxFocusedGlyph ??
                                                                   (_comboBoxFocusedGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxFocusedGlyph"));

        public static ComponentResourceKey ComboBoxFocusedText => _comboBoxFocusedText ??
                                                                  (_comboBoxFocusedText = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxFocusedText"));

        public static ComponentResourceKey ComboBoxGlyph => _comboBoxGlyph ??
                                                           (_comboBoxGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxGlyph"));

        public static ComponentResourceKey ComboBoxItemMouseOverBackground => _comboBoxItemMouseOverBackground ??
                                                                              (_comboBoxItemMouseOverBackground = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxItemMouseOverBackground"));

        public static ComponentResourceKey ComboBoxItemMouseOverBorder => _comboBoxItemMouseOverBorder ??
                                                                          (_comboBoxItemMouseOverBorder = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxItemMouseOverBorder"));

        public static ComponentResourceKey ComboBoxItemMouseOverText => _comboBoxItemMouseOverText ??
                                                                        (_comboBoxItemMouseOverText = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxItemMouseOverText"));

        public static ComponentResourceKey ComboBoxItemText => _comboBoxItemText ??
                                                               (_comboBoxItemText = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxItemText"));

        public static ComponentResourceKey ComboBoxItemTextInactive => _comboBoxItemTextInactive ??
                                                                       (_comboBoxItemTextInactive = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxItemTextInactive"));

        public static ComponentResourceKey ComboBoxMouseDownBackground => _comboBoxMouseDownBackground ??
                                                                          (_comboBoxMouseDownBackground = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxMouseDownBackground"));

        public static ComponentResourceKey ComboBoxMouseDownBorder => _comboBoxMouseDownBorder ??
                                                                      (_comboBoxMouseDownBorder = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxMouseDownBorder"));

        public static ComponentResourceKey ComboBoxMouseDownGlyph => _comboBoxMouseDownGlyph ??
                                                                     (_comboBoxMouseDownGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxMouseDownGlyph"));

        public static ComponentResourceKey ComboBoxMouseDownText => _comboBoxMouseDownText ??
                                                                    (_comboBoxMouseDownText = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxMouseDownText"));

        public static ComponentResourceKey ComboBoxMouseOverBackgroundBegin => _comboBoxMouseOverBackgroundBegin ??
                                                                               (_comboBoxMouseOverBackgroundBegin = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxMouseOverBackgroundBegin"));

        public static ComponentResourceKey ComboBoxMouseOverBackgroundEnd => _comboBoxMouseOverBackgroundEnd ??
                                                                             (_comboBoxMouseOverBackgroundEnd = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxMouseOverBackgroundEnd"));

        public static ComponentResourceKey ComboBoxMouseOverBackgroundMiddle1 => _comboBoxMouseOverBackgroundMiddle1 ??
                                                                                 (_comboBoxMouseOverBackgroundMiddle1 = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxMouseOverBackgroundMiddle1"));

        public static ComponentResourceKey ComboBoxMouseOverBackgroundMiddle2 => _comboBoxMouseOverBackgroundMiddle2 ??
                                                                                 (_comboBoxMouseOverBackgroundMiddle2 = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxMouseOverBackgroundMiddle2"));

        public static ComponentResourceKey ComboBoxMouseOverBorder => _comboBoxMouseOverBorder ??
                                                                      (_comboBoxMouseOverBorder = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxMouseOverBorder"));

        public static ComponentResourceKey ComboBoxMouseOverGlyph => _comboBoxMouseOverGlyph ??
                                                                     (_comboBoxMouseOverGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxMouseOverGlyph"));

        public static ComponentResourceKey ComboBoxMouseOverText => _comboBoxMouseOverText ??
                                                                    (_comboBoxMouseOverText = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxMouseOverText"));

        public static ComponentResourceKey ComboBoxPopupBackgroundBegin => _comboBoxPopupBackgroundBegin ??
                                                                           (_comboBoxPopupBackgroundBegin = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxPopupBackgroundBegin"));

        public static ComponentResourceKey ComboBoxPopupBackgroundEnd => _comboBoxPopupBackgroundEnd ??
                                                                         (_comboBoxPopupBackgroundEnd = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxPopupBackgroundEnd"));

        public static ComponentResourceKey ComboBoxPopupBorder => _comboBoxPopupBorder ??
                                                                 (_comboBoxPopupBorder = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxPopupBorder"));

        public static ComponentResourceKey ComboBoxSelection => _comboBoxSelection ??
                                                                (_comboBoxSelection = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxSelection"));

        public static ComponentResourceKey ComboBoxText => _comboBoxText ??
                                                           (_comboBoxText = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxText"));


        #endregion

        #region DropDown

        public static ComponentResourceKey DropDownBackground => _dropDownBackground ??
                                                                 (_dropDownBackground = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownBackground"));

        public static ComponentResourceKey DropDownBorder => _dropDownBorder ??
                                                             (_dropDownBorder = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownBorder"));

        public static ComponentResourceKey DropDownButtonMouseDownBackground => _dropDownButtonMouseDownBackground ??
                                                                                (_dropDownButtonMouseDownBackground = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonMouseDownBackground"));

        public static ComponentResourceKey DropDownButtonMouseDownSeparator => _dropDownButtonMouseDownSeparator ??
                                                                               (_dropDownButtonMouseDownSeparator = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonMouseDownSeparator"));

        public static ComponentResourceKey DropDownButtonMouseOverBackground => _dropDownButtonMouseOverBackground ??
                                                                                (_dropDownButtonMouseOverBackground = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonMouseOverBackground"));

        public static ComponentResourceKey DropDownButtonMouseOverSeparator => _dropDownButtonMouseOverSeparator ??
                                                                               (_dropDownButtonMouseOverSeparator = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonMouseOverSeparator"));

        public static ComponentResourceKey DropDownDisabledBackground => _dropDownDisabledBackground ??
                                                                         (_dropDownDisabledBackground = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownDisabledBackground"));

        public static ComponentResourceKey DropDownDisabledBorder => _dropDownDisabledBorder ??
                                                                     (_dropDownDisabledBorder = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownDisabledBorder"));

        public static ComponentResourceKey DropDownDisabledGlyph => _dropDownDisabledGlyph ??
                                                                    (_dropDownDisabledGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownDisabledGlyph"));

        public static ComponentResourceKey DropDownDisabledText => _dropDownDisabledText ??
                                                                   (_dropDownDisabledText = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownDisabledText"));

        public static ComponentResourceKey DropDownGlyph => _dropDownGlyph ??
                                                            (_dropDownGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownGlyph"));

        public static ComponentResourceKey DropDownMouseDownBackground => _dropDownMouseDownBackground ??
                                                                          (_dropDownMouseDownBackground = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownMouseDownBackground"));

        public static ComponentResourceKey DropDownMouseDownBorder => _dropDownMouseDownBorder ??
                                                                      (_dropDownMouseDownBorder = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownMouseDownBorder"));

        public static ComponentResourceKey DropDownMouseDownGlyph => _dropDownMouseDownGlyph ??
                                                                     (_dropDownMouseDownGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownMouseDownGlyph"));

        public static ComponentResourceKey DropDownMouseDownText => _dropDownMouseDownText ??
                                                                    (_dropDownMouseDownText = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownMouseDownText"));

        public static ComponentResourceKey DropDownMouseOverBackgroundBegin => _dropDownMouseOverBackgroundBegin ??
                                                                               (_dropDownMouseOverBackgroundBegin = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownMouseOverBackgroundBegin"));

        public static ComponentResourceKey DropDownMouseOverBackgroundEnd => _dropDownMouseOverBackgroundEnd ??
                                                                             (_dropDownMouseOverBackgroundEnd = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownMouseOverBackgroundEnd"));

        public static ComponentResourceKey DropDownMouseOverBackgroundMiddle1 => _dropDownMouseOverBackgroundMiddle1 ??
                                                                                 (_dropDownMouseOverBackgroundMiddle1 = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownMouseOverBackgroundMiddle1"));

        public static ComponentResourceKey DropDownMouseOverBackgroundMiddle2 => _dropDownMouseOverBackgroundMiddle2 ??
                                                                                 (_dropDownMouseOverBackgroundMiddle2 = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownMouseOverBackgroundMiddle2"));

        public static ComponentResourceKey DropDownMouseOverBorder => _dropDownMouseOverBorder ??
                                                                      (_dropDownMouseOverBorder = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownMouseOverBorder"));

        public static ComponentResourceKey DropDownMouseOverGlyph => _dropDownMouseOverGlyph ??
                                                                     (_dropDownMouseOverGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownMouseOverGlyph"));

        public static ComponentResourceKey DropDownMouseOverText => _dropDownMouseOverText ??
                                                                    (_dropDownMouseOverText = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownMouseOverText"));

        public static ComponentResourceKey DropDownPopupBackgroundBegin => _dropDownPopupBackgroundBegin ??
                                                                           (_dropDownPopupBackgroundBegin = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownPopupBackgroundBegin"));

        public static ComponentResourceKey DropDownPopupBackgroundEnd => _dropDownPopupBackgroundEnd ??
                                                                         (_dropDownPopupBackgroundEnd = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownPopupBackgroundEnd"));

        public static ComponentResourceKey DropDownPopupBorder => _dropDownPopupBorder ??
                                                                  (_dropDownPopupBorder = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownPopupBorder"));

        public static ComponentResourceKey DropDownText => _dropDownText ??
                                                           (_dropDownText = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownText"));

        public static ComponentResourceKey DropShadowBackground => _dropShadowBackground ??
                                                                   (_dropShadowBackground = new ComponentResourceKey(typeof(EnvironmentColors), "DropShadowBackground"));

        #endregion

        #region Separator

        public static ComponentResourceKey SeparatorBackground => _separatorBackground ??
                                                                  (_separatorBackground =
                                                                      new ComponentResourceKey(typeof(EnvironmentColors), "SeparatorBackground"));

        #endregion

        #region ScrollBar

        public static ComponentResourceKey ScrollBarArrowBackground => _scrollBarArrowBackground ??
                                                                       (_scrollBarArrowBackground =
                                                                           new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarArrowBackground"));
        public static ComponentResourceKey ScrollBarArrowDisabledBackground => _scrollBarArrowDisabledBackground ??
                                                                               (_scrollBarArrowDisabledBackground =
                                                                                   new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarArrowDisabledBackground"));
        public static ComponentResourceKey ScrollBarArrowGlyph => _scrollBarArrowGlyph ??
                                                                  (_scrollBarArrowGlyph =
                                                                      new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarArrowGlyph"));
        public static ComponentResourceKey ScrollBarArrowGlyphDisabled => _scrollBarArrowGlyphDisabled ??
                                                                          (_scrollBarArrowGlyphDisabled =
                                                                              new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarArrowGlyphDisabled"));

        public static ComponentResourceKey ScrollBarArrowGlyphMouseOver => _scrollBarArrowGlyphMouseOver ??
                                                                           (_scrollBarArrowGlyphMouseOver =
                                                                               new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarArrowGlyphMouseOver"));

        public static ComponentResourceKey ScrollBarArrowGlyphPressed => _scrollBarArrowGlyphPressed ??
                                                                         (_scrollBarArrowGlyphPressed =
                                                                             new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarArrowGlyphPressed"));

        public static ComponentResourceKey ScrollBarArrowMouseOverBackground => _scrollBarArrowMouseOverBackground ??
                                                                                (_scrollBarArrowMouseOverBackground =
                                                                                    new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarArrowMouseOverBackground"));

        public static ComponentResourceKey ScrollBarArrowPressedBackground => _scrollBarArrowPressedBackground ??
                                                                              (_scrollBarArrowPressedBackground =
                                                                                  new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarArrowPressedBackground"));

        public static ComponentResourceKey ScrollBarBackground => _scrollBarBackground ??
                                                                  (_scrollBarBackground =
                                                                      new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarBackground"));

        public static ComponentResourceKey ScrollBarBorder => _scrollBarBorder ??
                                                              (_scrollBarBorder =
                                                                  new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarBorder"));

        public static ComponentResourceKey ScrollBarDisabledBackground => _scrollBarDisabledBackground ??
                                                                          (_scrollBarDisabledBackground =
                                                                              new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarDisabledBackground"));


        public static ComponentResourceKey ScrollBarThumbBackground => _scrollBarThumbBackground ??
                                                                       (_scrollBarThumbBackground =
                                                                           new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarThumbBackground"));

        public static ComponentResourceKey ScrollBarThumbBorder => _scrollBarThumbBorder ??
                                                                   (_scrollBarThumbBorder =
                                                                       new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarThumbBorder"));

        public static ComponentResourceKey ScrollBarThumbDisabled => _scrollBarThumbDisabled ??
                                                                     (_scrollBarThumbDisabled =
                                                                         new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarThumbDisabled"));

        public static ComponentResourceKey ScrollBarThumbGlyph => _scrollBarThumbGlyph ??
                                                                  (_scrollBarThumbGlyph =
                                                                      new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarThumbGlyph"));

        public static ComponentResourceKey ScrollBarThumbGlyphMouseOverBorder => _scrollBarThumbGlyphMouseOverBorder ??
                                                                                 (_scrollBarThumbGlyphMouseOverBorder =
                                                                                     new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarThumbGlyphMouseOverBorder"));

        public static ComponentResourceKey ScrollBarThumbGlyphPressedBorder => _scrollBarThumbGlyphPressedBorder ??
                                                                               (_scrollBarThumbGlyphPressedBorder =
                                                                                   new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarThumbGlyphPressedBorder"));

        public static ComponentResourceKey ScrollBarThumbMouseOverBackground => _scrollBarThumbMouseOverBackground ??
                                                                                (_scrollBarThumbMouseOverBackground =
                                                                                    new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarThumbMouseOverBackground"));

        public static ComponentResourceKey ScrollBarThumbMouseOverBorder => _scrollBarThumbMouseOverBorder ??
                                                                            (_scrollBarThumbMouseOverBorder =
                                                                                new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarThumbMouseOverBorder"));

        public static ComponentResourceKey ScrollBarThumbPressedBackground => _scrollBarThumbPressedBackground ??
                                                                              (_scrollBarThumbPressedBackground =
                                                                                  new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarThumbPressedBackground"));

        public static ComponentResourceKey ScrollBarThumbPressedBorder => _scrollBarThumbPressedBorder ??
                                                                          (_scrollBarThumbPressedBorder =
                                                                              new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarThumbPressedBorder"));

        #endregion

        #region TextBox
        public static ComponentResourceKey TextBoxBorder => _textBoxBorder ??
                                                            (_textBoxBorder = new ComponentResourceKey(typeof(EnvironmentColors), "TextBoxBorder"));

        public static ComponentResourceKey TextBoxSelection => _textBoxSelection ??
                                                               (_textBoxSelection = new ComponentResourceKey(typeof(EnvironmentColors), "TextBoxSelection"));

        public static ComponentResourceKey TextBoxBackground => _textBoxBackground ??
                                                                (_textBoxBackground = new ComponentResourceKey(typeof(EnvironmentColors), "TextBoxBackground"));

        public static ComponentResourceKey TextBoxForeground => _textBoxForeground ??
                                                                (_textBoxForeground = new ComponentResourceKey(typeof(EnvironmentColors), "TextBoxForeground"));

        public static ComponentResourceKey TextBoxBorderHover => _textBoxBorderHover ??
                                                                 (_textBoxBorderHover = new ComponentResourceKey(typeof(EnvironmentColors), "TextBoxBorderHover"));

        public static ComponentResourceKey TextBoxBackgroundHover => _textBoxBackgroundHover ??
                                                                     (_textBoxBackgroundHover = new ComponentResourceKey(typeof(EnvironmentColors), "TextBoxBackgroundHover"));

        public static ComponentResourceKey TextBoxForegroundHover => _textBoxForegroundHover ??
                                                                     (_textBoxForegroundHover = new ComponentResourceKey(typeof(EnvironmentColors), "TextBoxForegroundHover"));

        public static ComponentResourceKey TextBoxBorderFocused => _textBoxBorderFocused ??
                                                                   (_textBoxBorderFocused = new ComponentResourceKey(typeof(EnvironmentColors), "TextBoxBorderFocused"));

        public static ComponentResourceKey TextBoxBackgroundFocused => _textBoxBackgroundFocused ??
                                                                       (_textBoxBackgroundFocused = new ComponentResourceKey(typeof(EnvironmentColors), "TextBoxBackgroundFocused"));

        public static ComponentResourceKey TextBoxForegroundFocused => _textBoxForegroundFocused ??
                                                                       (_textBoxForegroundFocused = new ComponentResourceKey(typeof(EnvironmentColors), "TextBoxForegroundFocused"));

        public static ComponentResourceKey TextBoxBorderDisabled => _textBoxBorderDisabled ??
                                                                    (_textBoxBorderDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "TextBoxBorderDisabled"));

        public static ComponentResourceKey TextBoxBackgroundDisabled => _textBoxBackgroundDisabled ??
                                                                        (_textBoxBackgroundDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "TextBoxBackgroundDisabled"));

        public static ComponentResourceKey TextBoxForegroundDisabled => _textBoxForegroundDisabled ??
                                                                        (_textBoxForegroundDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "TextBoxForegroundDisabled"));

        #endregion

        #region TitleBarButton
        public static ComponentResourceKey TitleBarButtonBackground => _titleBarButtonBackground ??
                                                                       (_titleBarButtonBackground =
                                                                           new ComponentResourceKey(typeof(EnvironmentColors), "TitleBarButtonBackground"));

        public static ComponentResourceKey TitleBarButtonBorder => _titleBarButtonBorder ??
                                                                   (_titleBarButtonBorder =
                                                                       new ComponentResourceKey(typeof(EnvironmentColors), "TitleBarButtonBorder"));

        public static ComponentResourceKey TitleBarButtonForeground => _titleBarButtonForeground ??
                                                                       (_titleBarButtonForeground =
                                                                           new ComponentResourceKey(typeof(EnvironmentColors), "TitleBarButtonForeground"));

        public static ComponentResourceKey TitleBarButtonBackgroundHover => _titleBarButtonBackgroundHover ??
                                                                            (_titleBarButtonBackgroundHover =
                                                                                new ComponentResourceKey(typeof(EnvironmentColors), "TitleBarButtonBackgroundHover"));

        public static ComponentResourceKey TitleBarButtonBorderHover => _titleBarButtonBorderHover ??
                                                                        (_titleBarButtonBorderHover =
                                                                            new ComponentResourceKey(typeof(EnvironmentColors), "TitleBarButtonBorderHover"));

        public static ComponentResourceKey TitleBarButtonForegroundHover => _titleBarButtonForegroundHover ??
                                                                            (_titleBarButtonForegroundHover =
                                                                                new ComponentResourceKey(typeof(EnvironmentColors), "TitleBarButtonForegroundHover"));

        public static ComponentResourceKey TitleBarButtonBackgroundDown => _titleBarButtonBackgroundDown ??
                                                                           (_titleBarButtonBackgroundDown =
                                                                               new ComponentResourceKey(typeof(EnvironmentColors), "TitleBarButtonBackgroundDown"));

        public static ComponentResourceKey TitleBarButtonBorderDown => _titleBarButtonBorderDown ??
                                                                       (_titleBarButtonBorderDown =
                                                                           new ComponentResourceKey(typeof(EnvironmentColors), "TitleBarButtonBorderDown"));

        public static ComponentResourceKey TitleBarButtonForegroundDown => _titleBarButtonForegroundDown ??
                                                                           (_titleBarButtonForegroundDown =
                                                                               new ComponentResourceKey(typeof(EnvironmentColors), "TitleBarButtonForegroundDown"));

        public static ComponentResourceKey TitleBarButtonBackgroundDisabled => _titleBarButtonBackgroundDisabled ??
                                                                               (_titleBarButtonBackgroundDisabled =
                                                                                   new ComponentResourceKey(typeof(EnvironmentColors), "TitleBarButtonBackgroundDisabled"));

        public static ComponentResourceKey TitleBarButtonBorderDisabled => _titleBarButtonBorderDisabled ??
                                                                           (_titleBarButtonBorderDisabled =
                                                                               new ComponentResourceKey(typeof(EnvironmentColors), "TitleBarButtonBorderDisabled"));

        public static ComponentResourceKey TitleBarButtonForegroundDisabled => _titleBarButtonForegroundDisabled ??
                                                                               (_titleBarButtonForegroundDisabled =
                                                                                   new ComponentResourceKey(typeof(EnvironmentColors), "TitleBarButtonForegroundDisabed"));

        #endregion

        #region CheckBox
        public static ComponentResourceKey CheckBoxBackground => _checkBoxBackground ??
                                                                 (_checkBoxBackground = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxBackground"));

        public static ComponentResourceKey CheckBoxBorder => _checkBoxBorder ??
                                                             (_checkBoxBorder = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxBorder"));

        public static ComponentResourceKey CheckBoxGlyph => _checkBoxGlyph ??
                                                            (_checkBoxGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxGlyph"));

        public static ComponentResourceKey CheckBoxText => _checkBoxText ??
                                                           (_checkBoxText = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxText"));

        public static ComponentResourceKey CheckBoxBackgroundHover => _checkBoxBackgroundHover ??
                                                                      (_checkBoxBackgroundHover = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxBackgroundHover"));

        public static ComponentResourceKey CheckBoxBorderHover => _checkBoxBorderHover ??
                                                                  (_checkBoxBorderHover = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxBorderHover"));

        public static ComponentResourceKey CheckBoxGlyphHover => _checkBoxGlyphHover ??
                                                                 (_checkBoxGlyphHover = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxGlyphHover"));

        public static ComponentResourceKey CheckBoxTextHover => _checkBoxTextHover ??
                                                                (_checkBoxTextHover = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxTextHover"));

        public static ComponentResourceKey CheckBoxBackgroundDown => _checkBoxBackgroundDown ??
                                                                     (_checkBoxBackgroundDown = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxBackgroundDown"));

        public static ComponentResourceKey CheckBoxBorderDown => _checkBoxBorderDown ??
                                                                 (_checkBoxBorderDown = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxBorderDown"));

        public static ComponentResourceKey CheckBoxGlyphDown => _checkBoxGlyphDown ??
                                                                (_checkBoxGlyphDown = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxGlyphDown"));

        public static ComponentResourceKey CheckBoxTextDown => _checkBoxTextDown ??
                                                               (_checkBoxTextDown = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxTextDown"));

        public static ComponentResourceKey CheckBoxBackgroundFocused => _checkBoxBackgroundFocused ??
                                                                        (_checkBoxBackgroundFocused = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxBackgroundFocused"));

        public static ComponentResourceKey CheckBoxBorderFocused => _checkBoxBorderFocused ??
                                                                    (_checkBoxBorderFocused = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxBorderFocused"));

        public static ComponentResourceKey CheckBoxGlyphFocused => _checkBoxGlyphFocused ??
                                                                   (_checkBoxGlyphFocused = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxGlyphFocused"));

        public static ComponentResourceKey CheckBoxTextFocused => _checkBoxTextFocused ??
                                                                  (_checkBoxTextFocused = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxTextFocused"));

        #endregion

        #region MainWindow
        public static ComponentResourceKey MainWindowBackground => _mainWindowBackground ??
                                                                   (_mainWindowBackground =
                                                                       new ComponentResourceKey(typeof(EnvironmentColors), "MainWindowBackground"));

        public static ComponentResourceKey MainWindowActiveShadowAndBorderColor => _mainWindowActiveShadowAndBorderColor ??
                                                                                   (_mainWindowActiveShadowAndBorderColor =
                                                                                       new ComponentResourceKey(typeof(EnvironmentColors), "MainWindowActiveShadowAndBorderColor"));

        public static ComponentResourceKey MainWindowInactiveShadowAndBorderColor => _mainWindowInactiveShadowAndBorderColor ??
                                                                                     (_mainWindowInactiveShadowAndBorderColor =
                                                                                         new ComponentResourceKey(typeof(EnvironmentColors), "MainWindowInactiveShadowAndBorderColor"));

        public static ComponentResourceKey MainWindowTitleBarBackground => _mainWindowTitleBarBackground ??
                                                                           (_mainWindowTitleBarBackground =
                                                                               new ComponentResourceKey(typeof(EnvironmentColors), "MainWindowTitleBarBackground"));

        public static ComponentResourceKey MainWindowTitleBarForeground => _mainWindowTitleBarForeground ??
                                                                           (_mainWindowTitleBarForeground =
                                                                               new ComponentResourceKey(typeof(EnvironmentColors), "MainWindowTitleBarForeground"));

        public static ComponentResourceKey MainWindowTitleBarForegroundInactive
            =>
                _mainWindowTitleBarForegroundInactive ??
                (_mainWindowTitleBarForegroundInactive =
                    new ComponentResourceKey(typeof(EnvironmentColors), "MainWindowTitleBarForegroundInactive"));

        #endregion

        #region ResizeGrip
        public static ComponentResourceKey ResizeGripColor1 => _resizeGripColor1 ??
                                                               (_resizeGripColor1 = new ComponentResourceKey(typeof(EnvironmentColors), "ResizeGripColor1"));

        public static ComponentResourceKey ResizeGripColor2 => _resizeGripColor2 ??
                                                               (_resizeGripColor2 = new ComponentResourceKey(typeof(EnvironmentColors), "ResizeGripColor2"));

        #endregion

        #region WindowTitleBarButton
        public static ComponentResourceKey WindowTitleBarButtonBackground => _windowTitleBarButtonBackground ??
                                                                             (_windowTitleBarButtonBackground =
                                                                                 new ComponentResourceKey(typeof(EnvironmentColors), "WindowTitleBarButtonBackground"));

        public static ComponentResourceKey WindowTitleBarButtonBorder => _windowTitleBarButtonBorder ??
                                                                         (_windowTitleBarButtonBorder =
                                                                             new ComponentResourceKey(typeof(EnvironmentColors), "WindowTitleBarButtonBorder"));

        public static ComponentResourceKey WindowTitleBarButtonForeground => _windowTitleBarButtonForeground ??
                                                                             (_windowTitleBarButtonForeground =
                                                                                 new ComponentResourceKey(typeof(EnvironmentColors), "WindowTitleBarButtonForeground"));

        public static ComponentResourceKey WindowTitleBarButtonHoverBackground => _windowTitleBarButtonHoverBackground ??
                                                                                  (_windowTitleBarButtonHoverBackground =
                                                                                      new ComponentResourceKey(typeof(EnvironmentColors), "WindowTitleBarButtonHoverBackground"));

        public static ComponentResourceKey WindowTitleBarButtonHoverBorder => _windowTitleBarButtonHoverBorder ??
                                                                              (_windowTitleBarButtonHoverBorder =
                                                                                  new ComponentResourceKey(typeof(EnvironmentColors), "WindowTitleBarButtonHoverBorder"));

        public static ComponentResourceKey WindowTitleBarButtonHoverForeground => _windowTitleBarButtonHoverForeground ??
                                                                                  (_windowTitleBarButtonHoverForeground =
                                                                                      new ComponentResourceKey(typeof(EnvironmentColors), "WindowTitleBarButtonHoverForeground"));

        public static ComponentResourceKey WindowTitleBarButtonDownBackground => _windowTitleBarButtonDownBackground ??
                                                                                 (_windowTitleBarButtonDownBackground =
                                                                                     new ComponentResourceKey(typeof(EnvironmentColors), "WindowTitleBarButtonDownBackground"));

        public static ComponentResourceKey WindowTitleBarButtonDownBorder => _windowTitleBarButtonDownBorder ??
                                                                             (_windowTitleBarButtonDownBorder =
                                                                                 new ComponentResourceKey(typeof(EnvironmentColors), "WindowTitleBarButtonDownBorder"));

        public static ComponentResourceKey WindowTitleBarButtonDownForeground => _windowTitleBarButtonDownForeground ??
                                                                                 (_windowTitleBarButtonDownForeground =
                                                                                     new ComponentResourceKey(typeof(EnvironmentColors), "WindowTitleBarButtonDownForeground"));

        #endregion

        #region WindowBase
        public static ComponentResourceKey WindowBaseBackground => _windowBaseBackground ??
                                                                   (_windowBaseBackground =
                                                                       new ComponentResourceKey(typeof(EnvironmentColors), "WindowBaseBackground"));

        public static ComponentResourceKey WindowBaseForeground => _windowBaseForeground ??
                                                                   (_windowBaseForeground =
                                                                       new ComponentResourceKey(typeof(EnvironmentColors), "WindowBaseForeground"));

        #endregion

        #region LayoutAutoHideWindowControl
        public static ComponentResourceKey LayoutAutoHideWindowBackground
           =>
               _layoutAutoHideWindowBackground ?? (_layoutAutoHideWindowBackground =
               (new ComponentResourceKey(typeof(EnvironmentColors), "LayoutAutoHideWindowBackground")));

        public static ComponentResourceKey LayoutAutoHideWindowBorder
            =>
                _layoutAutoHideWindowBorder ??
                (_layoutAutoHideWindowBorder =
                    new ComponentResourceKey(typeof(EnvironmentColors), "LayoutAutoHideWindowBorder"));
        #endregion

        #region LayoutGridResizer

        public static ComponentResourceKey LayoutGridResizerBackground
            =>
                _layoutGridResizerBackground ??
                (_layoutGridResizerBackground =
                    new ComponentResourceKey(typeof(EnvironmentColors), "LayoutGridResizerBackground"));

        public static ComponentResourceKey LayoutGridResizerBackgroundHorizontal
            =>
                _layoutGridResizerBackgroundHorizontal ??
                (_layoutGridResizerBackgroundHorizontal =
                    new ComponentResourceKey(typeof(EnvironmentColors), "LayoutGridResizerBackgroundHorizontal"));

        #endregion

        #region DockingManager
        public static ComponentResourceKey DockingManagerBackground
            =>
                _dockingManagerBackground ??
                (_dockingManagerBackground = new ComponentResourceKey(typeof(EnvironmentColors), "DockingManagerBackground"));
        #endregion

        #region OverlayWindow
        public static ComponentResourceKey OverlayWindowPreviewBoxBackground
            =>
                _overlayWindowPreviewBoxBackground ??
                (_overlayWindowPreviewBoxBackground =
                    new ComponentResourceKey(typeof(EnvironmentColors), "OverlayWindowPreviewBoxBackground"));

        public static ComponentResourceKey OverlayWindowPreviewBoxBorder
            =>
                _overlayWindowPreviewBoxBorder ??
                (_overlayWindowPreviewBoxBorder =
                    new ComponentResourceKey(typeof(EnvironmentColors), "OverlayWindowPreviewBoxBorder"));
        #endregion

        #region NavigatorWindow
        public static ComponentResourceKey NavigatorWindowBackground
            =>
                _navigatorWindowBackground ??
                (_navigatorWindowBackground = new ComponentResourceKey(typeof(EnvironmentColors), "NavigatorWindowBackground"));

        public static ComponentResourceKey NavigatorWindowBorder
            =>
                _navigatorWindowBorder ??
                (_navigatorWindowBorder = new ComponentResourceKey(typeof(EnvironmentColors), "NavigatorWindowBorder"));

        public static ComponentResourceKey NavigatorWindowTextForeground
            =>
                _navigatorWindowTextForeground ??
                (_navigatorWindowTextForeground = new ComponentResourceKey(typeof(EnvironmentColors), "NavigatorWindowTextForeground"));

        public static ComponentResourceKey NavigatorWindowItemBackground
            =>
                _navigatorWindowItemBackground ??
                (_navigatorWindowItemBackground = new ComponentResourceKey(typeof(EnvironmentColors), "NavigatorWindowItemBackground"));

        public static ComponentResourceKey NavigatorWindowItemForeground
            =>
                _navigatorWindowItemForeground ??
                (_navigatorWindowItemForeground = new ComponentResourceKey(typeof(EnvironmentColors), "NavigatorWindowItemForeground"));

        public static ComponentResourceKey NavigatorWindowItemBackgroundChecked
            =>
                _navigatorWindowItemBackgroundChecked ??
                (_navigatorWindowItemBackgroundChecked = new ComponentResourceKey(typeof(EnvironmentColors), "NavigatorWindowItemBackgroundChecked"));

        public static ComponentResourceKey NavigatorWindowItemForegroundChecked
            =>
                _navigatorWindowItemForegroundChecked ??
                (_navigatorWindowItemForegroundChecked = new ComponentResourceKey(typeof(EnvironmentColors), "NavigatorWindowItemForegroundChecked"));

        public static ComponentResourceKey NavigatorWindowTitleText
    =>
        _navigatorWindowTitleText ??
        (_navigatorWindowTitleText = new ComponentResourceKey(typeof(EnvironmentColors), "NavigatorWindowTitleText"));

        public static ComponentResourceKey NavigatorWindowItemBorder
            =>
                _navigatorWindowItemBorder ??
                (_navigatorWindowItemBorder = new ComponentResourceKey(typeof(EnvironmentColors), "NavigatorWindowItemBorder"));

        public static ComponentResourceKey NavigatorWindowItemBorderChecked
            =>
                _navigatorWindowItemBorderChecked ??
                (_navigatorWindowItemBorderChecked = new ComponentResourceKey(typeof(EnvironmentColors), "NavigatorWindowItemBorderChecked"));

        #endregion

        #region LayoutAnchorableFloatingWindowControl

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarBackground
            =>
                _anchorableFloatingWindowTitleBarBackground ??
                (_anchorableFloatingWindowTitleBarBackground =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarBackground"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarBackgroundActive
            =>
                _anchorableFloatingWindowTitleBarBackgroundActive ??
                (_anchorableFloatingWindowTitleBarBackgroundActive =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarBackgroundActive"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarGlyph
            =>
                _anchorableFloatingWindowTitleBarGlyph ??
                (_anchorableFloatingWindowTitleBarGlyph =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarGlyph"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarGlyphActive
            =>
                _anchorableFloatingWindowTitleBarGlyphActive ??
                (_anchorableFloatingWindowTitleBarGlyphActive =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarGlyphActive"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarGlyphHover
            =>
                _anchorableFloatingWindowTitleBarGlyphHover ??
                (_anchorableFloatingWindowTitleBarGlyphHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarGlyphHover"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarGlyphDown
            =>
                _anchorableFloatingWindowTitleBarGlyphDown ??
                (_anchorableFloatingWindowTitleBarGlyphDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarGlyphDown"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarGlyphActiveHover
            =>
                _anchorableFloatingWindowTitleBarGlyphActiveHover ??
                (_anchorableFloatingWindowTitleBarGlyphActiveHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarGlyphActiveHover"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarGlyphActiveDown
            =>
                _anchorableFloatingWindowTitleBarGlyphActiveDown ??
                (_anchorableFloatingWindowTitleBarGlyphActiveDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarGlyphActiveDown"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarForeground
            =>
                _anchorableFloatingWindowTitleBarForeground ??
                (_anchorableFloatingWindowTitleBarForeground =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarForeground"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarForegroundActive
            =>
                _anchorableFloatingWindowTitleBarForegroundActive ??
                (_anchorableFloatingWindowTitleBarForegroundActive =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarForegroundActive"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBackground
            =>
                _anchorableFloatingWindowTitleBarButtonBackground ??
                (_anchorableFloatingWindowTitleBarButtonBackground =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarButtonBackground"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBackgroundHover
            =>
                _anchorableFloatingWindowTitleBarButtonBackgroundHover ??
                (_anchorableFloatingWindowTitleBarButtonBackgroundHover
            =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarButtonBackgroundHover"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBackgroundDown
            =>
                _anchorableFloatingWindowTitleBarButtonBackgroundDown ??
                (_anchorableFloatingWindowTitleBarButtonBackgroundDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarButtonBackgroundDown"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBorder
            =>
                _anchorableFloatingWindowTitleBarButtonBorder ??
                (_anchorableFloatingWindowTitleBarButtonBorder =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarButtonBorder"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBorderHover
            =>
                _anchorableFloatingWindowTitleBarButtonBorderHover ??
                (_anchorableFloatingWindowTitleBarButtonBorderHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarButtonBorderHover"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBorderDown
            =>
                _anchorableFloatingWindowTitleBarButtonBorderDown ??
                (_anchorableFloatingWindowTitleBarButtonBorderDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarButtonBorderDown"));

        public static ComponentResourceKey AnchorableFloatingWindowGrip
            =>
                _anchorableFloatingWindowGrip ??
                (_anchorableFloatingWindowGrip =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowGrip"));

        public static ComponentResourceKey AnchorableFloatingWindowGripActive
            =>
                _anchorableFloatingWindowGripActive ??
                (_anchorableFloatingWindowGripActive =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowGripActive"));


        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBackgroundActive
            =>
                _anchorableFloatingWindowTitleBarButtonBackgroundActive ??
                (_anchorableFloatingWindowTitleBarButtonBackgroundActive =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarButtonBackgroundActive"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBackgroundActiveHover
            =>
                _anchorableFloatingWindowTitleBarButtonBackgroundActiveHover ??
                (_anchorableFloatingWindowTitleBarButtonBackgroundActiveHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarButtonBackgroundActiveHover"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBackgroundActiveDown
            =>
                _anchorableFloatingWindowTitleBarButtonBackgroundActiveDown ??
                (_anchorableFloatingWindowTitleBarButtonBackgroundActiveDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarButtonBackgroundActiveDown"));


        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBorderActive
            =>
                _anchorableFloatingWindowTitleBarButtonBorderActive ??
                (_anchorableFloatingWindowTitleBarButtonBorderActive =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarButtonBorderActive"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBorderActiveHover
            =>
                _anchorableFloatingWindowTitleBarButtonBorderActiveHover ??
                (_anchorableFloatingWindowTitleBarButtonBorderActiveHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarButtonBorderActiveHover"));

        public static ComponentResourceKey AnchorableFloatingWindowTitleBarButtonBorderActiveDown
            =>
                _anchorableFloatingWindowTitleBarButtonBorderActiveDown ??
                (_anchorableFloatingWindowTitleBarButtonBorderActiveDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableFloatingWindowTitleBarButtonBorderActiveDown"));

        #endregion

        #region FloatingWindow

        public static ComponentResourceKey FloatingWindowBackground
            =>
                _floatingWindowBackground ??
                (_floatingWindowBackground = new ComponentResourceKey(typeof(EnvironmentColors), "FloatingWindowBackground"));


        public static ComponentResourceKey FloatingWindowTitleBarBackground
            =>
                _floatingWindowTitleBarBackground ??
                (_floatingWindowTitleBarBackground = new ComponentResourceKey(typeof(EnvironmentColors), "FloatingWindowTitleBarBackground"));

        public static ComponentResourceKey FloatingWindowTitleBarGlyph
            =>
                _floatingWindowTitleBarGlyph ??
                (_floatingWindowTitleBarGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "FloatingWindowTitleBarGlyph"));

        public static ComponentResourceKey FloatingWindowTitleBarGlyphHover
            =>
                _floatingWindowTitleBarGlyphHover ??
                (_floatingWindowTitleBarGlyphHover = new ComponentResourceKey(typeof(EnvironmentColors), "FloatingWindowTitleBarGlyphHover"));

        public static ComponentResourceKey FloatingWindowTitleBarGlyphDown
            =>
                _floatingWindowTitleBarGlyphDown ??
                (_floatingWindowTitleBarGlyphDown = new ComponentResourceKey(typeof(EnvironmentColors), "FloatingWindowTitleBarGlyphDown"));

        public static ComponentResourceKey FloatingWindowTitleBarForeground
            =>
                _floatingWindowTitleBarForeground ??
                (_floatingWindowTitleBarForeground = new ComponentResourceKey(typeof(EnvironmentColors), "FloatingWindowTitleBarForeground"));

        public static ComponentResourceKey FloatingWindowTitleBarForegroundActive
            =>
                _floatingWindowTitleBarForegroundActive ??
                (_floatingWindowTitleBarForegroundActive = new ComponentResourceKey(typeof(EnvironmentColors), "FloatingWindowTitleBarForegroundActive"));

        #endregion

        #region DocumentTabItem

        public static ComponentResourceKey DocumentTabItemText
            =>
                _documentTabItemText ??
                (_documentTabItemText = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemText"));

        public static ComponentResourceKey DocumentTabItemTextHover
            =>
                _documentTabItemTextHover ??
                (_documentTabItemTextHover
            = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemTextHover"));

        public static ComponentResourceKey DocumentTabItemTextActive
            =>
                _documentTabItemTextActive ??
                (_documentTabItemTextActive = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemTextActive"));

        public static ComponentResourceKey DocumentTabItemTextLastActive
            =>
                _documentTabItemTextLastActive ??
                (_documentTabItemTextLastActive = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemTextLastActive"));

        public static ComponentResourceKey DocumentTabItemGlyph
            =>
                _documentTabItemGlyph ??
                (_documentTabItemGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemGlyph"));

        public static ComponentResourceKey DocumentTabItemGlyphHover
            =>
                _documentTabItemGlyphHover ??
                (_documentTabItemGlyphHover = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemGlyphHover"));

        public static ComponentResourceKey DocumentTabItemGlyphDown
            =>
                _documentTabItemGlyphDown ??
                (_documentTabItemGlyphDown = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemGlyphDown"));

        public static ComponentResourceKey DocumentTabItemGlyphActive
            =>
                _documentTabItemGlyphActive ??
                (_documentTabItemGlyphActive = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemGlyphActive"));

        public static ComponentResourceKey DocumentTabItemGlyphActiveHover
            =>
                _documentTabItemGlyphActiveHover ??
                (_documentTabItemGlyphActiveHover = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemGlyphActiveHover"));

        public static ComponentResourceKey DocumentTabItemGlyphActiveDown
            =>
                _documentTabItemGlyphActiveDown ??
                (_documentTabItemGlyphActiveDown = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemGlyphActiveDown"));

        public static ComponentResourceKey DocumentTabItemGlyphLastActive
            =>
                _documentTabItemGlyphLastActive ??
                (_documentTabItemGlyphLastActive = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemGlyphLastActive"));

        public static ComponentResourceKey DocumentTabItemGlyphLastActiveHover
            =>
                _documentTabItemGlyphLastActiveHover ??
                (_documentTabItemGlyphLastActiveHover = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemGlyphLastActiveHover"));

        public static ComponentResourceKey DocumentTabItemGlyphLastActiveDown
            =>
                _documentTabItemGlyphLastActiveDown ??
                (_documentTabItemGlyphLastActiveDown = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemGlyphLastActiveDown"));

        public static ComponentResourceKey DocumentTabItemButtonBackground
           =>
               _documentTabItemButtonBackground ??
               (_documentTabItemButtonBackground =
                   new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBackground"));

        public static ComponentResourceKey DocumentTabItemButtonBackgroundHover
            =>
                _documentTabItemButtonBackgroundHover ??
                (_documentTabItemButtonBackgroundHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBackgroundHover"));

        public static ComponentResourceKey DocumentTabItemButtonBackgroundDown
            =>
                _documentTabItemButtonBackgroundDown ??
                (_documentTabItemButtonBackgroundDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBackgroundDown"));

        public static ComponentResourceKey DocumentTabItemButtonBackgroundActive
            =>
                _documentTabItemButtonBackgroundActive ??
                (_documentTabItemButtonBackgroundActive =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBackgroundActive"));

        public static ComponentResourceKey DocumentTabItemButtonBackgroundActiveHover
            =>
                _documentTabItemButtonBackgroundActiveHover ??
                (_documentTabItemButtonBackgroundActiveHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBackgroundActiveHover"));

        public static ComponentResourceKey DocumentTabItemButtonBackgroundActiveDown
            =>
                _documentTabItemButtonBackgroundActiveDown ??
                (_documentTabItemButtonBackgroundActiveDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBackgroundActiveDown"));

        public static ComponentResourceKey DocumentTabItemButtonBackgroundLastActive
    =>
        _documentTabItemButtonBackgroundLastActive ??
        (_documentTabItemButtonBackgroundLastActive =
            new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBackgroundLastActive"));

        public static ComponentResourceKey DocumentTabItemButtonBackgroundLastActiveHover
            =>
                _documentTabItemButtonBackgroundLastActiveHover ??
                (_documentTabItemButtonBackgroundLastActiveHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBackgroundLastActiveHover"));

        public static ComponentResourceKey DocumentTabItemButtonBackgroundLastActiveDown
            =>
                _documentTabItemButtonBackgroundLastActiveDown ??
                (_documentTabItemButtonBackgroundLastActiveDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBackgroundLastActiveDown"));

        public static ComponentResourceKey DocumentTabItemButtonBorder
    =>
        _documentTabItemButtonBorder ??
        (_documentTabItemButtonBorder =
            new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBorder"));

        public static ComponentResourceKey DocumentTabItemButtonBorderHover
            =>
                _documentTabItemButtonBorderHover ??
                (_documentTabItemButtonBorderHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBorderHover"));

        public static ComponentResourceKey DocumentTabItemButtonBorderDown
            =>
                _documentTabItemButtonBorderDown ??
                (_documentTabItemButtonBorderDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBorderDown"));


        public static ComponentResourceKey DocumentTabItemButtonBorderActive
            =>
                _documentTabItemButtonBorderActive ??
                (_documentTabItemButtonBorderActive =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBorderActive"));

        public static ComponentResourceKey DocumentTabItemButtonBorderActiveHover
            =>
                _documentTabItemButtonBorderActiveHover ??
                (_documentTabItemButtonBorderActiveHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBorderActiveHover"));

        public static ComponentResourceKey DocumentTabItemButtonBorderActiveDown
            =>
                _documentTabItemButtonBorderActiveDown ??
                (_documentTabItemButtonBorderActiveDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBorderActiveDown"));


        public static ComponentResourceKey DocumentTabItemButtonBorderLastActive
    =>
        _documentTabItemButtonBorderLastActive ??
        (_documentTabItemButtonBorderLastActive =
            new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBorderLastActive"));

        public static ComponentResourceKey DocumentTabItemButtonBorderLastActiveHover
            =>
                _documentTabItemButtonBorderLastActiveHover ??
                (_documentTabItemButtonBorderLastActiveHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBorderLastActiveHover"));

        public static ComponentResourceKey DocumentTabItemButtonBorderLastActiveDown
            =>
                _documentTabItemButtonBorderLastActiveDown ??
                (_documentTabItemButtonBorderLastActiveDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemButtonBorderLastActiveDown"));

        public static ComponentResourceKey DocumentTabItemBackground
            =>
                _documentTabItemBackground ??
                (_documentTabItemBackground = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemBackground"));

        public static ComponentResourceKey DocumentTabItemBackgroundHover
            =>
                _documentTabItemBackgroundHover ??
                (_documentTabItemBackgroundHover = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemBackgroundHover"));

        public static ComponentResourceKey DocumentTabItemBackgroundActive
            =>
                _documentTabItemBackgroundActive ??
                (_documentTabItemBackgroundActive = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemBackgroundActive"));

        public static ComponentResourceKey DocumentTabItemBackgroundLastActive
            =>
                _documentTabItemBackgroundLastActive ??
                (_documentTabItemBackgroundLastActive = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemBackgroundLastActive"));

        public static ComponentResourceKey DocumentTabItemBackgroundDisabled
            =>
                _documentTabItemBackgroundDisabled ??
                (_documentTabItemBackgroundDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemBackgroundDisabled"));

        public static ComponentResourceKey DocumentTabItemBorder
           =>
               _documentTabItemBorder ??
               (_documentTabItemBorder = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemBorder"));

        public static ComponentResourceKey DocumentTabItemBorderHover
            =>
                _documentTabItemBorderHover ??
                (_documentTabItemBorderHover = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemBorderHover"));

        public static ComponentResourceKey DocumentTabItemBorderActive
            =>
                _documentTabItemBorderActive ??
                (_documentTabItemBorderActive = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemBorderActive"));

        public static ComponentResourceKey DocumentTabItemBorderLastActive
            =>
                _documentTabItemBorderLastActive ??
                (_documentTabItemBorderLastActive = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemBorderLastActive"));

        public static ComponentResourceKey DocumentTabItemBorderDisabled
            =>
                _documentTabItemBorderDisabled ??
                (_documentTabItemBorderDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentTabItemBorderDisabled"));

        #endregion

        #region AnchorTabItem
        public static ComponentResourceKey AnchorTabItemBackground
           =>
               _anchorTabItemBackground ??
               (_anchorTabItemBackground = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorTabItemBackground"));

        public static ComponentResourceKey AnchorTabItemBackgroundHover
            =>
                _anchorTabItemBackgroundHover ??
                (_anchorTabItemBackgroundHover = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorTabItemBackgroundHover"));

        public static ComponentResourceKey AnchorTabItemBackgroundActive
            =>
                _anchorTabItemBackgroundActive ??
                (_anchorTabItemBackgroundActive = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorTabItemBackgroundActive"));

        public static ComponentResourceKey AnchorTabItemBackgroundDisabled
            =>
                _anchorTabItemBackgroundDisabled ??
                (_anchorTabItemBackgroundDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorTabItemBackgroundDisabled"));

        public static ComponentResourceKey AnchorTabItemBorder
           =>
               _anchorTabItemBorder ??
               (_anchorTabItemBorder = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorTabItemBorder"));

        public static ComponentResourceKey AnchorTabItemBorderHover
           =>
               _anchorTabItemBorderHover ??
               (_anchorTabItemBorderHover = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorTabItemBorderHover"));

        public static ComponentResourceKey AnchorTabItemBorderActive
           =>
               _anchorTabItemBorderActive ??
               (_anchorTabItemBorderActive = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorTabItemBorderActive"));

        public static ComponentResourceKey AnchorTabItemBorderDisabled
           =>
               _anchorTabItemBorderDisabled ??
               (_anchorTabItemBorderDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorTabItemBorderDisabled"));

        public static ComponentResourceKey AnchorTabItemText
           =>
               _anchorTabItemText ??
               (_anchorTabItemText = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorTabItemText"));

        public static ComponentResourceKey AnchorTabItemTextHover
           =>
               _anchorTabItemTextHover ??
               (_anchorTabItemTextHover = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorTabItemTextHover"));

        public static ComponentResourceKey AnchorTabItemTextActive
           =>
               _anchorTabItemTextActive ??
               (_anchorTabItemTextActive = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorTabItemTextActive"));

        public static ComponentResourceKey AnchorTabItemTextDisabled
           =>
               _anchorTabItemTextDisabled ??
               (_anchorTabItemTextDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorTabItemTextDisabled"));
        #endregion

        #region AnchorPaneControl
        public static ComponentResourceKey AnchorPaneControlBorder
            =>
                _anchorPaneControlBorder ??
                (_anchorPaneControlBorder = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneControlBorder"));

        #endregion

        #region LayoutAnchorableControl

        public static ComponentResourceKey AnchorableControlBackground
            =>
                _anchorableControlBackground ??
                (_anchorableControlBackground =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorableControlBackground"));
        #endregion

        #region AnchorSideItem

        public static ComponentResourceKey AnchorSideItemBackground
            =>
                _anchorSideItemBackground ??
                (_anchorSideItemBackground = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorSideItemBackground"));

        public static ComponentResourceKey AnchorSideItemBackgroundHover
            =>
                _anchorSideItemBackgroundHover ??
                (_anchorSideItemBackgroundHover = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorSideItemBackgroundHover"));

        public static ComponentResourceKey AnchorSideItemBorder
            =>
                _anchorSideItemBorder ??
                (_anchorSideItemBorder = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorSideItemBorder"));

        public static ComponentResourceKey AnchorSideItemBorderHover
            =>
                _anchorSideItemBorderHover ??
                (_anchorSideItemBorderHover = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorSideItemBorderHover"));

        public static ComponentResourceKey AnchorSideItemForeground
            =>
                _anchorSideItemForeground ??
                (_anchorSideItemForeground = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorSideItemForeground"));
        public static ComponentResourceKey AnchorSideItemForegroundHover
            =>
                _anchorSideItemForegroundHover ??
                (_anchorSideItemForegroundHover = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorSideItemForegroundHover"));

        #endregion

        #region AnchorPaneTitle



        public static ComponentResourceKey AnchorPaneTitleBackgroundActive
            =>
                _anchorPaneTitleBackgroundActive ??
                (_anchorPaneTitleBackgroundActive = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleBackgroundActive"));

        public static ComponentResourceKey AnchorPaneTitleGlyph
            =>
                _anchorPaneTitleGlyph ??
                (_anchorPaneTitleGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleGlyph"));

        public static ComponentResourceKey AnchorPaneTitleGlyphHover
            =>
                _anchorPaneTitleGlyphHover ??
                (_anchorPaneTitleGlyphHover = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleGlyphHover"));

        public static ComponentResourceKey AnchorPaneTitleGlyphDown
            =>
                _anchorPaneTitleGlyphDown ??
                (_anchorPaneTitleGlyphDown = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleGlyphDown"));

        public static ComponentResourceKey AnchorPaneTitleGlyphActive
           =>
               _anchorPaneTitleGlyphActive ??
               (_anchorPaneTitleGlyphActive = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleGlyphActive"));

        public static ComponentResourceKey AnchorPaneTitleGlyphActiveHover
            =>
                _anchorPaneTitleGlyphActiveHover ??
                (_anchorPaneTitleGlyphActiveHover = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleGlyphActiveHover"));

        public static ComponentResourceKey AnchorPaneTitleGlyphActiveDown
            =>
                _anchorPaneTitleGlyphActiveDown ??
                (_anchorPaneTitleGlyphActiveDown = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleGlyphActiveDown"));

        public static ComponentResourceKey AnchorPaneTitleButtonBackground
            =>
                _anchorPaneTitleButtonBackground ??
                (_anchorPaneTitleButtonBackground =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleButtonBackground"));

        public static ComponentResourceKey AnchorPaneTitleButtonBackgroundHover
            =>
                _anchorPaneTitleButtonBackgroundHover ??
                (_anchorPaneTitleButtonBackgroundHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleButtonBackgroundHover"));

        public static ComponentResourceKey AnchorPaneTitleButtonBackgroundDown
            =>
                _anchorPaneTitleButtonBackgroundDown ??
                (_anchorPaneTitleButtonBackgroundDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleButtonBackgroundDown"));

        public static ComponentResourceKey AnchorPaneTitleButtonBackgroundActive
            =>
                _anchorPaneTitleButtonBackgroundActive ??
                (_anchorPaneTitleButtonBackgroundActive =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleButtonBackground"));

        public static ComponentResourceKey AnchorPaneTitleButtonBackgroundActiveHover
            =>
                _anchorPaneTitleButtonBackgroundActiveHover ??
                (_anchorPaneTitleButtonBackgroundActiveHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleButtonBackgroundActiveHover"));

        public static ComponentResourceKey AnchorPaneTitleButtonBackgroundActiveDown
            =>
                _anchorPaneTitleButtonBackgroundActiveDown ??
                (_anchorPaneTitleButtonBackgroundActiveDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleButtonBackgroundActiveDown"));

        public static ComponentResourceKey AnchorPaneTitleButtonBorder
            =>
                _anchorPaneTitleButtonBorder ??
                (_anchorPaneTitleButtonBorder =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleButtonBorder"));

        public static ComponentResourceKey AnchorPaneTitleButtonBorderHover
            =>
                _anchorPaneTitleButtonBorderHover ??
                (_anchorPaneTitleButtonBorderHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleButtonBorderHover"));

        public static ComponentResourceKey AnchorPaneTitleButtonBorderDown
            =>
                _anchorPaneTitleButtonBorderDown ??
                (_anchorPaneTitleButtonBorderDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleButtonBorderHover"));

        public static ComponentResourceKey AnchorPaneTitleButtonBorderActive
            =>
                _anchorPaneTitleButtonBorderActive ??
                (_anchorPaneTitleButtonBorderActive =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleButtonBorderActive"));

        public static ComponentResourceKey AnchorPaneTitleButtonBorderActiveHover
            =>
                _anchorPaneTitleButtonBorderActiveHover ??
                (_anchorPaneTitleButtonBorderActiveHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleButtonBorderActiveHover"));

        public static ComponentResourceKey AnchorPaneTitleButtonBorderActiveDown
            =>
                _anchorPaneTitleButtonBorderActiveDown ??
                (_anchorPaneTitleButtonBorderActiveDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleButtonBorderActiveDown"));

        public static ComponentResourceKey AnchorPaneTitleGrip
            =>
                _anchorPaneTitleGrip ??
                (_anchorPaneTitleGrip = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleGrip"));

        public static ComponentResourceKey AnchorPaneTitleGripActive
            =>
                _anchorPaneTitleGripActive ??
                (_anchorPaneTitleGripActive = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleGripActive"));

        public static ComponentResourceKey AnchorPaneTitleText
            =>
                _anchorPaneTitleText ??
                (_anchorPaneTitleText = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleText"));

        public static ComponentResourceKey AnchorPaneTitleTextActive
            =>
                _anchorPaneTitleTextActive ??
                (_anchorPaneTitleTextActive = new ComponentResourceKey(typeof(EnvironmentColors), "AnchorPaneTitleTextActive"));


        #endregion

        #region DocumentPaneControl

        public static ComponentResourceKey DocumentPaneControlBackground
            =>
                _documentPaneControlBackground ??
                (_documentPaneControlBackground =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentPaneControlBackground"));

        public static ComponentResourceKey DocumentPaneControlBackgroundLastActive
            =>
                _documentPaneControlBackgroundLastActive ??
                (_documentPaneControlBackgroundLastActive =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentPaneControlBackgroundLastActive"));

        public static ComponentResourceKey DocumentPaneControlGlyph
            =>
                _documentPaneControlGlyph ??
                (_documentPaneControlGlyph =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentPaneControlGlyph"));

        public static ComponentResourceKey DocumentPaneControlGlyphHover
            =>
                _documentPaneControlGlyphHover ??
                (_documentPaneControlGlyphHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentPaneControlGlyphHover"));

        public static ComponentResourceKey DocumentPaneControlGlyphDown
            =>
                _documentPaneControlGlyphDown ??
                (_documentPaneControlGlyphDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentPaneControlGlyphDown"));

        public static ComponentResourceKey DocumentPaneControlButtonBackground
            =>
                _documentPaneControlButtonBackground ??
                (_documentPaneControlButtonBackground =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentPaneControlButtonBackground"));

        public static ComponentResourceKey DocumentPaneControlButtonBackgroundHover
            =>
                _documentPaneControlButtonBackgroundHover ??
                (_documentPaneControlButtonBackgroundHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentPaneControlButtonBackgroundHover"));

        public static ComponentResourceKey DocumentPaneControlButtonBackgroundDown
            =>
                _documentPaneControlButtonBackgroundDown ??
                (_documentPaneControlButtonBackgroundDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentPaneControlButtonBackgroundDown"));

        public static ComponentResourceKey DocumentPaneControlButtonBorder
            =>
                _documentPaneControlButtonBorder ??
                (_documentPaneControlButtonBorder =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentPaneControlButtonBorder"));

        public static ComponentResourceKey DocumentPaneControlButtonBorderHover
            =>
                _documentPaneControlButtonBorderHover ??
                (_documentPaneControlButtonBorderHover =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentPaneControlButtonBorderHover"));

        public static ComponentResourceKey DocumentPaneControlButtonBorderDown
            =>
                _documentPaneControlButtonBorderDown ??
                (_documentPaneControlButtonBorderDown =
                    new ComponentResourceKey(typeof(EnvironmentColors), "DocumentPaneControlButtonBorderDown"));

        public static ComponentResourceKey DocumentPaneControlBorder
            =>
                _documentPaneControlBorder ??
                (_documentPaneControlBorder = new ComponentResourceKey(typeof(EnvironmentColors), "DocumentPaneControlBorder"));

        #endregion
    }
}