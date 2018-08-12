using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class ConnectionManager
    {
        private readonly GuardedOperations _guardedOperations;
        private readonly List<BaseListener> _listeners = new List<BaseListener>();
        private readonly ITextView _textView;

        public ConnectionManager(ITextView textView,
            ICollection<Lazy<ITextViewConnectionListener,
                IContentTypeAndTextViewRoleMetadata>> textViewConnectionListeners,
            GuardedOperations guardedOperations)
        {
            if (textViewConnectionListeners == null)
                throw new ArgumentNullException(nameof(textViewConnectionListeners));
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _guardedOperations = guardedOperations ?? throw new ArgumentNullException(nameof(guardedOperations));
            var lazyList1 = UiExtensionSelector.SelectMatchingExtensions(textViewConnectionListeners, _textView.Roles);
            if (lazyList1.Count > 0)
                foreach (var lazy in lazyList1)
                {
                    var listenerExport = lazy;
                    var listener = new Listener(listenerExport, guardedOperations);
                    _listeners.Add(listener);
                    var subjectBuffers =
                        textView.BufferGraph.GetTextBuffers(
                            buffer => Match(listenerExport.Metadata, buffer.ContentType));
                    if (subjectBuffers.Count > 0)
                    {
                        var errorSource = listener.ErrorSource;
                        if (errorSource != null)
                            _guardedOperations.CallExtensionPoint(errorSource,
                                () => listener.SubjectBuffersConnected(_textView, ConnectionReason.TextViewLifetime,
                                    subjectBuffers));
                    }
                }

            if (_listeners.Count <= 0)
                return;
            textView.BufferGraph.GraphBuffersChanged += OnGraphBuffersChanged;
            textView.BufferGraph.GraphBufferContentTypeChanged += OnGraphBufferContentTypeChanged;
        }

        public void Close()
        {
            if (_listeners.Count <= 0)
                return;
            foreach (var listener1 in _listeners)
            {
                var listener = listener1;
                var subjectBuffers =
                    _textView.BufferGraph.GetTextBuffers(buffer => Match(listener.Metadata, buffer.ContentType));
                if (subjectBuffers.Count > 0)
                {
                    var errorSource = listener.ErrorSource;
                    if (errorSource != null)
                        _guardedOperations.CallExtensionPoint(errorSource,
                            () => listener.SubjectBuffersDisconnected(_textView, ConnectionReason.TextViewLifetime,
                                subjectBuffers));
                }
            }

            _textView.BufferGraph.GraphBuffersChanged -= OnGraphBuffersChanged;
            _textView.BufferGraph.GraphBufferContentTypeChanged -= OnGraphBufferContentTypeChanged;
        }

        private static bool Match(IContentTypeMetadata metadata, IContentType bufferContentType)
        {
            foreach (var contentType in metadata.ContentTypes)
                if (bufferContentType.IsOfType(contentType))
                    return true;
            return false;
        }

        private void OnGraphBufferContentTypeChanged(object sender, GraphBufferContentTypeChangedEventArgs args)
        {
            var baseListenerList1 = new List<BaseListener>();
            var baseListenerList2 = new List<BaseListener>();
            foreach (var listener in _listeners)
            {
                var flag1 = Match(listener.Metadata, args.BeforeContentType);
                var flag2 = Match(listener.Metadata, args.AfterContentType);
                if (flag1 != flag2 && listener.ErrorSource != null)
                {
                    if (flag1)
                        baseListenerList2.Add(listener);
                    else
                        baseListenerList1.Add(listener);
                }
            }

            var subjectBuffers = new Collection<ITextBuffer>(new List<ITextBuffer>(1)
            {
                args.TextBuffer
            });
            foreach (var baseListener in baseListenerList2)
            {
                var listener = baseListener;
                _guardedOperations.CallExtensionPoint(listener.ErrorSource,
                    () => listener.SubjectBuffersDisconnected(_textView, ConnectionReason.ContentTypeChange,
                        subjectBuffers));
            }

            foreach (var baseListener in baseListenerList1)
            {
                var listener = baseListener;
                _guardedOperations.CallExtensionPoint(listener.ErrorSource,
                    () => listener.SubjectBuffersConnected(_textView, ConnectionReason.ContentTypeChange,
                        subjectBuffers));
            }
        }

        private void OnGraphBuffersChanged(object sender, GraphBuffersChangedEventArgs args)
        {
            if (args.AddedBuffers.Count > 0)
                foreach (var listener1 in _listeners)
                {
                    var listener = listener1;
                    var subjectBuffers = new Collection<ITextBuffer>();
                    foreach (var addedBuffer in args.AddedBuffers)
                        if (Match(listener.Metadata, addedBuffer.ContentType))
                            subjectBuffers.Add(addedBuffer);
                    if (subjectBuffers.Count > 0)
                    {
                        var errorSource = listener.ErrorSource;
                        if (errorSource != null)
                            _guardedOperations.CallExtensionPoint(errorSource,
                                () => listener.SubjectBuffersConnected(_textView, ConnectionReason.BufferGraphChange,
                                    subjectBuffers));
                    }
                }

            if (args.RemovedBuffers.Count <= 0)
                return;
            foreach (var listener1 in _listeners)
            {
                var listener = listener1;
                var subjectBuffers = new Collection<ITextBuffer>();
                foreach (var removedBuffer in args.RemovedBuffers)
                    if (Match(listener.Metadata, removedBuffer.ContentType))
                        subjectBuffers.Add(removedBuffer);
                if (subjectBuffers.Count > 0)
                {
                    var errorSource = listener.ErrorSource;
                    if (errorSource != null)
                        _guardedOperations.CallExtensionPoint(errorSource,
                            () => listener.SubjectBuffersDisconnected(_textView, ConnectionReason.BufferGraphChange,
                                subjectBuffers));
                }
            }
        }

        private abstract class BaseListener
        {
            public abstract object ErrorSource { get; }

            public abstract IContentTypeAndTextViewRoleMetadata Metadata { get; }

            public abstract void SubjectBuffersConnected(ITextView textView, ConnectionReason reason,
                Collection<ITextBuffer> subjectBuffers);

            public abstract void SubjectBuffersDisconnected(ITextView textView, ConnectionReason reason,
                Collection<ITextBuffer> subjectBuffers);
        }

        private class Listener : ListenerCommon<ITextViewConnectionListener>
        {
            public Listener(Lazy<ITextViewConnectionListener, IContentTypeAndTextViewRoleMetadata> importInfo,
                GuardedOperations guardedOperations)
                : base(importInfo, guardedOperations)
            {
            }

            public override void SubjectBuffersConnected(ITextView textView, ConnectionReason reason,
                Collection<ITextBuffer> subjectBuffers)
            {
                Instance?.SubjectBuffersConnected(textView, reason, subjectBuffers);
            }

            public override void SubjectBuffersDisconnected(ITextView textView, ConnectionReason reason,
                Collection<ITextBuffer> subjectBuffers)
            {
                Instance?.SubjectBuffersDisconnected(textView, reason, subjectBuffers);
            }
        }

        private abstract class ListenerCommon<T> : BaseListener
        {
            private readonly GuardedOperations _guardedOperations;
            private readonly Lazy<T, IContentTypeAndTextViewRoleMetadata> _importInfo;
            private T _listener;

            public override object ErrorSource => Instance;

            public override IContentTypeAndTextViewRoleMetadata Metadata => _importInfo.Metadata;

            protected T Instance
            {
                get
                {
                    if (_listener == null)
                        _listener = _guardedOperations.InstantiateExtension(_importInfo, _importInfo);
                    return _listener;
                }
            }

            protected ListenerCommon(Lazy<T, IContentTypeAndTextViewRoleMetadata> importInfo,
                GuardedOperations guardedOperations)
            {
                _importInfo = importInfo;
                _guardedOperations = guardedOperations;
            }
        }
    }
}