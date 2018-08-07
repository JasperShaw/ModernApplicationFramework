using System;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    public interface IFontsAndColorsInformation
    {
        IClassificationType GetClassificationType(int colorableItemIndex);

        FontColorPreferences2 GetFontAndColorPreferences();

        void AddLanguageService(Guid languageServiceId);

        event EventHandler Updated;
    }
}