using System;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Data
{
    public interface ITextBuffer : IPropertyOwner
    {
        IContentType ContentType { get; }

        ITextSnapshot CurrentSnapshot { get; }

        ITextEdit CreateEdit(EditOptions options, int? reiteratedVersionNumber, object editTag);

        ITextEdit CreateEdit();

        IReadOnlyRegionEdit CreateReadOnlyRegionEdit();

        bool EditInProgress { get; }

        void TakeThreadOwnership();

        bool CheckEditAccess();

        event EventHandler<SnapshotSpanEventArgs> ReadOnlyRegionsChanged;

        event EventHandler<TextContentChangedEventArgs> Changed;

        event EventHandler<TextContentChangedEventArgs> ChangedLowPriority;

        event EventHandler<TextContentChangedEventArgs> ChangedHighPriority;

        event EventHandler<TextContentChangingEventArgs> Changing;

        event EventHandler PostChanged;

        event EventHandler<ContentTypeChangedEventArgs> ContentTypeChanged;

        //event EventHandler<TextContentChangedEventArgs> ChangedOnBackground;

        void ChangeContentType(IContentType newContentType, object editTag);

        ITextSnapshot Insert(int position, string text);

        ITextSnapshot Delete(Span deleteSpan);

        ITextSnapshot Replace(Span replaceSpan, string replaceWith);

        bool IsReadOnly(int position);

        bool IsReadOnly(int position, bool isEdit);

        bool IsReadOnly(Span span);

        bool IsReadOnly(Span span, bool isEdit);

        NormalizedSpanCollection GetReadOnlyExtents(Span span);
    }
}
