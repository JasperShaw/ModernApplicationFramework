using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.Classification
{
    internal class ViewSpecificFormatMap : IClassificationFormatMap, IEditorFormatMap
    {
        private readonly IClassificationFormatMapService _classificationFormatMapService;
        private readonly IEditorFormatMapService _editorFormatMapService;
        private readonly HashSet<string> _requestedEditorFormatMapProperties;
        private readonly ITextView _textView;
        private IClassificationFormatMap _classificationFormatMap;
        private IEditorFormatMap _editorFormatMap;

        public event EventHandler<EventArgs> ClassificationFormatMappingChanged;

        public event EventHandler<FormatItemsEventArgs> FormatMappingChanged;

        public ReadOnlyCollection<IClassificationType> CurrentPriorityOrder =>
            _classificationFormatMap.CurrentPriorityOrder;

        public TextFormattingRunProperties DefaultTextProperties
        {
            get => _classificationFormatMap.DefaultTextProperties;
            set => _classificationFormatMap.DefaultTextProperties = value;
        }

        bool IClassificationFormatMap.IsInBatchUpdate => _classificationFormatMap.IsInBatchUpdate;

        bool IEditorFormatMap.IsInBatchUpdate => _editorFormatMap.IsInBatchUpdate;

        internal ViewSpecificFormatMap(IClassificationFormatMapService classificationFormatMapService,
            IEditorFormatMapService editorFormatMapService, ITextView textView)
        {
            _textView = textView;
            _classificationFormatMapService = classificationFormatMapService;
            _editorFormatMapService = editorFormatMapService;
            _requestedEditorFormatMapProperties = new HashSet<string>();
            _textView.Options.OptionChanged += TextViewOptionsChanged;
            _textView.Closed += TextViewClosed;
            AttachToFormatMaps();
        }

        public void AddExplicitTextProperties(IClassificationType classificationType,
            TextFormattingRunProperties properties)
        {
            _classificationFormatMap.AddExplicitTextProperties(classificationType, properties);
        }

        public void AddExplicitTextProperties(IClassificationType classificationType,
            TextFormattingRunProperties properties, IClassificationType priority)
        {
            _classificationFormatMap.AddExplicitTextProperties(classificationType, properties, priority);
        }

        public void AddProperties(string key, ResourceDictionary properties)
        {
            _requestedEditorFormatMapProperties.Add(key);
            _editorFormatMap.AddProperties(key, properties);
        }

        public string GetEditorFormatMapKey(IClassificationType classificationType)
        {
            return _classificationFormatMap.GetEditorFormatMapKey(classificationType);
        }

        public TextFormattingRunProperties GetExplicitTextProperties(IClassificationType classificationType)
        {
            return _classificationFormatMap.GetExplicitTextProperties(classificationType);
        }

        public ResourceDictionary GetProperties(string key)
        {
            _requestedEditorFormatMapProperties.Add(key);
            return _editorFormatMap.GetProperties(key);
        }

        public TextFormattingRunProperties GetTextProperties(IClassificationType classificationType)
        {
            return _classificationFormatMap.GetTextProperties(classificationType);
        }

        public void SetExplicitTextProperties(IClassificationType classificationType,
            TextFormattingRunProperties properties)
        {
            _classificationFormatMap.SetExplicitTextProperties(classificationType, properties);
        }

        public void SetProperties(string key, ResourceDictionary properties)
        {
            _requestedEditorFormatMapProperties.Add(key);
            _editorFormatMap.SetProperties(key, properties);
        }

        public void SetTextProperties(IClassificationType classificationType, TextFormattingRunProperties properties)
        {
            _classificationFormatMap.SetTextProperties(classificationType, properties);
        }

        public void SwapPriorities(IClassificationType firstType, IClassificationType secondType)
        {
            _classificationFormatMap.SwapPriorities(firstType, secondType);
        }

        void IClassificationFormatMap.BeginBatchUpdate()
        {
            _classificationFormatMap.BeginBatchUpdate();
        }

        void IClassificationFormatMap.EndBatchUpdate()
        {
            _classificationFormatMap.EndBatchUpdate();
        }

        void IEditorFormatMap.BeginBatchUpdate()
        {
            _editorFormatMap.BeginBatchUpdate();
        }

        void IEditorFormatMap.EndBatchUpdate()
        {
            _editorFormatMap.EndBatchUpdate();
        }

        private void AttachToFormatMaps()
        {
            UnsubscribeFromEvents();
            _classificationFormatMap =
                _classificationFormatMapService.GetClassificationFormatMap(_textView.Options.AppearanceCategory());
            _editorFormatMap = _editorFormatMapService.GetEditorFormatMap(_textView.Options.AppearanceCategory());
            _classificationFormatMap.ClassificationFormatMappingChanged += ClassificationFormatMapChanged;
            _editorFormatMap.FormatMappingChanged += EditorFormatMapChanged;
            EditorFormatMapChanged(this,
                new FormatItemsEventArgs(_requestedEditorFormatMapProperties.ToList().AsReadOnly()));
            ClassificationFormatMapChanged(this, EventArgs.Empty);
        }

        private void ClassificationFormatMapChanged(object sender, EventArgs e)
        {
            var formatMappingChanged = ClassificationFormatMappingChanged;
            formatMappingChanged?.Invoke(this, e);
        }

        private void EditorFormatMapChanged(object sender, FormatItemsEventArgs e)
        {
            var formatMappingChanged = FormatMappingChanged;
            formatMappingChanged?.Invoke(this, e);
        }

        private void TextViewClosed(object sender, EventArgs e)
        {
            _textView.Closed -= TextViewClosed;
            _textView.Options.OptionChanged -= TextViewOptionsChanged;
            UnsubscribeFromEvents();
        }

        private void TextViewOptionsChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (e.OptionId != DefaultViewOptions.AppearanceCategory.Name)
                return;
            AttachToFormatMaps();
        }

        private void UnsubscribeFromEvents()
        {
            if (_classificationFormatMap != null)
                _classificationFormatMap.ClassificationFormatMappingChanged -= ClassificationFormatMapChanged;
            if (_editorFormatMap == null)
                return;
            _editorFormatMap.FormatMappingChanged -= EditorFormatMapChanged;
        }
    }
}