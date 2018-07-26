using System;

namespace ModernApplicationFramework.TextEditor
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