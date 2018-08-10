using System;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Text.Data
{
    public interface ITextBuffer : IPropertyOwner
    {
        event EventHandler<TextContentChangedEventArgs> Changed;

        event EventHandler<TextContentChangedEventArgs> ChangedHighPriority;

        event EventHandler<TextContentChangedEventArgs> ChangedLowPriority;

        event EventHandler<TextContentChangingEventArgs> Changing;

        event EventHandler<ContentTypeChangedEventArgs> ContentTypeChanged;

        event EventHandler PostChanged;

        event EventHandler<SnapshotSpanEventArgs> ReadOnlyRegionsChanged;
        IContentType ContentType { get; }

        ITextSnapshot CurrentSnapshot { get; }

        bool EditInProgress { get; }

        //event EventHandler<TextContentChangedEventArgs> ChangedOnBackground;

        void ChangeContentType(IContentType newContentType, object editTag);

        bool CheckEditAccess();

        ITextEdit CreateEdit(EditOptions options, int? reiteratedVersionNumber, object editTag);

        ITextEdit CreateEdit();

        IReadOnlyRegionEdit CreateReadOnlyRegionEdit();

        ITextSnapshot Delete(Span deleteSpan);

        NormalizedSpanCollection GetReadOnlyExtents(Span span);

        ITextSnapshot Insert(int position, string text);

        bool IsReadOnly(int position);

        bool IsReadOnly(int position, bool isEdit);

        bool IsReadOnly(Span span);

        bool IsReadOnly(Span span, bool isEdit);

        ITextSnapshot Replace(Span replaceSpan, string replaceWith);

        void TakeThreadOwnership();
    }
}