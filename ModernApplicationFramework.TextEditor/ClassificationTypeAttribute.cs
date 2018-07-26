using System;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    public sealed class ClassificationTypeAttribute : MultipleBaseMetadataAttribute
    {
        private string _name;

        public string ClassificationTypeNames
        {
            get => _name;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentOutOfRangeException(nameof(value));
                _name = value;
            }
        }
    }
}