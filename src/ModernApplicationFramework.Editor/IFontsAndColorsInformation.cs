using System;
using ModernApplicationFramework.Editor.TextManager;
using ModernApplicationFramework.Text.Logic.Classification;

namespace ModernApplicationFramework.Editor
{
    public interface IFontsAndColorsInformation
    {
        IClassificationType GetClassificationType(int colorableItemIndex);

        FontColorPreferences2 GetFontAndColorPreferences();

        void AddLanguageService(Guid languageServiceId);

        event EventHandler Updated;
    }
}