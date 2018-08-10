using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
{
    internal class MefFontAndColorCategory : TextEditorFontAndColorCategoryBase
    {
        private List<Lazy<EditorFormatDefinition, IEditorFormatMetadata>> _exportedFormatDefinitions;

        private List<Lazy<EditorFormatDefinition, IEditorFormatMetadata>> ExportedFormatDefinitions
        {
            get
            {
                if (_exportedFormatDefinitions == null)
                {
                    var service = IoC.Get<IEditorFormatMapService>() as EditorFormatMapService;
                    _exportedFormatDefinitions = service?.Formats.Where(export => export.Metadata.UserVisible).ToList();
                }
                return _exportedFormatDefinitions;
            }
        }

        public override int GetItemCount()
        {
            return ExportedFormatDefinitions.Count;
        }

        public override int OnItemChanged(ref Guid rguidCategory, string szItem, int iItem, ColorableItemInfo[] pInfo, uint crLiteralForeground,
            uint crLiteralBackground)
        {
            if (!ChangedItemIsValid(rguidCategory, iItem))
                return 1;
            UpdateFormatMap(szItem, pInfo);
            return 0;
        }

        public override string GetCategoryName()
        {
            return "MEF Items";
        }

        public override int GetItemByName(string item, out AllColorableItemInfo info)
        {
            info = default;
            var export = ExportedFormatDefinitions.FirstOrDefault(i => i.Metadata.Name == item);
            if (export == null)
                return -2147467259;
            info = GetColorableItemInfo(export);
            return 0;
        }

        public override AllColorableItemInfo GetItem(int item)
        {
            return GetColorableItemInfo(ExportedFormatDefinitions[item]);
        }

        internal void UpdateFormatMap(string szItem, ColorableItemInfo[] pInfo)
        {
            if (!EditorFormatMap.IsInBatchUpdate)
                EditorFormatMap.BeginBatchUpdate();
            var properties = EditorFormatMap.GetProperties(szItem);
            FontsAndColorsHelper.FillResourceDictionary(pInfo, properties, true);
            EditorFormatMap.SetProperties(szItem, properties);
        }

        internal static AllColorableItemInfo GetColorableItemInfo(EditorFormatDefinition exportValue, string name)
        {
            var colorableItemInfo = new AllColorableItemInfo();
            if (exportValue.BackgroundColor.HasValue)
            {
                var background = exportValue.BackgroundColor.Value;
                colorableItemInfo.AutoBackground = background;
            }
            else
            {
                if (exportValue.BackgroundBrush is SolidColorBrush backgroundBrush)
                {
                    var color = backgroundBrush.Color;
                    colorableItemInfo.AutoBackground = color;
                }
            }
            if (exportValue.ForegroundColor.HasValue)
            {
                var background = exportValue.ForegroundColor.Value;
                colorableItemInfo.AutoForeground = background;
            }
            else
            {
                if (exportValue.ForegroundBrush is SolidColorBrush foregroundBrush)
                {
                    var color = foregroundBrush.Color;
                    colorableItemInfo.AutoForeground = color;
                }
            }
            colorableItemInfo.AutoBackgroundValid = true;
            colorableItemInfo.AutoForegroundValid = true;
            colorableItemInfo.Info.Background = colorableItemInfo.AutoBackground;
            colorableItemInfo.Info.Foreground = colorableItemInfo.AutoForeground;
            colorableItemInfo.NameValid = true;
            colorableItemInfo.Name = name;
            colorableItemInfo.LocalizedNameValid = true;
            colorableItemInfo.LocalizedName = exportValue.DisplayName;
            if (string.IsNullOrEmpty(colorableItemInfo.LocalizedName))
                colorableItemInfo.LocalizedName = name;
            colorableItemInfo.DescriptionValid = true;
            colorableItemInfo.Description = string.Empty;
            colorableItemInfo.MarkerVisualStyleValid = true;
            colorableItemInfo.MarkerVisualStyle = 0U;
            colorableItemInfo.LineStyleValid = true;
            colorableItemInfo.LineStyle = Implementation.Linestyle.None;
            colorableItemInfo.FlagsValid = true;
            colorableItemInfo.Flags = FcItemflags.AllowCustomColors;
            var nullable = exportValue.BackgroundCustomizable;
            if (nullable.HasValue)
            {
                nullable = exportValue.BackgroundCustomizable;
                if (!nullable.Value)
                    goto label_15;
            }
            colorableItemInfo.Flags |= FcItemflags.AllowBgChange;
            label_15:
            nullable = exportValue.ForegroundCustomizable;
            if (nullable.HasValue)
            {
                nullable = exportValue.ForegroundCustomizable;
                if (!nullable.Value)
                    goto label_18;
            }
            colorableItemInfo.Flags |= FcItemflags.AllowFgChange;
            label_18:
            if (exportValue is ClassificationFormatDefinition formatDefinition)
            {
                colorableItemInfo.Flags |= FcItemflags.AllowBoldChange;
                nullable = formatDefinition.IsBold;
                if (nullable.HasValue)
                {
                    nullable = formatDefinition.IsBold;
                    if (nullable.Value)
                        colorableItemInfo.Info.FontFlags = FontFlags.Bold;
                }
            }
            if (exportValue is MarkerFormatDefinition)
                colorableItemInfo.Flags |= FcItemflags.IsMarker;
            return colorableItemInfo;
        }

        protected override ushort GetPriorityOrder()
        {
            return 4;
        }

        private static AllColorableItemInfo GetColorableItemInfo(Lazy<EditorFormatDefinition, IEditorFormatMetadata> export)
        {
            return GetColorableItemInfo(export.Value, export.Metadata.Name);
        }
    }
}