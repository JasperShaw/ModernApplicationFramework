using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Storage;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IEditorFormatMapService))]
    internal sealed class EditorFormatMapService : IEditorFormatMapService
    {
        private readonly Dictionary<string, IEditorFormatMap> _formatMaps = new Dictionary<string, IEditorFormatMap>();
        [Import]
        private GuardedOperations _guardedOperations;

        [ImportMany]
        internal List<Lazy<EditorFormatDefinition, IEditorFormatMetadata>> Formats { get; set; }

        [Import]
        internal IClassificationFormatMapService ClassificationFormatMapService { get; set; }

        [Import(typeof(IDataStorageService), AllowDefault = true)]
        internal IDataStorageService DataStorageService { get; set; }

        public IEditorFormatMap GetEditorFormatMap(string category)
        {
            if (!_formatMaps.TryGetValue(category, out var editorFormatMap))
            {
                IDataStorage dataStorage = null;
                if (DataStorageService != null)
                    dataStorage = DataStorageService.GetDataStorage(category);
                editorFormatMap = new EditorFormatMap(Formats, dataStorage, _guardedOperations);
                _formatMaps[category] = editorFormatMap;
            }
            return editorFormatMap;
        }

        public IEditorFormatMap GetEditorFormatMap(ITextView textView)
        {
            return textView.Properties.GetOrCreateSingletonProperty(() => new ViewSpecificFormatMap(ClassificationFormatMapService, this, textView));
        }
    }
}