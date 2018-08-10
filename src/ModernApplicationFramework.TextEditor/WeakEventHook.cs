using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.TextEditor
{
    internal class WeakEventHook
    {
        private readonly WeakReference<BaseProjectionBuffer> _targetBuffer;

        public WeakEventHook(BaseProjectionBuffer targetBuffer, BaseBuffer sourceBuffer)
        {
            _targetBuffer = new WeakReference<BaseProjectionBuffer>(targetBuffer);
            SourceBuffer = sourceBuffer;
            sourceBuffer.ChangedImmediate += OnSourceTextChanged;
            sourceBuffer.ContentTypeChangedImmediate += OnSourceBufferContentTypeChanged;
            sourceBuffer.ReadOnlyRegionsChanged += OnSourceBufferReadOnlyRegionsChanged;
        }

        public BaseBuffer SourceBuffer { get; private set; }

        public BaseProjectionBuffer GetTargetBuffer()
        {
            if (_targetBuffer.TryGetTarget(out var target))
                return target;
            UnsubscribeFromSourceBuffer();
            return null;
        }

        private void OnSourceTextChanged(object sender, TextContentChangedEventArgs e)
        {
            GetTargetBuffer()?.OnSourceTextChanged(sender, e);
        }

        private void OnSourceBufferContentTypeChanged(object sender, ContentTypeChangedEventArgs e)
        {
            GetTargetBuffer()?.OnSourceBufferContentTypeChanged(sender, e);
        }

        private void OnSourceBufferReadOnlyRegionsChanged(object sender, SnapshotSpanEventArgs e)
        {
            GetTargetBuffer()?.OnSourceBufferReadOnlyRegionsChanged(sender, e);
        }

        public void UnsubscribeFromSourceBuffer()
        {
            if (SourceBuffer == null)
                return;
            SourceBuffer.ChangedImmediate -= OnSourceTextChanged;
            SourceBuffer.ContentTypeChangedImmediate -= OnSourceBufferContentTypeChanged;
            SourceBuffer.ReadOnlyRegionsChanged -= OnSourceBufferReadOnlyRegionsChanged;
            SourceBuffer = null;
        }
    }
}