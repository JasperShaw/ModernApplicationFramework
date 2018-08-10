using System;
using ModernApplicationFramework.Text.Logic.Classification;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public class ClassificationTag : IClassificationTag
    {
        public ClassificationTag(IClassificationType type)
        {
            ClassificationType = type ?? throw new ArgumentNullException(nameof(type));
        }

        public IClassificationType ClassificationType { get; }
    }
}