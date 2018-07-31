using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextViewMarginMetadata : IOrderable, IContentTypeAndTextViewRoleMetadata, ITextViewRoleMetadata
    {
        string MarginContainer { get; }

        [DefaultValue(null)]
        IEnumerable<string> Replaces { get; }

        [DefaultValue(null)]
        string OptionName { get; }

        [DefaultValue(GridUnitType.Auto)]
        GridUnitType GridUnitType { get; }

        [DefaultValue(1.0)]
        double GridCellLength { get; }
    }
}