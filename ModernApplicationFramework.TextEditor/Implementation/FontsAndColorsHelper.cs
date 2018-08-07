using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal static class FontsAndColorsHelper
    {
        internal static string LegacyMarkerClassificationSuffix = " {LegacyMarker}";

        internal static string TryExtractMarkerNameFromLegacyMarkerClassificationName(string name)
        {
            if (name.EndsWith(LegacyMarkerClassificationSuffix))
                return name.Remove(name.Length - LegacyMarkerClassificationSuffix.Length);
            return null;
        }

        internal static void UpdateLegacyMarkerClassification(IClassificationFormatMap classificationFormatMap, IClassificationType classificationType, ColorableItemInfo[] info)
        {
            var classificationType1 = EditorParts.ClassificationTypeRegistryService.GetClassificationType("MarkerPlaceHolder");
            var properties = TextFormattingRunProperties.CreateTextFormattingRunProperties().SetForeground(info[0].Foreground);
            classificationFormatMap.AddExplicitTextProperties(classificationType, properties, classificationType1);
        }

        internal static IClassificationType GetOrCreateLegacyMarkerClassificationType(string markerName)
        {
            var type = markerName + LegacyMarkerClassificationSuffix;
            var typeRegistryService = EditorParts.ClassificationTypeRegistryService;
            var classificationType1 = typeRegistryService.GetClassificationType(type);
            if (classificationType1 == null)
            {
                var classificationType2 = typeRegistryService.GetClassificationType("MarkerPlaceHolder");
                classificationType1 = typeRegistryService.CreateClassificationType(type, new IClassificationType[1]
                {
                    classificationType2
                });
            }
            return classificationType1;
        }

        internal static void FillResourceDictionaryForegroundAndBold(ColorableItemInfo[] info, ResourceDictionary dictionary, bool fHasHierarchy)
        {
            FillDictionaryForeground(info, dictionary, fHasHierarchy);
            if (info[0].FontFlags.HasFlag(FontFlags.Bold))
                dictionary["IsBold"] = true;
            else
                dictionary["IsBold"] = false;
        }


        internal static Color GetWpfColor(uint win32Foreground)
        {
            var color = ColorTranslator.FromWin32((int)win32Foreground);
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        internal static void FillResourceDictionary(ColorableItemInfo[] info, ResourceDictionary dictionary, bool fHasHierarchy)
        {
            FillDictionaryBackground(info, dictionary, fHasHierarchy);
            FillResourceDictionaryForegroundAndBold(info, dictionary, fHasHierarchy);
        }

        internal static void UpdateLegacyMarkerBackground(IEditorFormatMap editorFormatMap, string markerName, ColorableItemInfo[] info)
        {
            var properties = editorFormatMap.GetProperties(markerName);
            FillDictionaryBackground(info, properties, false);
            editorFormatMap.SetProperties(markerName, properties);
        }

        internal static void FillDictionaryBackground(ColorableItemInfo[] info, ResourceDictionary dictionary,
            bool fHasHierarchy)
        {
            var color = info[0].Background;
            System.Windows.Media.Brush brush = new SolidColorBrush(color);
            brush.Freeze();
            dictionary["Background"] = brush;
            dictionary["BackgroundColor"] = color;
        }

        internal static void FillDictionaryForeground(ColorableItemInfo[] info, ResourceDictionary dictionary, bool fHasHierarchy)
        {
            var wpfColor = info[0].Foreground;
            System.Windows.Media.Brush brush = new SolidColorBrush(wpfColor);
            brush.Freeze();
            dictionary["ForegroundColor"] = wpfColor;
            dictionary["Foreground"] = brush;
        }

        internal static AllColorableItemInfo GetPlainTextItem(string itemName, string localizedName, Color foreground, Color background, Color autoForeground, Color autoBackground)
        {
            var nonMarkerItem = GetNonMarkerItem(itemName, localizedName, foreground, background, autoForeground, autoBackground);
            nonMarkerItem.Flags |= FcItemflags.Plaintext;
            return nonMarkerItem;
        }

        internal static AllColorableItemInfo GetNonMarkerItem(string itemName, string localizedName, Color foreground,
            Color background, Color autoForeground, Color autoBackground, bool allowBold = true)
        {
            var result = new AllColorableItemInfo
            {
                AutoBackgroundValid = true,
                AutoBackground = autoBackground,
                AutoForegroundValid = true,
                AutoForeground = autoForeground,
                NameValid = true,
                Name = itemName,
                FlagsValid = true,
                Flags = (FcItemflags) 22,
                LocalizedNameValid = true,
                LocalizedName = localizedName,
                MarkerVisualStyleValid = true,
                MarkerVisualStyle = 0,
                LineStyleValid = true,
                LineStyle = Linestyle.None,
                DescriptionValid = true,
                Description = null
            };
            if (allowBold)
                result.Flags |= FcItemflags.AllowBoldChange;
            SetBackgroundColor(background, ref result);
            SetForegroundColor(foreground, ref result);
            result.Info.FontFlags =  0;
            return result;
        }

        internal static AllColorableItemInfo GetNonMarkerItem(string itemName, string localizedName, Color foreground, Color autoForeground, bool allowBold = true)
        {
            var result = new AllColorableItemInfo
            {
                AutoBackgroundValid = false,
                AutoBackground = Colors.Transparent,
                AutoForegroundValid = true,
                AutoForeground = autoForeground,
                NameValid = true,
                Name = itemName,
                FlagsValid = true,
                Flags = (FcItemflags) 22,
                LocalizedNameValid = true,
                LocalizedName = localizedName,
                MarkerVisualStyleValid = true,
                MarkerVisualStyle = 0,
                LineStyleValid = true,
                LineStyle = Linestyle.None,
                DescriptionValid = true,
                Description = string.Empty
            };
            if (allowBold)
                result.Flags |= FcItemflags.AllowBoldChange;
            SetBackgroundColor(Colors.Transparent, ref result);
            SetForegroundColor(foreground, ref result);
            result.Info.FontFlags = 0;
            return result;
        }

        internal static string GetLocalizedFaceName(System.Windows.Media.FontFamily family)
        {
            var language1 = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name);
            var language2 = XmlLanguage.GetLanguage("en-us");
            if (family.FamilyNames.ContainsKey(language1))
                return family.FamilyNames[language1];
            if (family.FamilyNames.ContainsKey(language2))
                return family.FamilyNames[language2];
            if (family.FamilyNames.Count > 0)
                return family.FamilyNames.First().Value;
            return null;
        }

        internal static Typeface GetTypefaceFromFont(string typefaceName)
        {
            var fontFamily = new System.Windows.Media.FontFamily(typefaceName);
            var normal1 = FontStyles.Normal;
            var normal2 = FontStretches.Normal;
            var normal3 = FontWeights.Normal;
            var style = normal1;
            var weight = normal3;
            var stretch = normal2;
            var fallbackFontFamily = GetFallbackFontFamily();
            return new Typeface(fontFamily, style, weight, stretch, fallbackFontFamily);
        }

        internal static System.Windows.Media.FontFamily GetFallbackFontFamily()
        {
            return new System.Windows.Media.FontFamily("Global Monospace, Global User Interface");
        }

        internal static System.Windows.Media.FontFamily GetWPFDefaultFontFamily()
        {
            var windowsLanguageName = CultureInfo.CurrentUICulture.ThreeLetterWindowsLanguageName;
            var str = windowsLanguageName == "ENU"
                ? "Consolas"
                : (windowsLanguageName == "JPN"
                    ? "MS Gothic"
                    : (windowsLanguageName == "KOR"
                        ? "DotumChe"
                        : (windowsLanguageName == "CHS"
                            ? "NSimSun"
                            : (windowsLanguageName == "CHT" ? "MingLiU" : "Consolas"))));
            return !IsSystemFontAvailable(str)
                ? new System.Windows.Media.FontFamily("Courier New")
                : new System.Windows.Media.FontFamily(str);
        }

        internal static void DecodePlainTextBackground(ColorableItemInfo[] pInfo)
        {
            //Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.GetColorType(pInfo[0].crBackground, out int pctType));
            //switch (pctType)
            //{
            //    case 2:
            //        COLORINDEX[] pIdx = new COLORINDEX[1];
            //        Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.GetEncodedIndex(pInfo[0].crBackground, pIdx));
            //        Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.GetRGBOfIndex(pIdx[0], out pInfo[0].crBackground));
            //        break;
            //    case 3:
            //        pInfo[0].crBackground = FontsAndColorsHelper.DecodeSystemColor(pInfo[0].crBackground);
            //        break;
            //    case 5:
            //        Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.GetRGBOfIndex(COLORINDEX.CI_SYSPLAINTEXT_BK, out pInfo[0].crBackground));
            //        break;
            //}
        }

        internal static void DecodePlainTextForeground(ColorableItemInfo[] pInfo)
        {
            //int pctType;
            //Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.GetColorType(pInfo[0].crForeground, out pctType));
            //switch (pctType)
            //{
            //    case 2:
            //        COLORINDEX[] pIdx = new COLORINDEX[1];
            //        Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.GetEncodedIndex(pInfo[0].crForeground, pIdx));
            //        Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.GetRGBOfIndex(pIdx[0], out pInfo[0].crForeground));
            //        break;
            //    case 3:
            //        pInfo[0].crForeground = FontsAndColorsHelper.DecodeSystemColor(pInfo[0].crForeground);
            //        break;
            //    case 5:
            //        Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.GetRGBOfIndex(COLORINDEX.CI_SYSPLAINTEXT_FG, out pInfo[0].crForeground));
            //        break;
            //}
        }

        internal static void SetBackgroundColor(Color background, ref AllColorableItemInfo result)
        {
            //TODO: ???
            result.Info.Background = background;
        }

        internal static void SetForegroundColor(Color foreground, ref AllColorableItemInfo result)
        {
            //TODO: ???
            result.Info.Foreground = foreground;
        }

        private static bool IsSystemFontAvailable(string str)
        {
            var fontsCollection = new InstalledFontCollection();
            return fontsCollection.Families.Any(fontFamiliy => fontFamiliy.Name == str);
        }
    }
}