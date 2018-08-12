using System;
using System.Windows.Media;
using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Text.Ui.Classification;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal abstract class FontAndColorCategoryBase : IFontAndColorDefaults, IFontAndColorEvents
    {
        private IClassificationFormatMap _classificationFormatMap;
        private IEditorFormatMap _editorFormatMap;

        protected virtual Guid CategoryGuid { set; get; }

        private Guid FontAndColorCategory
        {
            get
            {
                if (this is EditorTextManagerFontAndColorCategory || this is EditorMefFontAndColorCategory)
                    return CategoryGuids.GuidTextEditorGroup;
                if (this is PrinterTextManagerFontAndColorCategory)
                    return CategoryGuids.GuidPrinterGroup;
                return CategoryGuid;
            }
        }

        protected IClassificationFormatMap ClassificationFormatMap =>
            _classificationFormatMap ?? (_classificationFormatMap =
                EditorParts.ClassificationFormatMapService.GetClassificationFormatMap(
                    new FontsAndColorsCategory(ImplGuidList.GuidDefaultFileType, FontAndColorCategory,
                        FontAndColorCategory).AppearanceCategory));

        protected IEditorFormatMap EditorFormatMap =>
            _editorFormatMap ?? (_editorFormatMap = EditorParts.EditorFormatMapService.GetEditorFormatMap(
                new FontsAndColorsCategory(ImplGuidList.GuidDefaultFileType, FontAndColorCategory,
                    FontAndColorCategory).AppearanceCategory));

        public void GetBaseCategory(out Guid guidBase)
        {
            guidBase = Guid.Empty;
        }

        public void GetFlags(out FontColorFlags dwFlags)
        {
            dwFlags = FontColorFlags.OnlyTtFonts;
        }

        public virtual int GetFont(FontInfo[] pInfo)
        {
            if (pInfo == null)
                return -2147024809;
            var defaultFontFamily = FontsAndColorsHelper.GetWPFDefaultFontFamily();
            pInfo[0].Typeface = FontsAndColorsHelper.GetLocalizedFaceName(defaultFontFamily);
            pInfo[0].CharSet = 1;
            pInfo[0].PointSize = 10;
            return 0;
        }

        public virtual ushort GetPriority()
        {
            return (ushort) (0 + GetPriorityOrder());
        }

        public abstract string GetCategoryName();

        public abstract AllColorableItemInfo GetItem(int item);

        public abstract int GetItemByName(string item, out AllColorableItemInfo info);

        public abstract int GetItemCount();

        protected virtual ushort GetPriorityOrder()
        {
            return 0;
        }

        public virtual int OnFontChanged(ref Guid rguidCategory, FontInfo[] pInfo, Logfont[] pLogfont, uint hfont)
        {
            if (pInfo == null)
                return -2147024809;
            if (!rguidCategory.Equals(CategoryGuid))
                return 1;
            if (!ClassificationFormatMap.IsInBatchUpdate)
                ClassificationFormatMap.BeginBatchUpdate();
            var formattingRunProperties = ClassificationFormatMap.DefaultTextProperties;

            if (pInfo[0].IsValid)
            {
                Typeface typefaceFromFont = FontsAndColorsHelper.GetTypefaceFromFont(pInfo[0].Typeface);
                formattingRunProperties = formattingRunProperties.SetTypeface(typefaceFromFont);
                double renderingSize = DataStorage.FontSizeFromPointSize((int)pInfo[0].PointSize);
                formattingRunProperties = formattingRunProperties.SetFontRenderingEmSize(renderingSize);
            }

            ClassificationFormatMap.DefaultTextProperties = formattingRunProperties;
            return 0;
        }

        public virtual int OnApply()
        {
            if (ClassificationFormatMap.IsInBatchUpdate)
                ClassificationFormatMap.EndBatchUpdate();
            if (EditorFormatMap.IsInBatchUpdate)
                EditorFormatMap.EndBatchUpdate();
            return 0;
        }

        public abstract int OnItemChanged(ref Guid rguidCategory, string szItem, int iItem, ColorableItemInfo[] pInfo, uint crLiteralForeground, uint crLiteralBackground);


        internal bool ChangedItemIsValid(Guid rguidCategory, int iItem)
        {
            return iItem >= 0 && rguidCategory.Equals(CategoryGuid);
        }
    }

    public interface IFontAndColorEvents
    {
        int OnFontChanged(ref Guid rguidCategory, FontInfo[] pInfo, Logfont[] pLogfont, uint hfont);

        int OnItemChanged(ref Guid rguidCategory, string szItem, int iItem, ColorableItemInfo[] pInfo, uint crLiteralForeground, uint crLiteralBackground);

        int OnApply();
    }

    public interface IFontAndColorDefaults
    {
        void GetFlags(out FontColorFlags flags);

        int GetFont(FontInfo[] pInfo);

        void GetBaseCategory(out Guid guidBase);

        ushort GetPriority();

        AllColorableItemInfo GetItem(int item);

        int GetItemCount();

        string GetCategoryName();

        int GetItemByName(string item, out AllColorableItemInfo info);
    }
}