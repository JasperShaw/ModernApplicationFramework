using System;
using System.Collections.ObjectModel;
using System.Windows;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IAdornmentLayer
    {
        ITextView TextView { get; }

        bool AddAdornment(AdornmentPositioningBehavior behavior, SnapshotSpan? visualSpan, object tag,
            UIElement adornment, AdornmentRemovedCallback removedCallback);

        bool AddAdornment(SnapshotSpan visualSpan, object tag, UIElement adornment);

        void RemoveAdornment(UIElement adornment);

        void RemoveAdornmentsByTag(object tag);

        void RemoveAdornmentsByVisualSpan(SnapshotSpan visualSpan);

        void RemoveMatchingAdornments(Predicate<IAdornmentLayerElement> match);

        void RemoveMatchingAdornments(SnapshotSpan visualSpan, Predicate<IAdornmentLayerElement> match);

        void RemoveAllAdornments();

        bool IsEmpty { get; }

        double Opacity { get; set; }

        ReadOnlyCollection<IAdornmentLayerElement> Elements { get; }
    }
}