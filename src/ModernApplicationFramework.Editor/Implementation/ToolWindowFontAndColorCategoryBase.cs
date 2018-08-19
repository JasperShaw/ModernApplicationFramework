using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Editor.TextManager;
using ModernApplicationFramework.Text.Logic.Classification;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal abstract class ToolWindowFontAndColorCategoryBase : FontAndColorCategoryBase
    {
        private IFontAndColorDefaultsProvider _defaultsProvider;
        private List<AllColorableItemInfo> _items;


        private readonly Color _fore = Color.FromRgb(30, 30, 30);
        private readonly Color _back = Colors.White;

        public override Guid CategoryGuid { get; protected set; }

        protected IFontAndColorDefaultsProvider DefaultsProvider => _defaultsProvider ?? (_defaultsProvider = IoC.Get<IFontAndColorDefaultsProvider>());

        protected virtual List<AllColorableItemInfo> Items => _items ?? (_items = new List<AllColorableItemInfo>
        {
            //TODO: Localize (second param)
            FontsAndColorsHelper.GetPlainTextItem("Plain Text", "Plain Text",
                Color.FromRgb(30,30, 30),
                Color.FromRgb(230,231, 232),
                _fore,
                _back),
            FontsAndColorsHelper.GetNonMarkerItem("Selected Text", "Selected Text",
                Colors.Transparent,
                Color.FromRgb(10,36, 106),
                _fore,
                SystemColors.HighlightColor),
            FontsAndColorsHelper.GetNonMarkerItem("Inactive Selected Text",
                "Inactive Selected Text",
                Colors.Transparent,
                Color.FromRgb(187,189, 191),
                _fore, SystemColors.InactiveCaptionColor),
            FontsAndColorsHelper.GetNonMarkerItem("Selected Text in High Contrast",
                "Selected Text in High Contrast",
                _back,
                SystemColors.HighlightColor,
                _back,
                SystemColors.HighlightColor)
        });

        protected virtual bool IsLegacyMarker(string item)
        {
            return false;
        }

        public override AllColorableItemInfo GetItem(int iItem)
        {
            return Items[iItem];
        }

        public override int GetItemByName(string item, out AllColorableItemInfo info)
        {
            info = default;
            foreach (var colorableItemInfo in Items)
            {
                if (colorableItemInfo.Name == item)
                {
                    info = colorableItemInfo;
                    return 0;
                }
            }
            return -2147467259;
        }

        public override int GetItemCount()
        {
            return Items.Count;
        }

        public override ushort GetPriority()
        {
            return (ushort)(3U + GetPriorityOrder());
        }

        public override int OnItemChanged(ref Guid rguidCategory, string szItem, int iItem, ColorableItemInfo[] pInfo, uint crLiteralForeground, uint crLiteralBackground)
        {
            if (!ChangedItemIsValid(rguidCategory, iItem))
                return 1;
            UpdateFormatMap(szItem, pInfo);
            return 0;
        }

        internal void UpdateFormatMap(string szItem, ColorableItemInfo[] pInfo)
        {
            if (szItem.Equals("Plain Text", StringComparison.OrdinalIgnoreCase))
            {

                var properties = EditorFormatMap.GetProperties("TextView Background");
                FontsAndColorsHelper.DecodePlainTextBackground(pInfo);
                var background = pInfo[0].Background;
                var brush = (Brush)new SolidColorBrush(background);
                brush.Freeze();
                properties["BackgroundColor"] = background;
                properties["Background"] = brush;
                if (!EditorFormatMap.IsInBatchUpdate)
                    EditorFormatMap.BeginBatchUpdate();
                EditorFormatMap.SetProperties("TextView Background", properties);

                var formattingRunProperties = ClassificationFormatMap.DefaultTextProperties;

                FontsAndColorsHelper.DecodePlainTextForeground(pInfo);
                var foreground = pInfo[0].Foreground;
                formattingRunProperties = formattingRunProperties.SetForeground(foreground);


                var typeface1 = formattingRunProperties.Typeface;
                if (pInfo[0].FontFlags.HasFlag(FontFlags.Bold))
                {
                    var typeface2 = new Typeface(typeface1.FontFamily, typeface1.Style, FontWeights.Bold, typeface1.Stretch);
                    formattingRunProperties = formattingRunProperties.SetTypeface(typeface2);
                }
                else
                {
                    var typeface2 = new Typeface(typeface1.FontFamily, typeface1.Style, FontWeights.Normal, typeface1.Stretch);
                    formattingRunProperties = formattingRunProperties.SetTypeface(typeface2);
                }

                if (!ClassificationFormatMap.IsInBatchUpdate)
                    ClassificationFormatMap.BeginBatchUpdate();
                ClassificationFormatMap.DefaultTextProperties = formattingRunProperties;
            }
            else if (IsLegacyMarker(szItem))
            {
                IClassificationType classificationType = FontsAndColorsHelper.GetOrCreateLegacyMarkerClassificationType(szItem);
                if (!ClassificationFormatMap.IsInBatchUpdate)
                    ClassificationFormatMap.BeginBatchUpdate();
                FontsAndColorsHelper.UpdateLegacyMarkerClassification(ClassificationFormatMap, classificationType, pInfo);
                FontsAndColorsHelper.UpdateLegacyMarkerBackground(EditorFormatMap, szItem, pInfo);
            }
            else
            {
                if (!EditorFormatMap.IsInBatchUpdate)
                    EditorFormatMap.BeginBatchUpdate();
                var properties = EditorFormatMap.GetProperties(szItem);
                FontsAndColorsHelper.FillResourceDictionary(pInfo, properties, false);
                EditorFormatMap.SetProperties(szItem, properties);
            }
        }

        protected AllColorableItemInfo GetItemForMefName(string name)
        {
            var defaultsProvider = DefaultsProvider;
            var obj = defaultsProvider.GetObject(CategoryGuids.GuidEditorMef);
            ((FontAndColorCategoryBase)obj).GetItemByName(name, out var pInfo);
            return pInfo;
        }

        public override int GetFont(FontInfo[] pInfo)
        {
            //TODO: Text manager stuff
            //GetDefaultFontName();
            var defaultFontFamily = FontsAndColorsHelper.GetWPFDefaultFontFamily();
            pInfo[0].Typeface = FontsAndColorsHelper.GetLocalizedFaceName(defaultFontFamily);
            pInfo[0].CharSet = 1;
            pInfo[0].PointSize = 9;
            return 0;
        }

        //private string GetDefaultFontName()
        //{
        //    string defaultFontface;
        //    (Common.GetService<IVsTextManager, SVsTextManager>(this.ServiceProvider) as IVsPrivTextManager).GetEditorDefaultFontface(out defaultFontface);
        //    return defaultFontface;
        //}
    }
}