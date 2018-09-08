using System.Windows;

namespace ModernApplicationFramework.Core.Themes
{
    public static class CommonControlsColors
    {
        private static ComponentResourceKey _buttonBackground;
        private static ComponentResourceKey _buttonText;
        private static ComponentResourceKey _buttonBorder;
        private static ComponentResourceKey _buttonBorderDefault;
        private static ComponentResourceKey _buttonBorderDisabled;
        private static ComponentResourceKey _buttonBorderFocused;
        private static ComponentResourceKey _buttonBorderHover;
        private static ComponentResourceKey _buttonBorderPressed;
        private static ComponentResourceKey _buttonDefault;
        private static ComponentResourceKey _buttonDefaultText;
        private static ComponentResourceKey _buttonDisabled;
        private static ComponentResourceKey _buttonDisabledText;
        private static ComponentResourceKey _buttonFocused;
        private static ComponentResourceKey _buttonFocusedText;
        private static ComponentResourceKey _buttonHover;
        private static ComponentResourceKey _buttonHoverText;
        private static ComponentResourceKey _buttonPressed;
        private static ComponentResourceKey _buttonPressedText;

        private static ComponentResourceKey _comboBoxBackground;
        private static ComponentResourceKey _comboBoxBackgroundDisabled;
        private static ComponentResourceKey _comboBoxBackgroundFocused;
        private static ComponentResourceKey _comboBoxBackgroundHover;
        private static ComponentResourceKey _comboBoxBackgroundPressed;
        private static ComponentResourceKey _comboBoxBorder;
        private static ComponentResourceKey _comboBoxBorderDisabled;
        private static ComponentResourceKey _comboBoxBorderFocused;
        private static ComponentResourceKey _comboBoxBorderHover;
        private static ComponentResourceKey _comboBoxBorderPressed;
        private static ComponentResourceKey _comboBoxGlyph;
        private static ComponentResourceKey _comboBoxGlyphBackground;
        private static ComponentResourceKey _comboBoxGlyphBackgroundDisabled;
        private static ComponentResourceKey _comboBoxGlyphBackgroundFocused;
        private static ComponentResourceKey _comboBoxGlyphBackgroundHover;
        private static ComponentResourceKey _comboBoxGlyphBackgroundPressed;
        private static ComponentResourceKey _comboBoxGlyphDisabled;
        private static ComponentResourceKey _comboBoxGlyphFocused;
        private static ComponentResourceKey _comboBoxGlyphHover;
        private static ComponentResourceKey _comboBoxGlyphPressed;
        private static ComponentResourceKey _comboBoxListBackground;
        private static ComponentResourceKey _comboBoxListBackgroundShadow;
        private static ComponentResourceKey _comboBoxListBorder;
        private static ComponentResourceKey _comboBoxListItemBackgroundHover;
        private static ComponentResourceKey _comboBoxListItemBorderHover;
        private static ComponentResourceKey _comboBoxListItemText;
        private static ComponentResourceKey _comboBoxListItemTextHover;
        private static ComponentResourceKey _comboBoxSelection;
        private static ComponentResourceKey _comboBoxSeparator;
        private static ComponentResourceKey _comboBoxSeparatorDisabled;
        private static ComponentResourceKey _comboBoxSeparatorFocused;
        private static ComponentResourceKey _comboBoxSeparatorHover;
        private static ComponentResourceKey _comboBoxSeparatorPressed;
        private static ComponentResourceKey _comboBoxText;
        private static ComponentResourceKey _comboBoxTextDisabled;
        private static ComponentResourceKey _comboBoxTextFocused;
        private static ComponentResourceKey _comboBoxTextHover;
        private static ComponentResourceKey _comboBoxTextInputSelection;
        private static ComponentResourceKey _comboBoxTextPressed;

