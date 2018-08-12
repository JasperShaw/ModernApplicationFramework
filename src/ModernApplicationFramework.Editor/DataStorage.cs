using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Editor.Implementation;
using ModernApplicationFramework.Text.Storage;

namespace ModernApplicationFramework.Editor
{
    internal class DataStorage : IDataStorage
    {
        private readonly string _category;

        private IFontAndColorStorage _storage;
        private bool _isPrefetching;
        private bool _invalidatedDuringPrefetch;
        private bool _subscribedToClassificationFormatMapChanged;

        private Dictionary<string, ColorableItemInfo>[] _prefetchedCategories;

        private Dictionary<string, ColorableItemInfo>[] PrefetchedCategories
        {
            get => _prefetchedCategories ?? (_prefetchedCategories = new Dictionary<string, ColorableItemInfo>[MostOftenUsedItems.TextEditorCategories.Length]);
            set => _prefetchedCategories = value;
        }

        protected IFontAndColorStorage Storage => _storage ?? (_storage = IoC.Get<IFontAndColorStorage>());

        public DataStorage(string storageKey)
        {
            _category = storageKey;
        }

        public static double FontSizeFromPointSize(int pointSize)
        {
            return FontSizeFromPointSize((double)pointSize);
        }

        public static double FontSizeFromPointSize(double pointSize)
        {
            var a = pointSize * 96.0 / 72.0;
            return Math.Round(a);
        }

        public bool TryGetItemValue(string itemKey, out ResourceDictionary itemValue)
        {
            itemValue = new ResourceDictionary();
            var category = _category;
            return category == "text"
                ? TryGetItemValueFromSubCategoryOfTextEditor(itemKey, itemValue)
                : (category == "printer"
                    ? TryGetItemValueFromSubCategoryOfPrinter(itemKey, itemValue)
                    : TryGetItemValueFromSubCategoryOfToolWindow(itemKey, itemValue));
        }

        private bool TryGetItemValueFromSubCategoryOfTextEditor(string itemKey, ResourceDictionary itemValue)
        {
            var colorableItemInfoArray = new ColorableItemInfo[1];
            if (itemKey == "TextView Background")
                return GetTextViewBackgroundInfo(CategoryGuids.GuidEditorTextManager, itemValue, colorableItemInfoArray);

            if (itemKey == "Plain Text")
                return GetPlainTextItemInfo(CategoryGuids.GuidEditorTextManager, itemValue, colorableItemInfoArray);


            var guid = Guid.Empty;
            var fLegacyMarkerClassification = false;

            for (var index = 0; index < 2; ++index)
            {
                if (GetItemInfoFromSubCategory(itemKey, MostOftenUsedItems.TextEditorCategories[index],
                    colorableItemInfoArray, out fLegacyMarkerClassification))
                {
                    guid = MostOftenUsedItems.TextEditorCategories[index];
                    break;
                }
            }

            if (guid.Equals(Guid.Empty))
                return false;
            var fHasHierarchy = guid.Equals(CategoryGuids.GuidEditorMef);
            FillResourceDictionary(colorableItemInfoArray, itemValue, fLegacyMarkerClassification, fHasHierarchy);
            return true;
        }

        private static void FillResourceDictionary(ColorableItemInfo[] info, ResourceDictionary dictionary, bool isLegacyMarkerClassification, bool fHasHierarchy)
        {
            if (isLegacyMarkerClassification)
                FontsAndColorsHelper.FillResourceDictionaryForegroundAndBold(info, dictionary, fHasHierarchy);
            else
                FontsAndColorsHelper.FillResourceDictionary(info, dictionary, fHasHierarchy);
        }

        private bool GetItemInfoFromSubCategory(string itemKey, Guid categoryGuid, ColorableItemInfo[] itemInfo,
            out bool fLegacyMarkerClassification)
        {
            var str = itemKey;
            var classificationName = FontsAndColorsHelper.TryExtractMarkerNameFromLegacyMarkerClassificationName(itemKey);
            if (classificationName != null)
                str = classificationName;
            bool flag;
            if (IsPefetchedCategory(categoryGuid))
            {
                flag = TryGetPrefetchedItem(categoryGuid, str, itemInfo);
            }
            else
            {
                var storage = Storage;
                var guid = categoryGuid;
                storage.OpenCategory(ref guid, StorageFlags.Loaddefaults);
                try
                {
                    flag = storage.GetItem(str, itemInfo);
                }
                finally
                {
                    storage.CloseCategory();
                }
            }
            fLegacyMarkerClassification = classificationName != null & flag;
            return flag;
        }

