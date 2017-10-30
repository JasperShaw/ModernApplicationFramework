using System;
using System.Collections.Generic;
using ModernApplicationFramework.Core.Localization;

namespace ModernApplicationFramework.Interfaces.Services
{
    /// <summary>
    /// An <see cref="ILanguageManager"/> is responsible for the localization of the Framework
    /// </summary>
    public interface ILanguageManager
    {
        /// <summary>
        /// Fires when the Language was changed
        /// </summary>
        event EventHandler OnLanguageChanged;

        LanguageInfo CurrentLanguage { get; }
        LanguageInfo SavedLanguage { get; }

        /// <summary>
        /// Gets all available Languages on the Operating System
        /// </summary>
        /// <returns>Returns an <see cref="IEnumerable{T}"/> with the founded languages</returns>
        IEnumerable<LanguageInfo> GetInstalledLanguages();

        /// <summary>
        /// Saves the given Language
        /// </summary>
        /// <param name="languageCode">The language information that should be saved</param>
        void SaveLanguage(LanguageInfo languageCode);
    }
}