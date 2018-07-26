using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(IClassificationFormatMapService))]
    internal sealed class ClassificationFormatMapService : IClassificationFormatMapService
    {
        private readonly Dictionary<string, IClassificationFormatMap> _classificationFormatMaps = new Dictionary<string, IClassificationFormatMap>();

        [ImportMany]
        internal List<Lazy<EditorFormatDefinition, IClassificationFormatMetadata>> Formats { get; set; }

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry { get; set; }

        [Import]
        internal IEditorFormatMapService EditorFormatMapService { get; set; }

        [Import]
        internal IEditorOptionsFactoryService EditorOptionsService { get; set; }

        public IClassificationFormatMap GetClassificationFormatMap(string category)
        {
            if (!_classificationFormatMaps.TryGetValue(category, out var classificationFormatMap))
            {
                classificationFormatMap = new ClassificationFormatMap(Formats, ClassificationTypeRegistry, EditorFormatMapService.GetEditorFormatMap(category), EditorOptionsService);
                _classificationFormatMaps[category] = classificationFormatMap;
            }
            return classificationFormatMap;
        }

        public IClassificationFormatMap GetClassificationFormatMap(ITextView textView)
        {
            return textView.Properties.GetOrCreateSingletonProperty(() => new ViewSpecificFormatMap(this, EditorFormatMapService, textView));
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("High Priority")]
        [UserVisible(false)]
        [Order(After = "Default Priority")]
        [ClassificationType(ClassificationTypeNames = "dummyClassificationType")]
        internal class HighPriorityClassificationFormatDefinition : ClassificationFormatDefinition
        {
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("Default Priority")]
        [UserVisible(false)]
        [Order(After = "Low Priority", Before = "High Priority")]
        [ClassificationType(ClassificationTypeNames = "dummyClassificationType")]
        internal class DefaultPriorityClassificationFormatDefinition : ClassificationFormatDefinition
        {
        }

        [Export(typeof(EditorFormatDefinition))]
        [Name("Low Priority")]
        [UserVisible(false)]
        [Order(Before = "Default Priority")]
        [ClassificationType(ClassificationTypeNames = "dummyClassificationType")]
        internal class LowPriorityClassificationFormatDefinition : ClassificationFormatDefinition
        {
        }
    }
}