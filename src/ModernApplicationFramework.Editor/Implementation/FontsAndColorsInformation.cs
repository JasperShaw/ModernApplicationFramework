using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Caliburn.Micro;
using ModernApplicationFramework.Text.Logic.Classification;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class FontsAndColorsInformation : IFontsAndColorsInformation
    {
        private readonly FontsAndColorsCategory _fontsAndColorsCategory;
        private readonly Dictionary<int, IClassificationType> _classificationTypeMap;
        private readonly IClassificationType _languagePlaceHolder;
        private readonly List<Guid> _languages;
        private List<IProvideColorableItems> _colorableItemSources;

        private IEnumerable<IProvideColorableItems> ColorableItemSources
        {
            get
            {
                if (_colorableItemSources == null)
                {
                    _colorableItemSources = new List<IProvideColorableItems>();
                    var services = IoC.GetAll<IProvideColorableItems>().ToList();
                    foreach (var language in _languages)
                    {
                        var service = services.FirstOrDefault(x => x.LanguageId == language);
                        if (service != null)
                            _colorableItemSources.Add(service);
                    }
                    //TODO: Textmanager
                    //IProvideColorableItems service1 = Common.GetService<IVsProvideColorableItems, SVsTextManager>(this._serviceProvider);
                    //if (service1 != null)
                    //    _colorableItemSources.Add(service1);
                }
                return _colorableItemSources;
            }
        }

        public FontsAndColorsInformation(FontsAndColorsCategory fontsAndColorsCategory)
        {
            _fontsAndColorsCategory = fontsAndColorsCategory;
            _classificationTypeMap = new Dictionary<int, IClassificationType>();
            _languagePlaceHolder = EditorParts.ClassificationTypeRegistryService.GetClassificationType("formal language");
            _languages = new List<Guid>
            {
                _fontsAndColorsCategory.LanguageService
            };
        }

        public IClassificationType GetClassificationType(int colorableItemIndex)
        {
            if (!_classificationTypeMap.TryGetValue(colorableItemIndex, out var classificationType))
            {
                IColorableItem ppItem = null;
                foreach (var colorableItemSource in ColorableItemSources)
                {
                    if (colorableItemSource.GetColorableItem(colorableItemIndex, out ppItem) == 0)
                        break;
                }
                if (ppItem != null)
                {
                    var canonicalName = GetCanonicalName(ppItem);
                    classificationType = EditorParts.ClassificationTypeRegistryService.GetClassificationType(canonicalName) ??
                                         EditorParts.ClassificationTypeRegistryService.CreateClassificationType(canonicalName, new[]
                    {
                        _languagePlaceHolder
                    });
                }
                _classificationTypeMap.Add(colorableItemIndex, classificationType);
            }
            return classificationType;
        }

        private static string GetCanonicalName(IColorableItem item)
        {
            IMergeableUiItem vsMergeableUiItem = item;
            if (vsMergeableUiItem == null || vsMergeableUiItem.GetCanonicalName(out var str) != 0)
                Marshal.ThrowExceptionForHR(item.GetDisplayName(out str));
            return str;
        }

        public FontColorPreferences2 GetFontAndColorPreferences()
        {
            var pPrefs = new FontColorPreferences2();
            var service = IoC.Get<ICategorizedFontColorPrefs>();
            if (service == null)
                throw new InvalidOperationException("Unable to get IVsCategorizedFontColorPrefs from buffer's text manager");
            var gcHandle = GCHandle.Alloc(_fontsAndColorsCategory.LanguageService, GCHandleType.Pinned);
            try
            {
                pPrefs.pguidColorService = gcHandle.AddrOfPinnedObject();
                var fontCategory = _fontsAndColorsCategory.FontCategory;
                var colorCategory = _fontsAndColorsCategory.ColorCategory;
                if (service.GetFontColorPreferences(ref fontCategory, ref colorCategory, ref pPrefs) == 0)
                {
                    if (pPrefs.pColorTable != null)
                        Marshal.GetIUnknownForObject(pPrefs.pColorTable);
                }
            }
            finally
            {
                gcHandle.Free();
                if (Marshal.IsComObject(service))
                    Marshal.FinalReleaseComObject(service);
            }
            return pPrefs;
        }

        public void AddLanguageService(Guid languageGuid)
        {
            if (languageGuid == _fontsAndColorsCategory.LanguageService)
                return;
            _languages.Add(languageGuid);
        }

        public event EventHandler Updated;
    }
}
