using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
{

    //TODO: Implement missing stuff
    internal class TextMarkerFontAndColorCategory : TextEditorFontAndColorCategoryBase
    {
        private List<TextMarkerInstance> _markers;

        private List<TextMarkerInstance> Markers
        {
            get
            {
                //if (_markers == null)
                //{
                //    Marshal.ThrowExceptionForHR(TextManager.GetMarkerTypeCount(out int piMarkerTypeCount));
                //    _markers = new List<TextMarkerInstance>(piMarkerTypeCount);
                //    SettingsStore onlySettingsStore = new ShellSettingsManager((IServiceProvider)new ServiceProvider(this.ServiceProvider))?.GetReadOnlySettingsStore(SettingsScope.Configuration);
                //    for (var iMarkerType = 0; iMarkerType < piMarkerTypeCount; ++iMarkerType)
                //    {
                //        string markerTypeConfigurationPath;
                //        if (!LogEncodedColorData && onlySettingsStore != null && ErrorHandler.Succeeded(((IVsPrivTextManager)this.TextManager).GetMarkerTypeConfigurationPath(iMarkerType, out markerTypeConfigurationPath)))
                //        {
                //            if (!string.IsNullOrEmpty(markerTypeConfigurationPath))
                //            {
                //                try
                //                {
                //                    if (onlySettingsStore.PropertyExists(markerTypeConfigurationPath, "ColorData"))
                //                    {
                //                        var result = Guid.Empty;
                //                        if (Guid.TryParse(onlySettingsStore.GetString(markerTypeConfigurationPath, "Package", (string)null), out result))
                //                        {
                //                            using (MemoryStream memoryStream = onlySettingsStore.GetMemoryStream(markerTypeConfigurationPath, "ColorData"))
                //                            {
                //                                Tuple<int, AllColorableItemInfo>[] array = FontsAndColorsHelper.DecodeEditorColorData(memoryStream, CategoryGuid).ToArray<Tuple<int, AllColorableItemInfo>>();
                //                                if (array.Length == 1)
                //                                {
                //                                    var metadata = array[0].Item2;
                //                                    if (metadata.bLocalizedNameValid == 1 && metadata.bstrLocalizedName != null && result != Guid.Empty)
                //                                        metadata.bstrLocalizedName = FontsAndColorsHelper.GetLocalizedStringFromPackage(metadata.bstrLocalizedName, result, this.ServiceProvider);
                //                                    _markers.Add(new TextMarkerInstance(metadata));
                //                                    continue;
                //                                }
                //                            }
                //                        }
                //                    }
                //                }
                //                catch
                //                {
                //                }
                //            }
                //        }
                //        IPackageDefinedTextMarkerType markerInfo;
                //        if (!ErrorHandler.Failed(((IVsPrivTextManager)this.TextManager).GetMarkerTypeInterface2(iMarkerType, out markerInfo)))
                //        {
                //            uint pdwFlags = 0;
                //            if (!ErrorHandler.Failed(markerInfo.GetBehaviorFlags(out pdwFlags)) && ((int)pdwFlags & 512) == 0)
                //            {
                //                IMergeableUIItem vsMergeableUiItem = markerInfo as IMergeableUiItem;
                //                string pbstrDisplayName;
                //                if (vsMergeableUiItem != null && !ErrorHandler.Failed(vsMergeableUiItem.GetDisplayName(out pbstrDisplayName)))
                //                {
                //                    _markers.Add(new TextMarkerInstance(markerInfo));
                //                    if (LogEncodedColorData && onlySettingsStore != null && ErrorHandler.Succeeded(((IVsPrivTextManager)this.TextManager).GetMarkerTypeConfigurationPath(iMarkerType, out markerTypeConfigurationPath)))
                //                    {
                //                        Trace.WriteLine("Visual Studio Text Marker Color Data: " + markerTypeConfigurationPath);
                //                        Trace.WriteLine("hex:" + FontsAndColorsHelper.ConvertColorDataToString((IEnumerable<Tuple<int, AllColorableItemInfo>>)new Tuple<int, AllColorableItemInfo>[1]
                //                        {
                //                            Tuple.Create(-1, ConvertMarker(markerInfo))
                //                        }, CategoryGuid));
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                if (_markers == null)
                    _markers = new List<TextMarkerInstance>();
                return _markers;
            }
        }

        public override string GetCategoryName()
        {
            return "Text Marker Items";
        }

        public override int GetItemCount()
        {
            return Markers.Count;
        }

        public override AllColorableItemInfo GetItem(int iItem)
        {
            var item = default(AllColorableItemInfo);
            if (Markers[iItem].IsMetadataBased)
            {
                item = Markers[iItem].ColorData;
            }
            else
            {
                var instance = Markers[iItem].Instance;
                item = ConvertMarker(instance);
            }

            return item;
        }

        public override int GetItemByName(string szItem, out AllColorableItemInfo info)
        {
            info = default;
            foreach (var marker in Markers)
            {
                string displayName = null;
                string canonicalName = null;
                if (marker.IsMetadataBased)
                    canonicalName = marker.ColorData.Name;
                else
                    GetMarkerName(marker.Instance, ref displayName, ref canonicalName);
                if (szItem.Equals(canonicalName, StringComparison.Ordinal))
                {
                    info = !marker.IsMetadataBased ? ConvertMarker(marker.Instance) : marker.ColorData;
                    return 0;
                }
            }
            return -2147467259;
        }

        protected override ushort GetPriorityOrder()
        {
            return 3;
        }

        private static AllColorableItemInfo ConvertMarker(IPackageDefinedTextMarkerType marker)
        {
            return default;
            //var result = new AllColorableItemInfo();
            //string displayName = null;
            //string canonicalName = null;
            //TextMarkerFontAndColorCategory.GetMarkerName(marker, ref displayName, ref canonicalName);
            //uint pdwFlags = 0;
            //uint pdwVisualFlags = 0;
            //Marshal.ThrowExceptionForHR(marker.GetBehaviorFlags(out pdwFlags));
            //Marshal.ThrowExceptionForHR(marker.GetVisualStyle(out pdwVisualFlags));
            //if (((int)pdwFlags & 64) == 64)
            //    pdwVisualFlags |= 262144U;
            //var flag1 = false;
            //if (((int)pdwFlags & 32) == 32)
            //{
            //    flag1 = true;
            //    pdwVisualFlags |= 524288U;
            //}

            //if (marker.GetDefaultFontFlags(out uint pdwFontFlags) < 0)
            //    pdwFontFlags = 0U;
            //uint num = 0;
            //uint pcrResult1 = 0;
            //Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.EncodeIndexedColor(COLORINDEX.CI_USERTEXT_FG, out uint foreground));
            //Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.EncodeIndexedColor(COLORINDEX.CI_USERTEXT_BK, out uint background));
            //if (((int)pdwVisualFlags & 71) != 0)
            //{
            //    IVsHiColorItem vsHiColorItem = marker as IVsHiColorItem;
            //    COLORINDEX[] colorindexArray1 = new COLORINDEX[1]
            //    {
            //        COLORINDEX.CI_USERTEXT_BK
            //    };
            //    COLORINDEX[] colorindexArray2 = new COLORINDEX[1];
            //    if (vsHiColorItem == null || ErrorHandler.Failed(vsHiColorItem.GetColorData(1, out background)))
            //    {
            //        Marshal.ThrowExceptionForHR(marker.GetDefaultColors(colorindexArray1, colorindexArray2));
            //        Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.EncodeIndexedColor(colorindexArray1[0], out background));
            //    }
            //    if (vsHiColorItem == null || ErrorHandler.Failed(vsHiColorItem.GetColorData(0, out foreground)))
            //    {
            //        Marshal.ThrowExceptionForHR(marker.GetDefaultColors(colorindexArray2, colorindexArray1));
            //        Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.EncodeIndexedColor(colorindexArray2[0], out foreground));
            //    }
            //}
            //Linestyle[] piLineIndex = new Linestyle[1];
            //uint crSource;
            //if (((int)pdwVisualFlags & 8456) != 0)
            //{
            //    COLORINDEX[] piLineColor = new COLORINDEX[1];
            //    Marshal.ThrowExceptionForHR(marker.GetDefaultLineStyle(piLineColor, piLineIndex));
            //    IVsHiColorItem vsHiColorItem = marker as IVsHiColorItem;
            //    if (vsHiColorItem == null || ErrorHandler.Failed(vsHiColorItem.GetColorData(2, out crSource)))
            //        Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.EncodeIndexedColor(piLineColor[0], out crSource));
            //}
            //else
            //    Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.EncodeInvalidColor(out crSource));

            //result.Flags = (FcItemflags) 31;
            //if (((int)pdwVisualFlags & 264) != 0 || ((int)pdwVisualFlags & 6) == 0)
            //{
            //    Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.GetColorType(crSource, out int pctType));
            //    if (pctType != 0)
            //    {
            //        foreground = crSource;
            //        if (((int)pdwVisualFlags & 2) == 0)
            //        {
            //            Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.EncodeTrackedItem(0, 1, out pcrResult1));
            //            Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.EncodeIndexedColor(COLORINDEX.CI_SYSPLAINTEXT_BK, out background));
            //            result.Flags &= 4294967291U;
            //        }
            //        result.Flags &= 4294967287U;
            //    }
            //}
            //if (((int)pdwVisualFlags & 262144) != 0)
            //    result.Flags &= 4294967291U;
            //if (((int)pdwVisualFlags & 524288) != 0)
            //    result.Flags &= 4294967293U;
            //uint pcrResult2 = 0;
            //Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.EncodeInvalidColor(out pcrResult2));
            //if (num == 0U || (int)num == (int)pcrResult2)
            //    Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.EncodeIndexedColor(COLORINDEX.CI_USERTEXT_FG, out result.crAutoForeground));
            //else
            //    result.AutoForeground = num;
            //if (pcrResult1 == 0U || (int)pcrResult1 == (int)pcrResult2)
            //    Marshal.ThrowExceptionForHR(Common.FontAndColorUtilities.EncodeIndexedColor(COLORINDEX.CI_USERTEXT_BK, out result.crAutoBackground));
            //else
            //    result.AutoBackground = pcrResult1;
            //if (foreground == 0U || (int)foreground == (int)pcrResult2)
            //    FontsAndColorsHelper.SetForegroundColor(foreground, ref result);
            //else
            //    result.Info.Foreground = foreground;
            //if (background == 0U || (int)background == (int)pcrResult2)
            //    FontsAndColorsHelper.SetBackgroundColor(background, ref result);
            //else
            //    result.Info.Background = background;
            //result.bAutoBackgroundValid = 1;
            //result.bAutoForegroundValid = 1;
            //result.bDescriptionValid = 1;
            //result.bFlagsValid = 1;
            //result.bLineStyleValid = 1;
            //result.bLocalizedNameValid = 1;
            //result.bMarkerVisualStyleValid = 1;
            //result.bNameValid = 1;
            //result.bstrDescription = (string)null;
            //result.bstrLocalizedName = displayName;
            //result.bstrName = canonicalName;
            //result.dwMarkerVisualStyle = pdwVisualFlags;
            //result.eLineStyle = piLineIndex[0];
            //result.Info.dwFontFlags = pdwFontFlags;
            //result.Info.bBackgroundValid = 1;
            //result.Info.bFontFlagsValid = 1;
            //var flag2 = (pdwVisualFlags & 256U) > 0U || !flag1;
            //result.Info.bForegroundValid = flag2 ? 1 : 0;
            //return result;
        }

        private static void GetMarkerName(IPackageDefinedTextMarkerType marker, ref string displayName, ref string canonicalName)
        {
            if (!(marker is IMergeableUiItem mergeableUiItem))
                throw new InvalidOperationException("IVsPackageDefinedTextMarkerType must implement IVsMergeableUIItem");
            mergeableUiItem.GetDisplayName(out displayName);
            if (mergeableUiItem.GetCanonicalName(out canonicalName) < 0)
                canonicalName = null;
            if (displayName == null && canonicalName == null)
            {
                displayName = "Error: " + mergeableUiItem.GetType().FullName;
                canonicalName = displayName;
            }
            if (canonicalName == null)
                canonicalName = displayName;
            if (displayName != null)
                return;
            displayName = canonicalName;
        }

        public override int OnItemChanged(ref Guid rguidCategory, string szItem, int iItem, ColorableItemInfo[] pInfo, uint crLiteralForeground, uint crLiteralBackground)
        {
            if (!ChangedItemIsValid(rguidCategory, iItem))
                return 1;
            var marker = Markers[iItem];
            string pbstrNonLocalizeName;
            Linestyle[] piLineIndex = new Linestyle[1];
            if (marker.IsMetadataBased)
            {
                pbstrNonLocalizeName = marker.ColorData.Name;
                piLineIndex[0] = marker.ColorData.LineStyle;
            }
            else
            {
                pbstrNonLocalizeName = null;
                (marker.Instance as IMergeableUiItem).GetCanonicalName(out pbstrNonLocalizeName);
                //COLORINDEX[] piLineColor = new COLORINDEX[1];
                //marker.Instance.GetDefaultLineStyle(piLineColor, piLineIndex);
            }
            if (pbstrNonLocalizeName == null)
                return 1;
            if (piLineIndex[0] == Linestyle.Squiggly)
            {
                UpdateSquiggleProperties(pbstrNonLocalizeName, FontsAndColorsHelper.GetWpfColor(crLiteralForeground));
            }
            else
            {
                var classificationType = FontsAndColorsHelper.GetOrCreateLegacyMarkerClassificationType(pbstrNonLocalizeName);
                if (!ClassificationFormatMap.IsInBatchUpdate)
                    ClassificationFormatMap.BeginBatchUpdate();
                FontsAndColorsHelper.UpdateLegacyMarkerClassification(ClassificationFormatMap, classificationType, pInfo);
                FontsAndColorsHelper.UpdateLegacyMarkerBackground(EditorFormatMap, pbstrNonLocalizeName, pInfo);
            }
            return 0;
        }

        private void UpdateSquiggleProperties(string formatMapKey, Color color)
        {
            var properties = EditorFormatMap.GetProperties(formatMapKey);
            properties["ForegroundColor"] = color;
            EditorFormatMap.SetProperties(formatMapKey, properties);
        }

        private struct TextMarkerInstance
        {
            public AllColorableItemInfo ColorData { get; }

            public IPackageDefinedTextMarkerType Instance { get; }

            public bool IsMetadataBased => Instance == null;

            public TextMarkerInstance(AllColorableItemInfo metadata)
            {
                ColorData = metadata;
                Instance = null;
            }

            public TextMarkerInstance(IPackageDefinedTextMarkerType instance)
            {
                Instance = instance;
                ColorData = new AllColorableItemInfo();
            }
        }
    }

    public interface IPackageDefinedTextMarkerType : IMergeableUiItem
    {

    }

    public interface IMergeableUiItem
    {
        int GetCanonicalName(out string pbstrNonLocalizeName);

        int GetDisplayName(out string pbstrDisplayName);

        int GetMergingPriority(out int piMergingPriority);

        int GetDescription(out string pbstrDesc);
    }

    public interface IColorableItem : IMergeableUiItem
    {

    }
}