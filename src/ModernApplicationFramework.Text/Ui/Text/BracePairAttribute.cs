using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Ui.Text
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class BracePairAttribute : MultipleBaseMetadataAttribute
    {
        public BracePairAttribute(char openingBrace, char closingBrace)
        {
            OpeningBraces = openingBrace;
            ClosingBraces = closingBrace;
        }

        public char OpeningBraces { get; }

        public char ClosingBraces { get; }
    }
}
