using System;
using ModernApplicationFramework.Modules.Editor.Text;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Projection
{
    internal class WeakEventHook
    {
        private readonly WeakReference<BaseProjectionBuffer> _targetBuffer;

        public BaseBuffer SourceBuffer { get; private set; }

        public WeakEventHook(BaseProjectionBuffer targetBuffer, BaseBuffer sourceBuffer)
        {
            _targetBuffer = new WeakReference<BaseProjectionBuffer>(targetBuffer);
            SourceBuffer = sourceBuffer;
            sourceBuffer.ChangedImmediate += OnSourceTextChanged;
            sourceBuffer.ContentTypeChangedImmediate += OnSourceBufferContentTypeChanged;
            sourceBuffer.ReadOnlyRegionsChanged += OnSourceBufferReadOnlyRegionsChanged;
        }

        public BaseProjectionBuffer GetTargetBuffer()
        {
            if (_targetBuffer.TryGetTarget(out var target))
                return target;
            UnsubscribeFromSourceBuffer();
            return null;
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

        private void OnSourceBufferContentTypeChanged(object sender, ContentTypeChangedEventArgs e)
        {
            GetTargetBuffer()?.OnSourceBufferContentTypeChanged(sender, e);
        }

        private void OnSourceBufferReadOnlyRegionsChanged(object sender, SnapshotSpanEventArgs e)
        {
            GetTargetBuffer()?.OnSourceBufferReadOnlyRegionsChanged(sender, e);
        }

        private void OnSourceTextChanged(object sender, TextContentChangedEventArgs e)
        {
            GetTargetBuffer()?.OnSourceTextChanged(sender, e);
        }
    }
}