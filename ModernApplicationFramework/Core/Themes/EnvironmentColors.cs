using System.Windows;

namespace ModernApplicationFramework.Core.Themes
{
    public static class EnvironmentColors
    {
        //Global
        private static ComponentResourceKey _dropDownGlyph;
        private static ComponentResourceKey _dropDownGlyphHover;
        private static ComponentResourceKey _dropDownGlyphDown;
        private static ComponentResourceKey _dropDownGlyphDisabled;

        private static ComponentResourceKey _gloablForeground;

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

        //ComboBox
        private static ComponentResourceKey _comboBoxBorder;
        private static ComponentResourceKey _comboBoxBackground;
        private static ComponentResourceKey _comboboxButtonBackground;
        private static ComponentResourceKey _comboBoxButtonBorder;
        private static ComponentResourceKey _comboBoxForeground;
        private static ComponentResourceKey _comboBoxGlyph;
        private static ComponentResourceKey _comboBoxSelectionBrush;
        private static ComponentResourceKey _comboBoxCaretBrush;

        private static ComponentResourceKey _comboBoxBorderHover;
        private static ComponentResourceKey _comboBoxBackgroundHover;
        private static ComponentResourceKey _comboboxButtonBackgroundHover;
        private static ComponentResourceKey _comboBoxButtonBorderHover;
        private static ComponentResourceKey _comboBoxForegroundHover;
        private static ComponentResourceKey _comboBoxGlyphHover;

        private static ComponentResourceKey _comboBoxBorderDown;
        private static ComponentResourceKey _comboBoxBackgroundDown;
        private static ComponentResourceKey _comboboxButtonBackgroundDown;
        private static ComponentResourceKey _comboBoxButtonBorderDown;
        private static ComponentResourceKey _comboBoxForegroundDown;
        private static ComponentResourceKey _comboBoxGlyphDown;

        private static ComponentResourceKey _comboBoxBorderDisabled;
        private static ComponentResourceKey _comboBoxBackgroundDisabled;
        private static ComponentResourceKey _comboboxButtonBackgroundDisabled;
        private static ComponentResourceKey _comboBoxButtonBorderDisabled;
        private static ComponentResourceKey _comboBoxForegroundDisabled;
        private static ComponentResourceKey _comboBoxGlyphDisabled;

        private static ComponentResourceKey _comboBoxPopupBorder;
        private static ComponentResourceKey _comboBoxPopupBackground;
        private static ComponentResourceKey _comboBoxPopupShadowBackground;

        //ComboxItem
        private static ComponentResourceKey _comboBoxItemBackgroundHover;
        private static ComponentResourceKey _comboBoxItemBorderHover;
        private static ComponentResourceKey _comboBoxItemForegroundHover;
        private static ComponentResourceKey _comboBoxItemForeground;
        private static ComponentResourceKey _comboBoxItemForegroundDisabled;

        //DropDownButton
        private static ComponentResourceKey _dropDownButtonBackground;
        private static ComponentResourceKey _dropDownButtonBorder;
        private static ComponentResourceKey _dropDownButtonForeground;
        private static ComponentResourceKey _dropDownButtonGlyph;

        private static ComponentResourceKey _dropDownButtonBackgroundHover;
        private static ComponentResourceKey _dropDownButtonBorderHover;
        private static ComponentResourceKey _dropDownButtonForegroundHover;
        private static ComponentResourceKey _dropDownButtonGlyphHover;

        private static ComponentResourceKey _dropDownButtonBackgroundDown;
        private static ComponentResourceKey _dropDownButtonBorderDown;
        private static ComponentResourceKey _dropDownButtonForegroundDown;
        private static ComponentResourceKey _dropDownButtonGlyphDown;

        private static ComponentResourceKey _dropDownButtonBackgroundDisabled;
        private static ComponentResourceKey _dropDownButtonBorderDisabled;
        private static ComponentResourceKey _dropDownButtonForegroundDisabled;
        private static ComponentResourceKey _dropDownButtonGlyphDisabled;

        //Menu
        private static ComponentResourceKey _menuBackground;
        private static ComponentResourceKey _menuForeground;
        private static ComponentResourceKey _menuSeparator;

        //MenuItem
        private static ComponentResourceKey _menuItemForeground;
        private static ComponentResourceKey _menuItemBackgroundHover;
        private static ComponentResourceKey _menuItemBorderHover;
        private static ComponentResourceKey _menuItemForegroundHover;
        private static ComponentResourceKey _menuItemBackgroundDown;
        private static ComponentResourceKey _menuItemBorderDown;
        private static ComponentResourceKey _menuItemForegroundDown;
        private static ComponentResourceKey _menuItemForegroundDisabled;
        private static ComponentResourceKey _menuItemIconForeground;
        private static ComponentResourceKey _menuItemIconForegroundHover;

        private static ComponentResourceKey _menuItemPopupBackground;
        private static ComponentResourceKey _menuItemPopupIconBackground;
        private static ComponentResourceKey _menuItemPopupBorder;
        private static ComponentResourceKey _menuItemPopupShadow;

