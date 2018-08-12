using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Export(typeof(IFontsAndColorsInformationService))]
    internal sealed class FontAndColorInformationService : IFontsAndColorsInformationService
    {
        private readonly IDictionary<Guid, WeakReference> _dataMap = new Dictionary<Guid, WeakReference>();

        public IFontsAndColorsInformation GetFontAndColorInformation(FontsAndColorsCategory fontsAndColorsCategory)
        {
            var languageService = fontsAndColorsCategory.LanguageService;
            FontsAndColorsInformation colorsInformation = null;
            if (_dataMap.TryGetValue(languageService, out var weakReference))
            {
                if (weakReference.IsAlive)
                    colorsInformation = weakReference.Target as FontsAndColorsInformation;
                else
                    _dataMap.Remove(languageService);
            }
            if (colorsInformation == null)
            {
                colorsInformation = new FontsAndColorsInformation(fontsAndColorsCategory);
                _dataMap.Add(languageService, new WeakReference(colorsInformation));
            }
            return colorsInformation;
        }
    }
}