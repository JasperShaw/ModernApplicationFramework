﻿using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Editor.OutputClassifier
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