        private static ComponentResourceKey _menuSubItemGlyph;
        private static ComponentResourceKey _menuSubItemForeground;
        private static ComponentResourceKey _menuSubItemGlyphHover;
        private static ComponentResourceKey _menuSubItemBackgroundHover;
        private static ComponentResourceKey _menuSubItemForegroundHover;
        private static ComponentResourceKey _menuSubItemGlyphDisabled;
        private static ComponentResourceKey _menuSubItemForegroundDisabled;

        //Separator
        private static ComponentResourceKey _separatorBackground;

        //ScrollBar
        private static ComponentResourceKey _scrollBarBackground;
        private static ComponentResourceKey _scrollBarBorder;
        private static ComponentResourceKey _scrollBarButtonBackground;
        private static ComponentResourceKey _scrollBarButtonGlyph;
        private static ComponentResourceKey _scrollBarButtonBackgroundHover;
        private static ComponentResourceKey _scrollBarButtonGlyphHover;
        private static ComponentResourceKey _scrollBarButtonBackgroundDown;
        private static ComponentResourceKey _scrollBarButtonGlyphDown;
        private static ComponentResourceKey _scrollBarButtonBackgroundDisabled;
        private static ComponentResourceKey _scrollBarButtonGlyphDisabled;
        private static ComponentResourceKey _scrollBarThumbBackground;
        private static ComponentResourceKey _scrollBarThumbBackgroundHover;
        private static ComponentResourceKey _scrollBarThumbBackgroundDown;
        private static ComponentResourceKey _scrollBarThumbBackgroundDisabled;

        //SplitButton
        private static ComponentResourceKey _splitButtonBorder;
        private static ComponentResourceKey _splitButtonBackground;
        private static ComponentResourceKey _splitButtonActionButtonBackground;
        private static ComponentResourceKey _splitButtonActionButtonForeground;
        private static ComponentResourceKey _splitButtonSeparator;
        private static ComponentResourceKey _splitButtonToggleButtonBackground;
        private static ComponentResourceKey _splitButtonToggleButtonGlyph;

        private static ComponentResourceKey _splitButtonBorderHover;
        private static ComponentResourceKey _splitButtonActionButtonBackgroundHover;
        private static ComponentResourceKey _splitButtonActionButtonForegroundHover;
        private static ComponentResourceKey _splitButtonSeparatorHover;
        private static ComponentResourceKey _splitButtonToggleButtonBackgroundHover;
        private static ComponentResourceKey _splitButtonToggleButtonGlyphHover;

        private static ComponentResourceKey _splitButtonActionButtonBackgroundDown;
        private static ComponentResourceKey _splitButtonActionButtonForegroundDown;
        private static ComponentResourceKey _splitButtonSeparatorDown;
        private static ComponentResourceKey _splitButtonToggleButtonDown;
        private static ComponentResourceKey _splitButtonActionButtonDown;

