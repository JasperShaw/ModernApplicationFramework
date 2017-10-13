using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Properties;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.Core.Localization
{
    /// <inheritdoc />
    /// <summary>
    ///     The <see cref="T:ModernApplicationFramework.Core.Localization.LanguageManager" /> controls the application's
    ///     environment language
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Services.ILanguageManager" />
    public sealed class LanguageManager : ILanguageManager
    {
        private const string SystemLanguageCode = "system";
        private const string RegistryKey = "InstalledApplicationLanguage";

        /// <summary>
        ///     Fires when the Language was changed
        /// </summary>
        public event EventHandler OnLanguageChanged;

        /// <summary>
        ///     The current language.
        /// </summary>
        public LanguageInfo CurrentLanguage { get; private set; }

        /// <inheritdoc />
        /// <summary>
        ///     Gets all available Languages on the Operating System
        /// </summary>
        /// <returns>
        ///     Returns an <see cref="T:System.Collections.Generic.IEnumerable`1" /> with the founded languages
        /// </returns>
        public IEnumerable<LanguageInfo> GetInstalledLanguages()
        {
            var culture = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var exeLocation = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);


            var installedCultures = culture.Where(
                cultureInfo => Directory.Exists(Path.Combine(exeLocation, cultureInfo.Name)) &&
                               !Equals(cultureInfo, CultureInfo.InvariantCulture));

            var list = installedCultures.Select(cultureInfo => new LanguageInfo(cultureInfo)).ToList();
            list.Add(new LanguageInfo(CultureInfo.InstalledUICulture, CommonUI_Resources.SystemLanguage_Name,
                SystemLanguageCode));
            return list;
        }

        /// <summary>
        ///     Gets the saved language.
        /// </summary>
        /// <value>
        ///     The saved language.
        /// </value>
        public LanguageInfo SavedLanguage
        {
            get
            {
                var savedCode = IoC.Get<IEnvironmentVariables>()
                    .GetOrCreateRegistryVariable(RegistryKey, null, SystemLanguageCode);
                //var savedCode = Settings.Default.LanguageCode;
                var installedLanguages = GetInstalledLanguages();
                if (string.IsNullOrEmpty(savedCode))
                    return installedLanguages.FirstOrDefault(
                        x => x.Code.Equals(SystemLanguageCode, StringComparison.InvariantCultureIgnoreCase));
                return installedLanguages.FirstOrDefault(
                    x => x.Code.Equals(savedCode, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        /// <summary>
        ///     Saves the given Language
        /// </summary>
        /// <param name="languageCode">The language information that should be saved</param>
        public void SaveLanguage(LanguageInfo languageCode)
        {
            IoC.Get<IEnvironmentVariables>().SetRegistryVariable(RegistryKey, languageCode.Code, null);
            OnRaiseLanguageChanged(new EventArgs());
        }

        internal void SetLanguage(LanguageInfo info)
        {
            Thread.CurrentThread.CurrentCulture = info.CultureInfo;
            Thread.CurrentThread.CurrentUICulture = info.CultureInfo;
            CurrentLanguage = info;
            SaveLanguage(info);
        }

        internal void SetLanguage()
        {
            SetLanguage(SavedLanguage);
        }

        private void OnRaiseLanguageChanged(EventArgs e)
        {
            var handler = OnLanguageChanged;
            handler?.Invoke(this, e);
        }
    }
}