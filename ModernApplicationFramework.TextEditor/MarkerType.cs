using System;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
{
    internal class MarkerType
    {
        public readonly int MarkerTypeID;
        public readonly IVsTextMarkerType VsMarkerType;
        public readonly int Priority;
        public readonly uint VisualStyle;
        public readonly uint Behavior;
        public readonly Linestyle LineStyle;
        private bool _lazyStateInitialized;
        private IClassificationType _textClassificationType;
        private string _squiggleName;
        public const string MarkerPlaceHolder = "MarkerPlaceHolder";
        private IClassificationFormatMap _classificationFormatMap;
        private IEditorFormatMap _editorFormatMap;

        public IClassificationType TextClassificationType
        {
            get
            {
                EnsureLazyStateInitialized();
                return _textClassificationType;
            }
        }

        public string SquiggleName
        {
            get
            {
                EnsureLazyStateInitialized();
                return _squiggleName;
            }
        }

        //protected IVsFontAndColorStorage FCStorage
        //{
        //    get
        //    {
        //        if (this._fcStorage == null)
        //            this._fcStorage = Common.GetService<IVsFontAndColorStorage, SVsFontAndColorStorage>(Common.GlobalServiceProvider);
        //        return this._fcStorage;
        //    }
        //}

        protected IClassificationFormatMap ClassificationFormatMap => null;
            //_classificationFormatMap ?? (_classificationFormatMap =
            //    EditorParts.ClassificationFormatMapService.GetClassificationFormatMap(
            //        new FontsAndColorsCategory(
            //                ImplGuidList.guidDefaultFileType,
            //                CategoryGuids.GuidTextEditorGroup, CategoryGuids.GuidTextEditorGroup)
            //            .AppearanceCategory));

        protected IEditorFormatMap EditorFormatMap => null;
            //_editorFormatMap ?? (_editorFormatMap = EditorParts.EditorFormatMapService.GetEditorFormatMap(
            //                                              new FontsAndColorsCategory(ImplGuidList.guidDefaultFileType,
            //                                                      CategoryGuids.GuidTextEditorGroup, CategoryGuids.GuidTextEditorGroup)
            //                                                  .AppearanceCategory));

        public MarkerType(int markerTypeId, IVsTextMarkerType vsMarkerType)
        {
            MarkerTypeID = markerTypeId;
            VsMarkerType = vsMarkerType;
            //if (ErrorHandler.Failed(VsMarkerType.GetPriorityIndex(out Priority)))
                Priority = 0;
            //if (ErrorHandler.Failed(VsMarkerType.GetVisualStyle(out VisualStyle)))
                VisualStyle = 0U;
            Colorindex[] piLineColor = new Colorindex[1];
            Linestyle[] piLineIndex = new Linestyle[1];
            //LineStyle = ErrorHandler.Failed(VsMarkerType.GetDefaultLineStyle(piLineColor, piLineIndex)) ? Linestyle.LiNone : piLineIndex[0];
            //if (!ErrorHandler.Failed(VsMarkerType.GetBehaviorFlags(out Behavior)))
            //    return;
            Behavior = 0U;
        }

        private void EnsureLazyStateInitialized()
        {
            if (_lazyStateInitialized)
                return;
            _lazyStateInitialized = true;
            if (MarkerTypeID <= 0)
                return;
            string mergeName = MergeName;
            if (LineStyle == Linestyle.Squiggly)
            {
                _squiggleName = mergeName;
            }
            else
            {
                //_textClassificationType = FontsAndColorsHelper.GetOrCreateLegacyMarkerClassificationType(mergeName);
                UpdateMarkerFormat(mergeName, _textClassificationType);
            }
        }

        private void UpdateMarkerFormat(string markerName, IClassificationType classificationType)
        {
            //ColorableItemInfo[] colorableItemInfoArray = new ColorableItemInfo[1];
            //IVsFontAndColorStorage fcStorage = FCStorage;
            //Guid editorTextMarker = CategoryGuids.GuidEditorTextMarker;
            //ref Guid local = ref editorTextMarker;
            //int num = 2;
            //fcStorage.OpenCategory(ref local, (uint)num);
            //FCStorage.GetItem(markerName, colorableItemInfoArray);
            //FCStorage.CloseCategory();
            if (!ClassificationFormatMap.IsInBatchUpdate)
                ClassificationFormatMap.BeginBatchUpdate();
            //FontsAndColorsHelper.UpdateLegacyMarkerClassification(ClassificationFormatMap, classificationType, colorableItemInfoArray);
            //FontsAndColorsHelper.UpdateLegacyMarkerBackground(EditorFormatMap, markerName, colorableItemInfoArray);
            if (!ClassificationFormatMap.IsInBatchUpdate)
                return;
            ClassificationFormatMap.EndBatchUpdate();
        }

        //private bool HasColor => (VisualStyle & 71U) > 0U;

        public string DisplayName => string.Empty;

        public string MergeName => string.Empty;
    }
}