        private bool TryGetPrefetchedItem(Guid categoryGuid, string nameForItemLookup, ColorableItemInfo[] itemInfo)
        {
            if (_isPrefetching)
                return false;
            _isPrefetching = true;
            try
            {
                if (!_subscribedToClassificationFormatMapChanged)
                {
                    _subscribedToClassificationFormatMapChanged = true;
                    var classificationFormatMap = EditorParts.ClassificationFormatMapService.GetClassificationFormatMap(_category);
                    if (classificationFormatMap != null)
                        classificationFormatMap.ClassificationFormatMappingChanged += OnClassificationFormatMappingChanged;
                }
                var textEditorCategoryIndex = Array.IndexOf(MostOftenUsedItems.TextEditorCategories, categoryGuid);
                if (textEditorCategoryIndex >= 0)
                {
                    if (textEditorCategoryIndex < PrefetchedCategories.Length)
                    {
                        AssureCategoryIsPrefetched(textEditorCategoryIndex);
                        if (_invalidatedDuringPrefetch)
                            AssureCategoryIsPrefetched(textEditorCategoryIndex);
                        if (PrefetchedCategories[textEditorCategoryIndex].TryGetValue(nameForItemLookup, out var colorableItemInfo))
                        {
                            itemInfo[0] = colorableItemInfo;
                            return true;
                        }
                    }
                }
            }
            finally
            {
                _isPrefetching = false;
                _invalidatedDuringPrefetch = false;
            }
            return false;
        }

        private void OnClassificationFormatMappingChanged(object sender, EventArgs e)
        {
            PrefetchedCategories = null;
            if (!_isPrefetching)
                return;
            _invalidatedDuringPrefetch = true;
        }

        private void AssureCategoryIsPrefetched(int textEditorCategoryIndex)
        {
            if (PrefetchedCategories[textEditorCategoryIndex] != null && !_invalidatedDuringPrefetch)
                return;
            var storage = Storage;
            var textEditorCategory = MostOftenUsedItems.TextEditorCategories[textEditorCategoryIndex];
            storage.OpenCategory(ref textEditorCategory, StorageFlags.Loaddefaults);
            PrefetchedCategories[textEditorCategoryIndex] = new Dictionary<string, ColorableItemInfo>();
            try
            {
                if (storage == null || !storage.GetItemCount(out var count))
                    return;
                for (var index = 0; index < count; ++index)
                {
                    var pInfo = new ColorableItemInfo[1];
                    if (storage.GetItemNameAtIndex(index, out var itemName) && storage.GetItem(itemName, pInfo))
                        PrefetchedCategories[textEditorCategoryIndex][itemName] = pInfo[0];
                }
            }
            finally
            {
                Storage.CloseCategory();
            }
        }

        private bool IsPefetchedCategory(Guid categoryGuid)
        {
            if (_category != "text")
                return false;
            return Array.IndexOf(MostOftenUsedItems.TextEditorCategories, categoryGuid) != -1;
        }

        private bool GetTextViewBackgroundInfo(Guid categoryGuid, ResourceDictionary itemValue,
            ColorableItemInfo[] itemInfo)
        {
            try
            {
                var storage = Storage;
                var guid = categoryGuid;

                storage.OpenCategory(ref guid, StorageFlags.Loaddefaults);

                if (!storage.GetItem("Plain Text", itemInfo))
                    return false;
                FontsAndColorsHelper.DecodePlainTextBackground(itemInfo);
                var color = itemInfo[0].Background;
                Brush brush = new SolidColorBrush(color);
                brush.Freeze();        

                itemValue["Background"] = brush;
                itemValue["BackgroundColor"] = color;
                return true;
            }
            finally
            {
                Storage.CloseCategory();
            }
        }

