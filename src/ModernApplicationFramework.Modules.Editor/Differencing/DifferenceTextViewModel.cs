using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Differencing;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal class DifferenceTextViewModel : IDifferenceTextViewModel
    {
        public ITextDataModel DataModel { get; }

        public ITextBuffer DataBuffer => DataModel.DataBuffer;

        public ITextBuffer EditBuffer { get; }

        public ITextBuffer VisualBuffer => EditBuffer;

        public SnapshotPoint GetNearestPointInVisualBuffer(SnapshotPoint editBufferPoint)
        {
            if (editBufferPoint.Snapshot.TextBuffer != EditBuffer)
                throw new ArgumentException("The given snapshot point must be on the edit buffer.", nameof(editBufferPoint));
            return editBufferPoint;
        }

        public SnapshotPoint GetNearestPointInVisualSnapshot(SnapshotPoint editBufferPoint, ITextSnapshot targetVisualSnapshot, PointTrackingMode trackingMode)
        {
            if (targetVisualSnapshot == null)
                throw new ArgumentNullException(nameof(targetVisualSnapshot));
            if (targetVisualSnapshot.TextBuffer != VisualBuffer)
                throw new ArgumentException("The given snapshot must be on the visual buffer.", nameof(targetVisualSnapshot));
            return editBufferPoint.TranslateTo(targetVisualSnapshot, trackingMode);
        }

        public bool IsPointInVisualBuffer(SnapshotPoint editBufferPoint, PositionAffinity affinity)
        {
            return true;
        }

        public PropertyCollection Properties { get; }

        public void Dispose()
        {
        }

        public IDifferenceViewer Viewer { get; }

        public DifferenceViewType ViewType { get; }

        public DifferenceTextViewModel(IDifferenceViewer viewer, DifferenceViewType viewType, ITextBuffer dataBuffer, ITextBuffer editBuffer)
        {
            DataModel = new DiffDataModel(dataBuffer);
            Properties = new PropertyCollection();
            Viewer = viewer;
            ViewType = viewType;
            EditBuffer = editBuffer;
        }

        internal class DiffDataModel : ITextDataModel
        {
            public DiffDataModel(ITextBuffer dataBuffer)
            {
                DataBuffer = dataBuffer ?? throw new ArgumentNullException(nameof(dataBuffer));
                dataBuffer.ContentTypeChanged += OnContentTypeChanged;
            }

            public event EventHandler<TextDataModelContentTypeChangedEventArgs> ContentTypeChanged;

            private void OnContentTypeChanged(object sender, ContentTypeChangedEventArgs args)
            {
                EventHandler<TextDataModelContentTypeChangedEventArgs> contentTypeChanged = ContentTypeChanged;
                contentTypeChanged?.Invoke(this, new TextDataModelContentTypeChangedEventArgs(args.BeforeContentType, args.AfterContentType));
            }

            public IContentType ContentType => DataBuffer.ContentType;

            public ITextBuffer DocumentBuffer => DataBuffer;

            public ITextBuffer DataBuffer { get; }
        }
    }
}