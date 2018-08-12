using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.Utilities.Attributes;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Classification
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("any")]
    [TagType(typeof(ClassificationTag))]
    internal class ClassifierTaggerProvider : ITaggerProvider
    {
        [ImportMany(typeof(IClassifierProvider))]
        internal List<Lazy<IClassifierProvider, INamedContentTypeMetadata>> ClassifierProviders { get; set; }

        [Import] internal GuardedOperations GuardedOperations { get; set; }

        [Import] private IContentTypeRegistryService ContentTypeRegistryService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            var classifierList = GuardedOperations.InvokeEligibleFactories(ClassifierProviders,
                provider => provider.GetClassifier(buffer), buffer.ContentType, ContentTypeRegistryService, this);
            if (classifierList.Count <= 0)
                return null;
            return new ClassifierTagger(classifierList) as ITagger<T>;
        }
    }
}