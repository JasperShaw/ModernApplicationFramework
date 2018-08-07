using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public class FontsAndColorsCategory
    {
        public Guid FontCategory { get; protected set; }

        public Guid ColorCategory { get; protected set; }

        public Guid LanguageService { get; protected set; }

        public FontsAndColorsCategory(Guid languageService, Guid fontCategory, Guid colorCategory)
        {
            FontCategory = fontCategory;
            ColorCategory = colorCategory;
            LanguageService = languageService;
        }

        public FontsAndColorsCategory SetColorCategory(Guid newColorCategory)
        {
            return new FontsAndColorsCategory(LanguageService, FontCategory, newColorCategory);
        }

        public FontsAndColorsCategory SetFontCategory(Guid newFontCategory)
        {
            return new FontsAndColorsCategory(LanguageService, newFontCategory, ColorCategory);
        }

        public FontsAndColorsCategory SetLanguageService(Guid newLanguageService)
        {
            return new FontsAndColorsCategory(newLanguageService, FontCategory, ColorCategory);
        }

        public string AppearanceCategory
        {
            get
            {
                string str;
                if (FontCategory == ColorCategory)
                {
                    if (FontCategory == CategoryGuids.GuidTextEditorGroup)
                        str = "text";
                    else if (FontCategory == CategoryGuids.GuidPrinterGroup)
                    {
                        str = "printer";
                    }
                    else
                    {
                        if (FontCategory == CategoryGuids.GuidToolTip)
                            return "tooltip";
                        str = !(FontCategory == CategoryGuids.GuidStatementCompletion)
                            ? (!(FontCategory == CategoryGuids.GuidImmediateWindow)
                                ? (!(FontCategory == CategoryGuids.GuidCommandWindow)
                                    ? (!(FontCategory == CategoryGuids.GuidCommandWindow)
                                        ? (!(FontCategory == CategoryGuids.GuidFindResultsWindow)
                                            ? FontCategory + ":" + ColorCategory
                                            : "find results")
                                        : "output")
                                    : "command")
                                : "immediate")
                            : "completion";
                    }
                }
                else
                    str = FontCategory + ":" + ColorCategory;
                return str;
            }
        }

        public override bool Equals(object obj)
        {
            var andColorsCategory = obj as FontsAndColorsCategory;
            if (andColorsCategory == null || !(FontCategory == andColorsCategory.FontCategory) || !(ColorCategory == andColorsCategory.ColorCategory))
                return false;
            return LanguageService == andColorsCategory.LanguageService;
        }

        public override int GetHashCode()
        {
            return AppearanceCategory.GetHashCode();
        }

        public static bool operator ==(FontsAndColorsCategory first, FontsAndColorsCategory second)
        {
            if ((object)first != null)
                return first.Equals(second);
            return (object)second == null;
        }

        public static bool operator !=(FontsAndColorsCategory first, FontsAndColorsCategory second)
        {
            return !(first == second);
        }
    }
}