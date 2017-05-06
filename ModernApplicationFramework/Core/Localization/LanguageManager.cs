using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Core.Localization
{
    public interface ILanguageManager
    {
        event EventHandler OnLanguageChanged;

        LanguageInfo CurrentLanguage { get; }
        LanguageInfo SavedLanguage { get; }

        IEnumerable<LanguageInfo> GetInstalledLanguages();

        void SaveLanguage(LanguageInfo languageCode);

    }

    public sealed class LanguageManager : ILanguageManager
    {
        private const string SystemLanguageCode = "system";

        public event EventHandler OnLanguageChanged;

        public LanguageInfo CurrentLanguage { get; private set; }

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

        public void SaveLanguage(LanguageInfo languageCode)
        {
            Settings.Default.LanguageCode = languageCode.Code;
            Settings.Default.Save();
            OnRaiseLanguageChanged(new EventArgs());
        }


	    public LanguageInfo SavedLanguage
	    {
		    get
		    {
				var savedCode = Settings.Default.LanguageCode;
			    var installedLanguages = GetInstalledLanguages();
			    if (string.IsNullOrEmpty(savedCode))
				    return installedLanguages.FirstOrDefault(
					    x => x.Code.Equals(SystemLanguageCode, StringComparison.InvariantCultureIgnoreCase));
			    return installedLanguages.FirstOrDefault(
				    x => x.Code.Equals(savedCode, StringComparison.InvariantCultureIgnoreCase));
			}
	    }

        private void OnRaiseLanguageChanged(EventArgs e)
        {
            var handler = OnLanguageChanged;
            handler?.Invoke(this, e);
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
    }
}