        public static ComponentResourceKey ButtonBackground => _buttonBackground ??
                                                                 (_buttonBackground = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonBackground)));

        public static ComponentResourceKey ButtonText => _buttonText ??
                                                                 (_buttonText = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonText)));

        public static ComponentResourceKey ButtonBorder => _buttonBorder ??
                                                                 (_buttonBorder = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonBorder)));

        public static ComponentResourceKey ButtonBorderDefault => _buttonBorderDefault ??
                                                                 (_buttonBorderDefault = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonBorderDefault)));

        public static ComponentResourceKey ButtonBorderDisabled => _buttonBorderDisabled ??
                                                                 (_buttonBorderDisabled = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonBorderDisabled)));

        public static ComponentResourceKey ButtonBorderFocused => _buttonBorderFocused ??
                                                                 (_buttonBorderFocused = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonBorderFocused)));

        public static ComponentResourceKey ButtonBorderHover => _buttonBorderHover ??
                                                                 (_buttonBorderHover = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonBorderHover)));

        public static ComponentResourceKey ButtonBorderPressed => _buttonBorderPressed ??
                                                                 (_buttonBorderPressed = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonBorderPressed)));

        public static ComponentResourceKey ButtonDefault => _buttonDefault ??
                                                                 (_buttonDefault = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonDefault)));

        public static ComponentResourceKey ButtonDefaultText => _buttonDefaultText ??
                                                                 (_buttonDefaultText = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonDefaultText)));

        public static ComponentResourceKey ButtonDisabled => _buttonDisabled ??
                                                                 (_buttonDisabled = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonDisabled)));

        public static ComponentResourceKey ButtonDisabledText => _buttonDisabledText ??
                                                                 (_buttonDisabledText = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonDisabledText)));

        public static ComponentResourceKey ButtonFocused => _buttonFocused ??
                                                                 (_buttonFocused = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonFocused)));

        public static ComponentResourceKey ButtonFocusedText => _buttonFocusedText ??
                                                                 (_buttonFocusedText = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonFocusedText)));

        public static ComponentResourceKey ButtonHover => _buttonHover ??
                                                                 (_buttonHover = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonHover)));

        public static ComponentResourceKey ButtonHoverText => _buttonHoverText ??
                                                                 (_buttonHoverText = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonHoverText)));

        public static ComponentResourceKey ButtonPressed => _buttonPressed ??
                                                                 (_buttonPressed = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonPressed)));

        public static ComponentResourceKey ButtonPressedText => _buttonPressedText ??
                                                                 (_buttonPressedText = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ButtonPressedText)));

        public static ComponentResourceKey ComboBoxBackground => _comboBoxBackground ??
                                                                         (_comboBoxBackground = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxBackground)));

        public static ComponentResourceKey ComboBoxBackgroundDisabled => _comboBoxBackgroundDisabled ??
                                                                         (_comboBoxBackgroundDisabled = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxBackgroundDisabled)));

        public static ComponentResourceKey ComboBoxBackgroundFocused => _comboBoxBackgroundFocused ??
                                                                         (_comboBoxBackgroundFocused = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxBackgroundFocused)));

        public static ComponentResourceKey ComboBoxBackgroundHover => _comboBoxBackgroundHover ??
                                                                         (_comboBoxBackgroundHover = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxBackgroundHover)));

        public static ComponentResourceKey ComboBoxBackgroundPressed => _comboBoxBackgroundPressed ??
                                                                         (_comboBoxBackgroundPressed = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxBackgroundPressed)));

        public static ComponentResourceKey ComboBoxBorder => _comboBoxBorder ??
                                                                         (_comboBoxBorder = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxBorder)));

        public static ComponentResourceKey ComboBoxBorderDisabled => _comboBoxBorderDisabled ??
                                                                         (_comboBoxBorderDisabled = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxBorderDisabled)));

        public static ComponentResourceKey ComboBoxBorderFocused => _comboBoxBorderFocused ??
                                                                         (_comboBoxBorderFocused = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxBorderFocused)));

        public static ComponentResourceKey ComboBoxBorderHover => _comboBoxBorderHover ??
                                                                         (_comboBoxBorderHover = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxBorderHover)));

        public static ComponentResourceKey ComboBoxBorderPressed => _comboBoxBorderPressed ??
                                                                         (_comboBoxBorderPressed = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxBorderPressed)));

        public static ComponentResourceKey ComboBoxGlyph => _comboBoxGlyph ??
                                                                         (_comboBoxGlyph = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxGlyph)));

        public static ComponentResourceKey ComboBoxGlyphBackground => _comboBoxGlyphBackground ??
                                                                         (_comboBoxGlyphBackground = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxGlyphBackground)));

        public static ComponentResourceKey ComboBoxGlyphBackgroundDisabled => _comboBoxGlyphBackgroundDisabled ??
                                                                         (_comboBoxGlyphBackgroundDisabled = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxGlyphBackgroundDisabled)));

        public static ComponentResourceKey ComboBoxGlyphBackgroundFocused => _comboBoxGlyphBackgroundFocused ??
                                                                         (_comboBoxGlyphBackgroundFocused = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxGlyphBackgroundFocused)));

        public static ComponentResourceKey ComboBoxGlyphBackgroundHover => _comboBoxGlyphBackgroundHover ??
                                                                         (_comboBoxGlyphBackgroundHover = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxGlyphBackgroundHover)));

        public static ComponentResourceKey ComboBoxGlyphBackgroundPressed => _comboBoxGlyphBackgroundPressed ??
                                                                         (_comboBoxGlyphBackgroundPressed = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxGlyphBackgroundPressed)));

        public static ComponentResourceKey ComboBoxGlyphDisabled => _comboBoxGlyphDisabled ??
                                                                         (_comboBoxGlyphDisabled = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxGlyphDisabled)));

        public static ComponentResourceKey ComboBoxGlyphFocused => _comboBoxGlyphFocused ??
                                                                         (_comboBoxGlyphFocused = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxGlyphFocused)));

        public static ComponentResourceKey ComboBoxGlyphHover => _comboBoxGlyphHover ??
                                                                         (_comboBoxGlyphHover = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxGlyphHover)));

        public static ComponentResourceKey ComboBoxGlyphPressed => _comboBoxGlyphPressed ??
                                                                         (_comboBoxGlyphPressed = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxGlyphPressed)));

        public static ComponentResourceKey ComboBoxListBackground => _comboBoxListBackground ??
                                                                         (_comboBoxListBackground = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxListBackground)));

        public static ComponentResourceKey ComboBoxListBackgroundShadow => _comboBoxListBackgroundShadow ??
                                                                         (_comboBoxListBackgroundShadow = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxListBackgroundShadow)));

        public static ComponentResourceKey ComboBoxListBorder => _comboBoxListBorder ??
                                                                         (_comboBoxListBorder = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxListBorder)));

        public static ComponentResourceKey ComboBoxListItemBackgroundHover => _comboBoxListItemBackgroundHover ??
                                                                         (_comboBoxListItemBackgroundHover = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxListItemBackgroundHover)));

        public static ComponentResourceKey ComboBoxListItemBorderHover => _comboBoxListItemBorderHover ??
                                                                         (_comboBoxListItemBorderHover = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxListItemBorderHover)));

        public static ComponentResourceKey ComboBoxListItemText => _comboBoxListItemText ??
                                                                         (_comboBoxListItemText = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxListItemText)));

        public static ComponentResourceKey ComboBoxListItemTextHover => _comboBoxListItemTextHover ??
                                                                         (_comboBoxListItemTextHover = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxListItemTextHover)));

        public static ComponentResourceKey ComboBoxSelection => _comboBoxSelection ??
                                                                         (_comboBoxSelection = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxSelection)));

        public static ComponentResourceKey ComboBoxSeparator => _comboBoxSeparator ??
                                                                         (_comboBoxSeparator = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxSeparator)));

        public static ComponentResourceKey ComboBoxSeparatorDisabled => _comboBoxSeparatorDisabled ??
                                                                         (_comboBoxSeparatorDisabled = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxSeparatorDisabled)));

        public static ComponentResourceKey ComboBoxSeparatorFocused => _comboBoxSeparatorFocused ??
                                                                         (_comboBoxSeparatorFocused = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxSeparatorFocused)));

        public static ComponentResourceKey ComboBoxSeparatorHover => _comboBoxSeparatorHover ??
                                                                         (_comboBoxSeparatorHover = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxSeparatorHover)));

        public static ComponentResourceKey ComboBoxSeparatorPressed => _comboBoxSeparatorPressed ??
                                                                         (_comboBoxSeparatorPressed = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxSeparatorPressed)));

        public static ComponentResourceKey ComboBoxText => _comboBoxText ??
                                                                         (_comboBoxText = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxText)));

        public static ComponentResourceKey ComboBoxTextDisabled => _comboBoxTextDisabled ??
                                                                         (_comboBoxTextDisabled = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxTextDisabled)));

        public static ComponentResourceKey ComboBoxTextFocused => _comboBoxTextFocused ??
                                                                         (_comboBoxTextFocused = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxTextFocused)));

        public static ComponentResourceKey ComboBoxTextHover => _comboBoxTextHover ??
                                                                         (_comboBoxTextHover = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxTextHover)));

        public static ComponentResourceKey ComboBoxTextInputSelection => _comboBoxTextInputSelection ??
                                                                         (_comboBoxTextInputSelection = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxTextInputSelection)));

        public static ComponentResourceKey ComboBoxTextPressed => _comboBoxTextPressed ??
                                                                         (_comboBoxTextPressed = new ComponentResourceKey(typeof(CommonControlsColors), nameof(ComboBoxTextPressed)));

    }
}
