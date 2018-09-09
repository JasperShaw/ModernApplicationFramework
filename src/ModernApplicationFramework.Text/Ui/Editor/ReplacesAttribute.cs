using System;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public sealed class ReplacesAttribute : MultipleBaseMetadataAttribute
    {
        public ReplacesAttribute(string replaces)
        {
            if (replaces == null)
                throw new ArgumentNullException(nameof(replaces));
            if (replaces.Length == 0)
                throw new ArgumentException("replaces is an empty string.");
            Replaces = replaces;
        }

        public string Replaces { get; }
    }
}