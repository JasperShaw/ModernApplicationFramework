﻿using System;
using System.Collections.Generic;
using System.Threading;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Classification;

namespace ModernApplicationFramework.Modules.Editor.Formatting
{
    internal class EmptyClassifier : IAccurateClassifier
    {
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public IList<ClassificationSpan> GetAllClassificationSpans(SnapshotSpan textSpan, CancellationToken cancel)
        {
            return new FrugalList<ClassificationSpan>();
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan textSpan)
        {
            return new FrugalList<ClassificationSpan>();
        }
    }
}