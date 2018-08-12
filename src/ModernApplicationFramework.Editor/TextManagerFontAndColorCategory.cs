using System;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using SystemColors = System.Windows.SystemColors;

namespace ModernApplicationFramework.Editor
{
    internal class TextManagerFontAndColorCategory : TextEditorFontAndColorCategoryBase
    {
        private AllColorableItemInfo[] _items;

        private AllColorableItemInfo[] Items
        {
            get
            {
                if (_items == null)
                {
                    var fore = Color.FromRgb(30, 30, 30);
                    var back = Colors.White;

                    var autoColorBg = Colors.White;
                    var autoColorFg = Color.FromRgb(30, 30, 30);


                    _items = new[]
                    {
                        //TODO: Text (Second param)
                        FontsAndColorsHelper.GetPlainTextItem("Plain Text", "Plain Text", fore, back, autoColorFg, autoColorBg),
                        FontsAndColorsHelper.GetNonMarkerItem("Selected Text", "Selected Text", Colors.Transparent, SystemColors.HighlightColor, autoColorFg ,SystemColors.HighlightColor),
                        FontsAndColorsHelper.GetNonMarkerItem("Inactive Selected Text", "Inactive Selected Text",Colors.Transparent, SystemColors.InactiveCaptionColor, autoColorFg, SystemColors.InactiveSelectionHighlightBrush.Color),
                        FontsAndColorsHelper.GetNonMarkerItem("Indicator Margin", "Indicator Margin",Colors.Transparent, Color.FromRgb(230, 231, 232), autoColorFg, autoColorBg),
                        FontsAndColorsHelper.GetNonMarkerItem("Visible Whitespace", "Visible Whitespace", Color.FromRgb(43, 145, 175), Colors.Transparent, autoColorFg, autoColorBg)
                    };
                }
                return _items;
            }
        }

        public override string GetCategoryName()
        {
            return "Text Manager Items";
        }

        public override int GetItemCount()
        {
            return Items.Length;
        }

        public override AllColorableItemInfo GetItem(int item)
        {
            return Items[item];
        }

        public override int GetItemByName(string item, out AllColorableItemInfo info)
        {
            info = default;
            foreach (var colorableItemInfo in Items)
            {
                if (colorableItemInfo.Name != item)
                    continue;
                info = colorableItemInfo;
                return 0;
            }
            return -2147467259;
        }

        protected override ushort GetPriorityOrder()
        {
            return 1;
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
                Brush brush = new SolidColorBrush(background);
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

                formattingRunProperties = formattingRunProperties.SetBold(pInfo[0].FontFlags.HasFlag(FontFlags.Bold));

                if (!ClassificationFormatMap.IsInBatchUpdate)
                    ClassificationFormatMap.BeginBatchUpdate();
                ClassificationFormatMap.DefaultTextProperties = formattingRunProperties;
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
    }
}