        private bool TryGetItemValueFromSubCategoryOfPrinter(string itemKey, ResourceDictionary itemValue)
        {
            ColorableItemInfo[] colorableItemInfoArray = new ColorableItemInfo[1];
            if (itemKey == "TextView Background")
                return GetTextViewBackgroundInfo(CategoryGuids.GuidPrinterTextManager, itemValue, colorableItemInfoArray);
            if (itemKey == "Plain Text")
                return GetPlainTextItemInfo(CategoryGuids.GuidPrinterTextManager, itemValue, colorableItemInfoArray);
            bool fHasHierarchy;
            if (GetItemInfoFromSubCategory(itemKey, CategoryGuids.GuidPrinterTextManager, colorableItemInfoArray, out var fLegacyMarkerClassification))
                fHasHierarchy = false;
            //else if (GetItemInfoFromSubCategory(itemKey, CategoryGuids.GuidPrinterLanguageService, colorableItemInfoArray, out fLegacyMarkerClassification))
            //    fHasHierarchy = true;
            //else if (GetItemInfoFromSubCategory(itemKey, CategoryGuids.GuidPrinterTextMarker, colorableItemInfoArray, out fLegacyMarkerClassification))
            //{
            //    fHasHierarchy = false;
            //}
            else
            {
                if (!GetItemInfoFromSubCategory(itemKey, CategoryGuids.GuidPrinterMef, colorableItemInfoArray, out fLegacyMarkerClassification))
                    return false;
                fHasHierarchy = true;
            }
            FillResourceDictionary(colorableItemInfoArray, itemValue, fLegacyMarkerClassification, fHasHierarchy);
            return true;
        }

        private bool TryGetItemValueFromSubCategoryOfToolWindow(string itemKey, ResourceDictionary itemValue)
        {
            var colorableItemInfoArray = new ColorableItemInfo[1];
            var windowSubCategoryGuid = GetToolWindowSubCategoryGuid();
            if (windowSubCategoryGuid.Equals(Guid.Empty))
                return false;
            if (itemKey == "TextView Background")
                return GetTextViewBackgroundInfo(windowSubCategoryGuid, itemValue, colorableItemInfoArray);
            if (itemKey == "Plain Text")
                return GetPlainTextItemInfo(windowSubCategoryGuid, itemValue, colorableItemInfoArray);
            if (!GetItemInfoFromSubCategory(itemKey, windowSubCategoryGuid, colorableItemInfoArray, out var fLegacyMarkerClassification))
                return false;
            FillResourceDictionary(colorableItemInfoArray, itemValue, fLegacyMarkerClassification, false);
            return true;
        }

        private Guid GetToolWindowSubCategoryGuid()
        {
            var category = _category;
            Guid result;
            if (category != "command")
            {
                if (category != "immediate")
                {
                    if (category != "output")
                    {
                        if (category != "find results")
                        {
                            if (category != "completion")
                            {
                                if (category == "tooltip")
                                    result = CategoryGuids.GuidToolTip;
                                else if (!Guid.TryParse(_category.Split(':')[0], out result))
                                    result = Guid.Empty;
                            }
                            else
                                result = CategoryGuids.GuidStatementCompletion;
                        }
                        else
                            result = CategoryGuids.GuidFindResultsWindow;
                    }
                    else
                        result = CategoryGuids.GuidOutputWindow;
                }
                else
                    result = CategoryGuids.GuidImmediateWindow;
            }
            else
                result = CategoryGuids.GuidCommandWindow;
            return result;
        }

        private bool GetPlainTextItemInfo(Guid guid, IDictionary itemValue, ColorableItemInfo[] itemInfo)
        {
            try
            {
                var storage = Storage;
                var categoryGuid = guid;
                storage.OpenCategory(ref categoryGuid, StorageFlags.Loaddefaults);

                var pInfo = new[]
                {
                    new FontInfo()
                };

                if (!Storage.GetFont(null, pInfo))
                    return false;

                if (!pInfo[0].IsValid)
                    return false;

                var typeFaceFromFont = FontsAndColorsHelper.GetTypefaceFromFont(pInfo[0].Typeface);
                itemValue["Typeface"] = typeFaceFromFont;

                itemValue["FontRenderingSize"] = FontSizeFromPointSize(pInfo[0].PointSize);
                if (!storage.GetItem("Plain Text", itemInfo))
                    return false;

                if (itemInfo[0].FontFlags.HasFlag(FontFlags.Bold))
                    itemValue["IsBold"] = true;
                else
                    itemValue["IsBold"] = false;

                var color = itemInfo[0].Foreground;
                var brush = new SolidColorBrush(color);
                brush.Freeze();
                itemValue["ForegroundColor"] = color;
                itemValue["Foreground"] = brush;
                itemValue.Remove("BackgroundColor");
                itemValue.Remove("Background");
                return true;
            }
            finally
            {
                Storage.CloseCategory();
            }
        }
    }
}