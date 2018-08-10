using System;
using System.Collections.ObjectModel;

namespace ModernApplicationFramework.Text.Ui.Classification
{
    public class FormatItemsEventArgs : EventArgs
    {
        public ReadOnlyCollection<string> ChangedItems { get; }

        public FormatItemsEventArgs(ReadOnlyCollection<string> items)
        {
            ChangedItems = items ?? throw new ArgumentNullException(nameof(items));
        }
    }
}