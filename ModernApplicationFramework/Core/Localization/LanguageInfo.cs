using System.Globalization;

namespace ModernApplicationFramework.Core.Localization
{
    public class LanguageInfo
    {
        public CultureInfo CultureInfo { get; }

        public string Name { get; }

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