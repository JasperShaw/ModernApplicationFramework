using System.Collections.Generic;
using System.Windows;

namespace ModernApplicationFramework.TextEditor
{
    internal class WorkaroundMetadata : ITextViewMarginMetadata
    {
        private readonly string[] _emptyStrings = new string[0];

        public string MarginContainer => string.Empty;

        public IEnumerable<string> Replaces => _emptyStrings;

        public string OptionName => string.Empty;

        public GridUnitType GridUnitType => GridUnitType.Auto;

        public double GridCellLength => 0.0;

        public IEnumerable<string> After => _emptyStrings;

        public IEnumerable<string> Before => _emptyStrings;

        public string Name => string.Empty;

        public IEnumerable<string> ContentTypes => _emptyStrings;

        public IEnumerable<string> TextViewRoles => _emptyStrings;
    }
}