using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation.OutputClassifier
{
    [ContentType("Output")]
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IClassificationTag))]
    public class OutputWindowTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (buffer == null)
                throw new ArgumentNullException("The buffer is invalid.");
            return new OutputWindowTagger(buffer) as ITagger<T>;
        }
    }
}