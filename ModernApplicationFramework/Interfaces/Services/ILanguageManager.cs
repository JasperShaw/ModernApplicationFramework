using System;
using System.Collections.Generic;
using ModernApplicationFramework.Core.Localization;

namespace ModernApplicationFramework.Interfaces.Services
{
    public interface ILanguageManager
    {
        event EventHandler OnLanguageChanged;

        LanguageInfo CurrentLanguage { get; }
        LanguageInfo SavedLanguage { get; }

        IEnumerable<LanguageInfo> GetInstalledLanguages();

        void SaveLanguage(LanguageInfo languageCode);

    }
}