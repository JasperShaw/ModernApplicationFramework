using System.Globalization;

namespace ModernApplicationFramework.Core.Localization
{
    /// <summary>
    /// A model used by the <see cref="LanguageManager"/> 
    /// </summary>
    public class LanguageInfo
    {
        /// <summary>
        /// The culture information.
        /// </summary>
        public CultureInfo CultureInfo { get; }

        /// <summary>
        /// The unlocalized, but native name of the language.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The two letter ISO code
        /// </summary>
        public string Code { get; }

        public LanguageInfo(CultureInfo cultureInfo) : this (cultureInfo, cultureInfo.NativeName, cultureInfo.TwoLetterISOLanguageName)
        {         
        }

        public LanguageInfo(CultureInfo cultureInfo, string name, string code)
        {
            CultureInfo = cultureInfo;
            Name = name;
            Code = code;
        }
    }
}