﻿using System;

namespace ModernApplicationFramework.TextEditor
{
    internal class VacuousTextViewModel : ITextViewModel
    {
        public ITextDataModel DataModel { get; }
        public ITextBuffer DataBuffer => DataModel.DataBuffer;
        public ITextBuffer EditBuffer { get; }
        public ITextBuffer VisualBuffer => EditBuffer;

        public PropertyCollection Properties { get; }

        public VacuousTextViewModel(ITextDataModel dataModel) : this(dataModel, dataModel.DataBuffer)
        {
        }

        public VacuousTextViewModel(ITextDataModel dataModel, ITextBuffer editBuffer)
        {
            DataModel = dataModel;
            EditBuffer = editBuffer;
            Properties = new PropertyCollection();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool IsPointInVisualBuffer(SnapshotPoint editBufferPoint, PositionAffinity affinity)
        {
            return true;
        }

        public SnapshotPoint GetNearestPointInVisualBuffer(SnapshotPoint editBufferPoint)
        {
            return editBufferPoint;
        }

        public SnapshotPoint GetNearestPointInVisualSnapshot(SnapshotPoint editBufferPoint, ITextSnapshot targetVisualSnapshot,
            PointTrackingMode trackingMode)
        {
            return editBufferPoint.TranslateTo(targetVisualSnapshot, trackingMode);
        }
    }
}
