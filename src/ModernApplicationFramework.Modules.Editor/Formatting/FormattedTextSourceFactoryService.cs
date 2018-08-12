using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Formatting
{
    [Export(typeof(IFormattedTextSourceFactoryService))]
    internal sealed class FormattedTextSourceFactoryService : IFormattedTextSourceFactoryService
    {
        private readonly TextInfoCache _displayModeTextInfoCache = new TextInfoCache(true);
        private readonly TextInfoCache _idealModeTextInfoCache = new TextInfoCache(false);

        [Import]
        private IClassificationTypeRegistryService ClassificationTypeRegistry { get; set; }

        [ImportMany]
        private List<Lazy<ITextParagraphPropertiesFactoryService, IContentTypeMetadata>> ParagraphPropertiesFactories { get; set; }

        [Import]
        private GuardedOperations GuardedOperations { get; set; }

        [Import]
        private IContentTypeRegistryService ContentTypeRegistry { get; set; }

        internal void Initialize(IClassificationTypeRegistryService classificationTypeRegistry, List<Lazy<ITextParagraphPropertiesFactoryService, IContentTypeMetadata>> paragraphPropertiesFactories)
        {
            ClassificationTypeRegistry = classificationTypeRegistry;
            ParagraphPropertiesFactories = paragraphPropertiesFactories;
        }

        public IFormattedLineSource Create(ITextSnapshot sourceTextSnapshot, ITextSnapshot visualBufferSnapshot, int tabSize, double baseIndentation, double wordWrapWidth, double maxAutoIndent, bool useDisplayMode, IClassifier aggregateClassifier, ITextAndAdornmentSequencer sequencer, IClassificationFormatMap classificationFormatMap)
        {
            return Create(sourceTextSnapshot, visualBufferSnapshot, tabSize, baseIndentation, wordWrapWidth, maxAutoIndent, useDisplayMode, aggregateClassifier, sequencer, classificationFormatMap, false);
        }

        public IFormattedLineSource Create(ITextSnapshot sourceTextSnapshot, ITextSnapshot visualBufferSnapshot, int tabSize, double baseIndentation, double wordWrapWidth, double maxAutoIndent, bool useDisplayMode, IClassifier aggregateClassifier, ITextAndAdornmentSequencer sequencer, IClassificationFormatMap classificationFormatMap, bool isViewWrapEnabled)
        {
            if (sourceTextSnapshot == null)
                throw new ArgumentNullException(nameof(sourceTextSnapshot));
            if (visualBufferSnapshot == null)
                throw new ArgumentNullException(nameof(visualBufferSnapshot));
            if (tabSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(tabSize));
            if (double.IsNaN(baseIndentation) || baseIndentation < 0.0)
                throw new ArgumentOutOfRangeException(nameof(baseIndentation));
            if (double.IsNaN(wordWrapWidth) || wordWrapWidth < 0.0)
                throw new ArgumentOutOfRangeException(nameof(wordWrapWidth));
            if (double.IsNaN(maxAutoIndent) || maxAutoIndent < 0.0)
                throw new ArgumentOutOfRangeException(nameof(maxAutoIndent));
            if (aggregateClassifier == null)
                throw new ArgumentNullException(nameof(aggregateClassifier));
            if (sequencer == null)
                throw new ArgumentNullException(nameof(sequencer));
            if (classificationFormatMap == null)
                throw new ArgumentNullException(nameof(classificationFormatMap));
            var textInfoCache = useDisplayMode ? _displayModeTextInfoCache : _idealModeTextInfoCache;
            var textInfo = textInfoCache.GetTextInfo(classificationFormatMap.DefaultTextProperties);
            var viewWrapProperties = isViewWrapEnabled ? new ViewWrapProperties(textInfo.SpaceWidth * 1.5, classificationFormatMap.GetTextProperties(ClassificationTypeRegistry.GetClassificationType("word wrap glyph")).ForegroundBrush) : null;
            return new FormattedLineSource(sourceTextSnapshot, visualBufferSnapshot, tabSize, textInfo, baseIndentation, wordWrapWidth, maxAutoIndent, aggregateClassifier, sequencer, classificationFormatMap, viewWrapProperties, textInfoCache, ParagraphPropertiesFactories, GuardedOperations, ContentTypeRegistry);
        }

        public IFormattedLineSource Create(ITextSnapshot sourceTextSnapshot, ITextSnapshot visualBufferSnapshot, int tabSize, double baseIndentation, double wordWrapWidth, double maxAutoIndent, bool useDisplayMode, ITextAndAdornmentSequencer sequencer, IClassificationFormatMap classificationFormatMap)
        {
            return Create(sourceTextSnapshot, visualBufferSnapshot, tabSize, baseIndentation, wordWrapWidth, maxAutoIndent, useDisplayMode, new EmptyClassifier(), sequencer, classificationFormatMap);
        }
    }
}