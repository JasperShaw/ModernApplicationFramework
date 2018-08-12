using System.Collections.Generic;
using System.Windows;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    internal class WorkaroundMetadata : ITextViewMarginMetadata
    {
        private readonly string[] _emptyStrings = new string[0];

        public IEnumerable<string> After => _emptyStrings;

        public IEnumerable<string> Before => _emptyStrings;

        public IEnumerable<string> ContentTypes => _emptyStrings;

        public double GridCellLength => 0.0;

        public GridUnitType GridUnitType => GridUnitType.Auto;

        public string MarginContainer => string.Empty;

        public string Name => string.Empty;

        public string OptionName => string.Empty;

        public IEnumerable<string> Replaces => _emptyStrings;

        public IEnumerable<string> TextViewRoles => _emptyStrings;
    }
}