        public static ComponentResourceKey SplitButtonToggleButtonDown => _splitButtonToggleButtonDown ??
                                                                      (_splitButtonToggleButtonDown = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonToggleButtonDown"));

        public static ComponentResourceKey SplitButtonActionButtonDown => _splitButtonActionButtonDown ??
                                                                      (_splitButtonActionButtonDown = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonActionButtonDown"));

        private static ComponentResourceKey _splitButtonActionButtonBackgroundChecked;
        private static ComponentResourceKey _splitButtonActionButtonForegroundChecked;
        private static ComponentResourceKey _splitButtonSeparatorChecked;
        private static ComponentResourceKey _splitButtonToggleButtonBackgroundChecked;
        private static ComponentResourceKey _splitButtonToggleButtonGlyphChecked;

        private static ComponentResourceKey _splitButtonToggleButtonGlyphDisabled;
        private static ComponentResourceKey _splitButtonActionButtonForegroundDisabled;

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

        //ToolBarTray
        private static ComponentResourceKey _toolBarTrayBackground;

        //ToolBar
        private static ComponentResourceKey _toolBarBackground;
        private static ComponentResourceKey _toolBarBorder;
        private static ComponentResourceKey _toolBarGrip;
        private static ComponentResourceKey _toolBarButtonBackgroundDisabled;
        private static ComponentResourceKey _toolBarButtonBackgroundGlyphDisabled;

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

        //ContextMenu
        private static ComponentResourceKey _contextMenuBackground;
        private static ComponentResourceKey _contextMenuForeground;
        private static ComponentResourceKey _contextMenuIconBackground;
        private static ComponentResourceKey _contextMenuBorder;
        private static ComponentResourceKey _contextMenuShadow;

        //ContextMenuGlyphItem
        private static ComponentResourceKey _contextMenuGlyphItemIconForeground;
        private static ComponentResourceKey _contextMenuGlyphItemIconForegroundHover;

        //MainWindow
        private static ComponentResourceKey _mainWindowBackground;
        private static ComponentResourceKey _mainWindowActiveShadowAndBorderColor;
        private static ComponentResourceKey _mainWindowInactiveShadowAndBorderColor;
        private static ComponentResourceKey _mainWindowTitleBarBackground;
        private static ComponentResourceKey _mainWindowTitleBarForeground;
        private static ComponentResourceKey _mainWindowTitleBarForegroundInactive;

        //MenuHostControl
        private static ComponentResourceKey _menuHostControlBackground;

        //ToolBarHostControl
        private static ComponentResourceKey _toolBarHostControlTopDockBackground;
        private static ComponentResourceKey _toolBarHostControlDefaultDockBackground;

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

        public static ComponentResourceKey DropDownGlyph => _dropDownGlyph ??
                                                            (_dropDownGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownGlyph"));

        public static ComponentResourceKey DropDownGlyphHover => _dropDownGlyphHover ??
                                                                 (_dropDownGlyphHover = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownGlyphHover"));

        public static ComponentResourceKey DropDownGlyphDown => _dropDownGlyphDown ??
                                                                (_dropDownGlyphDown = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownGlyphDown"));

        public static ComponentResourceKey DropDownGlyphDisabled => _dropDownGlyphDisabled ??
                                                                    (_dropDownGlyphDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "DropDownGlyphDisabled"));

        public static ComponentResourceKey GloablForeground => _gloablForeground ??
                                                               (_gloablForeground = new ComponentResourceKey(typeof(EnvironmentColors), "GloablForeground"));

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

        public static ComponentResourceKey ComboBoxBorder => _comboBoxBorder ??
                                                             (_comboBoxBorder = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxBorder"));

        public static ComponentResourceKey ComboBoxBackground => _comboBoxBackground ??
                                                                 (_comboBoxBackground = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxBackground"));

        public static ComponentResourceKey ComboboxButtonBackground => _comboboxButtonBackground ??
                                                                       (_comboboxButtonBackground =
                                                                           new ComponentResourceKey(typeof(EnvironmentColors), "ComboboxButtonBackground"));

        public static ComponentResourceKey ComboBoxButtonBorder => _comboBoxButtonBorder ??
                                                                   (_comboBoxButtonBorder =
                                                                       new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxButtonBorder"));

        public static ComponentResourceKey ComboBoxForeground => _comboBoxForeground ??
                                                                 (_comboBoxForeground = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxForeground"));

        public static ComponentResourceKey ComboBoxArrow => _comboBoxGlyph ??
                                                            (_comboBoxGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxArrow"));

        public static ComponentResourceKey ComboBoxSelectionBrush => _comboBoxSelectionBrush ??
                                                                     (_comboBoxSelectionBrush =
                                                                         new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxSelectionBrush"));

        public static ComponentResourceKey ComboBoxCaretBrush => _comboBoxCaretBrush ??
                                                                 (_comboBoxCaretBrush = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxCaretBrush"));

        public static ComponentResourceKey ComboBoxBorderHover => _comboBoxBorderHover ??
                                                                  (_comboBoxBorderHover = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxBorderHover"));

        public static ComponentResourceKey ComboBoxBackgroundHover => _comboBoxBackgroundHover ??
                                                                      (_comboBoxBackgroundHover = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxBackgroundHover"));

        public static ComponentResourceKey ComboboxButtonBackgroundHover => _comboboxButtonBackgroundHover ??
                                                                            (_comboboxButtonBackgroundHover =
                                                                                new ComponentResourceKey(typeof(EnvironmentColors), "ComboboxButtonBackgroundHover"));

        public static ComponentResourceKey ComboBoxButtonBorderHover => _comboBoxButtonBorderHover ??
                                                                        (_comboBoxButtonBorderHover =
                                                                            new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxButtonBorderHover"));

        public static ComponentResourceKey ComboBoxForegroundHover => _comboBoxForegroundHover ??
                                                                      (_comboBoxForegroundHover = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxForegroundHover"));

        public static ComponentResourceKey ComboBoxArrowHover => _comboBoxGlyphHover ??
                                                                 (_comboBoxGlyphHover = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxArrowHover"));

        public static ComponentResourceKey ComboBoxBorderDown => _comboBoxBorderDown ??
                                                                 (_comboBoxBorderDown = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxBorderDown"));

        public static ComponentResourceKey ComboBoxBackgroundDown => _comboBoxBackgroundDown ??
                                                                     (_comboBoxBackgroundDown = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxBackgroundDown"));

        public static ComponentResourceKey ComboboxButtonBackgroundDown => _comboboxButtonBackgroundDown ??
                                                                           (_comboboxButtonBackgroundDown =
                                                                               new ComponentResourceKey(typeof(EnvironmentColors), "ComboboxButtonBackgroundDown"));

        public static ComponentResourceKey ComboBoxButtonBorderDown => _comboBoxButtonBorderDown ??
                                                                       (_comboBoxButtonBorderDown =
                                                                           new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxButtonBorderDown"));

        public static ComponentResourceKey ComboBoxForegroundDown => _comboBoxForegroundDown ??
                                                                     (_comboBoxForegroundDown = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxForegroundDown"));

        public static ComponentResourceKey ComboBoxArrowDown => _comboBoxGlyphDown ??
                                                                (_comboBoxGlyphDown = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxArrowDown"));

        public static ComponentResourceKey ComboBoxBorderDisabled => _comboBoxBorderDisabled ??
                                                                     (_comboBoxBorderDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxBorderDisabled"));

        public static ComponentResourceKey ComboBoxBackgroundDisabled => _comboBoxBackgroundDisabled ??
                                                                         (_comboBoxBackgroundDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxBackgroundDisabled"));

        public static ComponentResourceKey ComboboxButtonBackgroundDisabled => _comboboxButtonBackgroundDisabled ??
                                                                               (_comboboxButtonBackgroundDisabled =
                                                                                   new ComponentResourceKey(typeof(EnvironmentColors), "ComboboxButtonBackgroundDisabled"));

        public static ComponentResourceKey ComboBoxButtonBorderDisabled => _comboBoxButtonBorderDisabled ??
                                                                           (_comboBoxButtonBorderDisabled =
                                                                               new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxButtonBorderDisabled"));

        public static ComponentResourceKey ComboBoxForegroundDisabled => _comboBoxForegroundDisabled ??
                                                                         (_comboBoxForegroundDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxForegroundDisabled"));

        public static ComponentResourceKey ComboBoxArrowDisabled => _comboBoxGlyphDisabled ??
                                                                    (_comboBoxGlyphDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxArrowDisabled"));

        public static ComponentResourceKey ComboBoxPopupBorder => _comboBoxPopupBorder ??
                                                                  (_comboBoxPopupBorder = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxPopupBorder"));

        public static ComponentResourceKey ComboBoxPopupBackground => _comboBoxPopupBackground ??
                                                                      (_comboBoxPopupBackground = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxPopupBackground"));

        public static ComponentResourceKey ComboBoxPopupShadowBackground => _comboBoxPopupShadowBackground ??
                                                                            (_comboBoxPopupShadowBackground = new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxPopupShadowBackground"));

        #endregion

        #region ComboBoxItem

        public static ComponentResourceKey ComboBoxItemBackgroundHover => _comboBoxItemBackgroundHover ??
                                                                          (_comboBoxItemBackgroundHover =
                                                                              new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxItemBackgroundHover"));

        public static ComponentResourceKey ComboBoxItemBorderHover => _comboBoxItemBorderHover ??
                                                                      (_comboBoxItemBorderHover =
                                                                          new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxItemBorderHover"));

        public static ComponentResourceKey ComboBoxItemForeground => _comboBoxItemForeground ??
                                                                     (_comboBoxItemForeground =
                                                                         new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxItemForeground"));

        public static ComponentResourceKey ComboBoxItemForegroundHover => _comboBoxItemForegroundHover ??
                                                                          (_comboBoxItemForegroundHover =
                                                                              new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxItemForegroundHover"));

        public static ComponentResourceKey ComboBoxItemForegroundDisabled => _comboBoxItemForegroundDisabled ??
                                                                             (_comboBoxItemForegroundDisabled =
                                                                                 new ComponentResourceKey(typeof(EnvironmentColors), "ComboBoxItemForegroundDisabled"));

        #endregion

        #region DropDownButton

        public static ComponentResourceKey DropDownButtonBackground => _dropDownButtonBackground ??
                                                                       (_dropDownButtonBackground =
                                                                           new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonBackground"));

        public static ComponentResourceKey DropDownButtonBorder => _dropDownButtonBorder ??
                                                                   (_dropDownButtonBorder =
                                                                       new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonBorder"));

        public static ComponentResourceKey DropDownButtonForeground => _dropDownButtonForeground ??
                                                                       (_dropDownButtonForeground =
                                                                           new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonForeground"));

        public static ComponentResourceKey DropDownButtonGlyph => _dropDownButtonGlyph ??
                                                                  (_dropDownButtonGlyph =
                                                                      new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonGlyph"));

        public static ComponentResourceKey DropDownButtonBackgroundDisabled => _dropDownButtonBackgroundDisabled ??
                                                                               (_dropDownButtonBackgroundDisabled =
                                                                                   new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonBackgroundDisabled"));

        public static ComponentResourceKey DropDownButtonBorderDisabled => _dropDownButtonBorderDisabled ??
                                                                           (_dropDownButtonBorderDisabled =
                                                                               new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonBorderDisabled"));

        public static ComponentResourceKey DropDownButtonForegroundDisabled => _dropDownButtonForegroundDisabled ??
                                                                               (_dropDownButtonForegroundDisabled =
                                                                                   new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonForegroundDisabled"));

        public static ComponentResourceKey DropDownButtonGlyphDisabled => _dropDownButtonGlyphDisabled ??
                                                                          (_dropDownButtonGlyphDisabled =
                                                                              new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonGlyphDisabled"));

        public static ComponentResourceKey DropDownButtonBackgroundHover => _dropDownButtonBackgroundHover ??
                                                                            (_dropDownButtonBackgroundHover =
                                                                                new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonBackgroundHover"));

        public static ComponentResourceKey DropDownButtonBorderHover => _dropDownButtonBorderHover ??
                                                                        (_dropDownButtonBorderHover =
                                                                            new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonBorderHover"));

        public static ComponentResourceKey DropDownButtonForegroundHover => _dropDownButtonForegroundHover ??
                                                                            (_dropDownButtonForegroundHover =
                                                                                new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonForegroundHover"));

        public static ComponentResourceKey DropDownButtonGlyphHover => _dropDownButtonGlyphHover ??
                                                                       (_dropDownButtonGlyphHover =
                                                                           new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonGlyph"));

        public static ComponentResourceKey DropDownButtonBackgroundDown => _dropDownButtonBackgroundDown ??
                                                                           (_dropDownButtonBackgroundDown =
                                                                               new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonBackgroundDown"));

        public static ComponentResourceKey DropDownButtonBorderDown => _dropDownButtonBorderDown ??
                                                                       (_dropDownButtonBorderDown =
                                                                           new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonBorderDown"));

        public static ComponentResourceKey DropDownButtonForegroundDown => _dropDownButtonForegroundDown ??
                                                                           (_dropDownButtonForegroundDown =
                                                                               new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonForegroundDown"));

        public static ComponentResourceKey DropDownButtonGlyphDown => _dropDownButtonGlyphDown ??
                                                                      (_dropDownButtonGlyphDown =
                                                                          new ComponentResourceKey(typeof(EnvironmentColors), "DropDownButtonGlyphDown"));

        #endregion

        #region Menu

        public static ComponentResourceKey MenuBackground => _menuBackground ??
                                                             (_menuBackground = new ComponentResourceKey(typeof(EnvironmentColors), "MenuBackground"));

        public static ComponentResourceKey MenuForeground => _menuForeground ??
                                                             (_menuForeground = new ComponentResourceKey(typeof(EnvironmentColors), "MenuForeground"));

        public static ComponentResourceKey MenuSeparator => _menuSeparator ??
                                                            (_menuSeparator = new ComponentResourceKey(typeof(EnvironmentColors), "MenuSeparator"));

        #endregion

        #region MenuItem

        public static ComponentResourceKey MenuItemForeground => _menuItemForeground ??
                                                                 (_menuItemForeground = new ComponentResourceKey(typeof(EnvironmentColors), "MenuItemForeground"));

        public static ComponentResourceKey MenuItemBackgroundHover => _menuItemBackgroundHover ??
                                                                      (_menuItemBackgroundHover =
                                                                          new ComponentResourceKey(typeof(EnvironmentColors), "MenuItemBackgroundHover"));

        public static ComponentResourceKey MenuItemBorderHover => _menuItemBorderHover ??
                                                                  (_menuItemBorderHover =
                                                                      new ComponentResourceKey(typeof(EnvironmentColors), "MenuItemBorderHover"));

        public static ComponentResourceKey MenuItemForegroundHover => _menuItemForegroundHover ??
                                                                      (_menuItemForegroundHover =
                                                                          new ComponentResourceKey(typeof(EnvironmentColors), "MenuItemForegroundHover"));

        public static ComponentResourceKey MenuItemBackgroundDown => _menuItemBackgroundDown ??
                                                                     (_menuItemBackgroundDown =
                                                                         new ComponentResourceKey(typeof(EnvironmentColors), "MenuItemBackgroundDown"));

        public static ComponentResourceKey MenuItemBorderDown => _menuItemBorderDown ??
                                                                 (_menuItemBorderDown = new ComponentResourceKey(typeof(EnvironmentColors), "MenuItemBorderDown"));

        public static ComponentResourceKey MenuItemForegroundDown => _menuItemForegroundDown ??
                                                                     (_menuItemForegroundDown =
                                                                         new ComponentResourceKey(typeof(EnvironmentColors), "MenuItemForegroundDown"));

        public static ComponentResourceKey MenuItemForegroundDisabled => _menuItemForegroundDisabled ??
                                                                         (_menuItemForegroundDisabled =
                                                                             new ComponentResourceKey(typeof(EnvironmentColors), "MenuItemForegroundDisabled"));

        public static ComponentResourceKey MenuItemPopupBackground => _menuItemPopupBackground ??
                                                                      (_menuItemPopupBackground =
                                                                          new ComponentResourceKey(typeof(EnvironmentColors), "MenuItemPopupBackground"));

        public static ComponentResourceKey MenuItemPopupIconBackground => _menuItemPopupIconBackground ??
                                                                          (_menuItemPopupIconBackground =
                                                                              new ComponentResourceKey(typeof(EnvironmentColors), "MenuItemPopupIconBackground"));

        public static ComponentResourceKey MenuItemPopupBorder => _menuItemPopupBorder ??
                                                                  (_menuItemPopupBorder =
                                                                      new ComponentResourceKey(typeof(EnvironmentColors), "MenuItemPopupBorder"));

        public static ComponentResourceKey MenuItemIconForeground => _menuItemIconForeground ??
                                                                     (_menuItemIconForeground =
                                                                         new ComponentResourceKey(typeof(EnvironmentColors), "MenuItemIconForeground"));

        public static ComponentResourceKey MenuItemPopupShadow => _menuItemPopupShadow ??
                                                                  (_menuItemPopupShadow =
                                                                      new ComponentResourceKey(typeof(EnvironmentColors), "MenuItemPopupShadow"));

        public static ComponentResourceKey MenuItemIconForegroundHover => _menuItemIconForegroundHover ??
                                                                          (_menuItemIconForegroundHover =
                                                                              new ComponentResourceKey(typeof(EnvironmentColors), "MenuItemIconForegroundHover"));

        public static ComponentResourceKey MenuSubItemGlyph => _menuSubItemGlyph ??
                                                               (_menuSubItemGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "MenuSubItemGlyph"));

        public static ComponentResourceKey MenuSubItemGlyphHover => _menuSubItemGlyphHover ??
                                                                    (_menuSubItemGlyphHover = new ComponentResourceKey(typeof(EnvironmentColors), "MenuSubItemGlyphHover"));

        public static ComponentResourceKey MenuSubItemGlyphDisabled => _menuSubItemGlyphDisabled ??
                                                                       (_menuSubItemGlyphDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "MenuSubItemGlyphDisabled"));

        public static ComponentResourceKey MenuSubItemForeground => _menuSubItemForeground ??
                                                                    (_menuSubItemForeground =
                                                                        new ComponentResourceKey(typeof(EnvironmentColors), "MenuSubItemForeground"));

        public static ComponentResourceKey MenuSubItemForegroundHover => _menuSubItemForegroundHover ??
                                                                         (_menuSubItemForegroundHover =
                                                                             new ComponentResourceKey(typeof(EnvironmentColors), "MenuSubItemForegroundHover"));

        public static ComponentResourceKey MenuSubItemForegroundDisabled => _menuSubItemForegroundDisabled ??
                                                                            (_menuSubItemForegroundDisabled =
                                                                                new ComponentResourceKey(typeof(EnvironmentColors), "MenuSubItemForegroundDisabled"));

        public static ComponentResourceKey MenuSubItemBackgroundHover => _menuSubItemBackgroundHover ??
                                                                         (_menuSubItemBackgroundHover =
                                                                             new ComponentResourceKey(typeof(EnvironmentColors), "MenuSubItemBackgroundHover"));

        #endregion

        #region Separator

        public static ComponentResourceKey SeparatorBackground => _separatorBackground ??
                                                                  (_separatorBackground =
                                                                      new ComponentResourceKey(typeof(EnvironmentColors), "SeparatorBackground"));

        #endregion

        #region ScrollBar
        public static ComponentResourceKey ScrollBarBackground => _scrollBarBackground ??
                                                                  (_scrollBarBackground =
                                                                      new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarBackground"));

        public static ComponentResourceKey ScrollBarBorder => _scrollBarBorder ??
                                                              (_scrollBarBorder =
                                                                  new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarBorder"));

        public static ComponentResourceKey ScrollBarButtonBackground => _scrollBarButtonBackground ??
                                                                        (_scrollBarButtonBackground =
                                                                            new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarButtonBackground"));

        public static ComponentResourceKey ScrollBarButtonGlyph => _scrollBarButtonGlyph ??
                                                                   (_scrollBarButtonGlyph =
                                                                       new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarButtonGlyph"));

        public static ComponentResourceKey ScrollBarButtonBackgroundHover => _scrollBarButtonBackgroundHover ??
                                                                             (_scrollBarButtonBackgroundHover =
                                                                                 new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarButtonBackgroundHover"));

        public static ComponentResourceKey ScrollBarButtonGlyphHover => _scrollBarButtonGlyphHover ??
                                                                        (_scrollBarButtonGlyphHover =
                                                                            new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarButtonGlyphHover"));

        public static ComponentResourceKey ScrollBarButtonBackgroundDown => _scrollBarButtonBackgroundDown ??
                                                                            (_scrollBarButtonBackgroundDown =
                                                                                new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarButtonBackgroundDown"));

        public static ComponentResourceKey ScrollBarButtonGlyphDown => _scrollBarButtonGlyphDown ??
                                                                       (_scrollBarButtonGlyphDown =
                                                                           new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarButtonGlyphDown"));

        public static ComponentResourceKey ScrollBarButtonBackgroundDisabled => _scrollBarButtonBackgroundDisabled ??
                                                                                (_scrollBarButtonBackgroundDisabled =
                                                                                    new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarButtonBackgroundDisabled"));

        public static ComponentResourceKey ScrollBarButtonGlyphDisabled => _scrollBarButtonGlyphDisabled ??
                                                                           (_scrollBarButtonGlyphDisabled =
                                                                               new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarButtonGlyphDisabled"));

        public static ComponentResourceKey ScrollBarThumbBackground => _scrollBarThumbBackground ??
                                                                       (_scrollBarThumbBackground =
                                                                           new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarThumbBackground"));

        public static ComponentResourceKey ScrollBarThumbBackgroundHover => _scrollBarThumbBackgroundHover ??
                                                                            (_scrollBarThumbBackgroundHover =
                                                                                new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarThumbBackgroundHover"));

        public static ComponentResourceKey ScrollBarThumbBackgroundDown => _scrollBarThumbBackgroundDown ??
                                                                           (_scrollBarThumbBackgroundDown =
                                                                               new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarThumbBackgroundDown"));

        public static ComponentResourceKey ScrollBarThumbBackgroundDisabled => _scrollBarThumbBackgroundDisabled ??
                                                                               (_scrollBarThumbBackgroundDisabled =
                                                                                   new ComponentResourceKey(typeof(EnvironmentColors), "ScrollBarThumbBackgroundDisabled"));

        #endregion

        #region SplitButton
        public static ComponentResourceKey SplitButtonBorder => _splitButtonBorder ??
                                                                (_splitButtonBorder = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonBorder"));

        public static ComponentResourceKey SplitButtonBackground => _splitButtonBackground ??
                                                                    (_splitButtonBackground =
                                                                        new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonBackground"));

        public static ComponentResourceKey SplitButtonActionButtonBackground => _splitButtonActionButtonBackground ??
                                                                                (_splitButtonActionButtonBackground = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonActionButtonBackground"));

        public static ComponentResourceKey SplitButtonActionButtonForeground => _splitButtonActionButtonForeground ??
                                                                                (_splitButtonActionButtonForeground = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonActionButtonForeground"));

        public static ComponentResourceKey SplitButtonSeparator => _splitButtonSeparator ??
                                                                   (_splitButtonSeparator = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonSeparator"));

        public static ComponentResourceKey SplitButtonToggleButtonBackground => _splitButtonToggleButtonBackground ??
                                                                                (_splitButtonToggleButtonBackground = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonToggleButtonBackground"));

        public static ComponentResourceKey SplitButtonToggleButtonGlyph => _splitButtonToggleButtonGlyph ??
                                                                           (_splitButtonToggleButtonGlyph = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonToggleButtonGlyph"));

        public static ComponentResourceKey SplitButtonBorderHover => _splitButtonBorderHover ??
                                                                     (_splitButtonBorderHover = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonBorderHover"));

        public static ComponentResourceKey SplitButtonActionButtonBackgroundHover => _splitButtonActionButtonBackgroundHover ??
                                                                                     (_splitButtonActionButtonBackgroundHover = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonActionButtonBackgroundHover"));

        public static ComponentResourceKey SplitButtonActionButtonForegroundHover => _splitButtonActionButtonForegroundHover ??
                                                                                     (_splitButtonActionButtonForegroundHover = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonActionButtonForegroundHover"));

        public static ComponentResourceKey SplitButtonSeparatorHover => _splitButtonSeparatorHover ??
                                                                        (_splitButtonSeparatorHover = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonSeparatorHover"));

        public static ComponentResourceKey SplitButtonToggleButtonBackgroundHover => _splitButtonToggleButtonBackgroundHover ??
                                                                                     (_splitButtonToggleButtonBackgroundHover = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonToggleButtonBackgroundHover"));

        public static ComponentResourceKey SplitButtonToggleButtonGlyphHover => _splitButtonToggleButtonGlyphHover ??
                                                                                (_splitButtonToggleButtonGlyphHover = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonToggleButtonGlyphHover"));

        public static ComponentResourceKey SplitButtonActionButtonBackgroundDown => _splitButtonActionButtonBackgroundDown ??
                                                                                    (_splitButtonActionButtonBackgroundDown = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonActionButtonBackgroundDown"));

        public static ComponentResourceKey SplitButtonActionButtonForegroundDown => _splitButtonActionButtonForegroundDown ??
                                                                                    (_splitButtonActionButtonForegroundDown = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonActionButtonForegroundDown"));

        public static ComponentResourceKey SplitButtonSeparatorDown => _splitButtonSeparatorDown ??
                                                                       (_splitButtonSeparatorDown = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonSeparatorDown"));

        public static ComponentResourceKey SplitButtonActionButtonBackgroundChecked => _splitButtonActionButtonBackgroundChecked ??
                                                                                       (_splitButtonActionButtonBackgroundChecked = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonActionButtonBackgroundChecked"));

        public static ComponentResourceKey SplitButtonActionButtonForegroundChecked => _splitButtonActionButtonForegroundChecked ??
                                                                                       (_splitButtonActionButtonForegroundChecked = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonActionButtonForegroundChecked"));

        public static ComponentResourceKey SplitButtonSeparatorChecked => _splitButtonSeparatorChecked ??
                                                                          (_splitButtonSeparatorChecked = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonSeparatorChecked"));

        public static ComponentResourceKey SplitButtonToggleButtonBackgroundChecked => _splitButtonToggleButtonBackgroundChecked ??
                                                                                       (_splitButtonToggleButtonBackgroundChecked = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonToggleButtonBackgroundChecked"));

        public static ComponentResourceKey SplitButtonToggleButtonGlyphChecked => _splitButtonToggleButtonGlyphChecked ??
                                                                                  (_splitButtonToggleButtonGlyphChecked = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonToggleButtonGlyphChecked"));

        public static ComponentResourceKey SplitButtonToggleButtonGlyphDisabled => _splitButtonToggleButtonGlyphDisabled ??
                                                                                   (_splitButtonToggleButtonGlyphDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonToggleButtonGlyphDisabled"));

        public static ComponentResourceKey SplitButtonActionButtonForegroundDisabled => _splitButtonActionButtonForegroundDisabled ??
                                                                                        (_splitButtonActionButtonForegroundDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "SplitButtonActionButtonForegroundDisabled"));

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

        #region ToolBarTray

        public static ComponentResourceKey ToolBarTrayBackground => _toolBarTrayBackground ??
                                                                    (_toolBarTrayBackground =
                                                                        new ComponentResourceKey(typeof(EnvironmentColors), "ToolBarTrayBackground"));

        #endregion

        #region ToolBar
        public static ComponentResourceKey ToolBarBackground => _toolBarBackground ??
                                                                (_toolBarBackground = new ComponentResourceKey(typeof(EnvironmentColors), "ToolBarBackground"));

        public static ComponentResourceKey ToolBarBorder => _toolBarBorder ??
                                                            (_toolBarBorder = new ComponentResourceKey(typeof(EnvironmentColors), "ToolBarBorder"));

        public static ComponentResourceKey ToolBarGrip => _toolBarGrip ??
                                                          (_toolBarGrip = new ComponentResourceKey(typeof(EnvironmentColors), "ToolBarGrip"));

        public static ComponentResourceKey ToolBarButtonBackgroundDisabled => _toolBarButtonBackgroundDisabled ??
                                                                              (_toolBarButtonBackgroundDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "ToolBarButtonBackgroundDisabled"));

        public static ComponentResourceKey ToolBarButtonBackgroundGlyphDisabled => _toolBarButtonBackgroundGlyphDisabled ??
                                                                                   (_toolBarButtonBackgroundGlyphDisabled = new ComponentResourceKey(typeof(EnvironmentColors), "ToolBarButtonBackgroundGlyphDisabled"));

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
                                                                    (_checkBoxBorderFocused = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxBoderFocused"));

        public static ComponentResourceKey CheckBoxGlyphFocused => _checkBoxGlyphFocused ??
                                                                   (_checkBoxGlyphFocused = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxGlyphFocused"));

        public static ComponentResourceKey CheckBoxTextFocused => _checkBoxTextFocused ??
                                                                  (_checkBoxTextFocused = new ComponentResourceKey(typeof(EnvironmentColors), "CheckBoxTextFocused"));

        #endregion

        #region ContextMenu
        public static ComponentResourceKey ContextMenuBackground => _contextMenuBackground ??
                                                                    (_contextMenuBackground =
                                                                        new ComponentResourceKey(typeof(EnvironmentColors), "ContextMenuBackground"));

        public static ComponentResourceKey ContextMenuForeground => _contextMenuForeground ??
                                                                    (_contextMenuForeground =
                                                                        new ComponentResourceKey(
                                                                            typeof (EnvironmentColors),
                                                                            "ContextMenuForeground"));

        public static ComponentResourceKey ContextMenuIconBackground => _contextMenuIconBackground ??
                                                                        (_contextMenuIconBackground =
                                                                            new ComponentResourceKey(typeof(EnvironmentColors), "ContextMenuIconBackground"));

        public static ComponentResourceKey ContextMenuBorder => _contextMenuBorder ??
                                                                (_contextMenuBorder = new ComponentResourceKey(typeof(EnvironmentColors), "ContextMenuBorder"));

        public static ComponentResourceKey ContextMenuShadow => _contextMenuShadow ??
                                                                (_contextMenuShadow = new ComponentResourceKey(typeof(EnvironmentColors), "ContextMenuShadow"));

        #endregion

        #region ContextMenuGlyphItem
        public static ComponentResourceKey ContextMenuGlyphItemIconForeground => _contextMenuGlyphItemIconForeground ??
                                                                                 (_contextMenuGlyphItemIconForeground =
                                                                                     new ComponentResourceKey(typeof(EnvironmentColors), "ContextMenuGlyphItemIconForeground"));

        public static ComponentResourceKey ContextMenuGlyphItemIconForegroundHover => _contextMenuGlyphItemIconForegroundHover ??
                                                                                      (_contextMenuGlyphItemIconForegroundHover =
                                                                                          new ComponentResourceKey(typeof(EnvironmentColors),
                                                                                              "ContextMenuGlyphItemIconForegroundHover"));

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

        #region MenuHostControl

        public static ComponentResourceKey MenuHostControlBackground => _menuHostControlBackground ??
                                                                        (_menuHostControlBackground =
                                                                            new ComponentResourceKey(typeof(EnvironmentColors), "MenuHostControlBackground"));

        #endregion

        #region ToolBarHostControl
        public static ComponentResourceKey ToolBarHostControlTopDockBackground => _toolBarHostControlTopDockBackground ??
                                                                                  (_toolBarHostControlTopDockBackground =
                                                                                      new ComponentResourceKey(typeof(EnvironmentColors), "ToolBarHostControlTopDockBackground"));

        public static ComponentResourceKey ToolBarHostControlDefaultDockBackground => _toolBarHostControlDefaultDockBackground ??
                                                                                      (_toolBarHostControlDefaultDockBackground =
                                                                                          new ComponentResourceKey(typeof(EnvironmentColors), "ToolBarHostControlDefaultDockBackground"));

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