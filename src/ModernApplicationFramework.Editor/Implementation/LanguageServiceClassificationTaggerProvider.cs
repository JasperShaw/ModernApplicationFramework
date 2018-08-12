using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IClassificationTag))]
    [ContentType("code")]
    internal sealed class LanguageServiceClassificationTaggerProvider : ITaggerProvider
    {
        [Import]
        internal IFontsAndColorsInformationService FontAndColorInformationService { get; set; }

        [Import]
        internal IEditorOptionsFactoryService EditorOptionsFactoryService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer == null)
                return null;
            if (TryGetTaggerForBuffer(buffer, out var classifier))
                return classifier as ITagger<T>;
            if (!LanguageServiceClassificationTagger.TryCreateTagger(buffer, FontAndColorInformationService, EditorOptionsFactoryService.GlobalOptions.GetOptionValue(DefaultOptions.LongBufferLineThresholdId), out classifier))
                return null;
            buffer.Properties[typeof(LanguageServiceClassificationTagger)] = new WeakReference(classifier);
            return classifier as ITagger<T>;
        }

        internal static bool TryGetTaggerForBuffer(ITextBuffer buffer, out LanguageServiceClassificationTagger classifier)
        {
            classifier = null;
            if (!buffer.Properties.TryGetProperty(typeof(LanguageServiceClassificationTagger), out WeakReference property) || !property.IsAlive || !((LanguageServiceClassificationTagger)property.Target).IsValidForBuffer(buffer))
                return false;
            classifier = (LanguageServiceClassificationTagger)property.Target;
            return true;
        }
    }
}