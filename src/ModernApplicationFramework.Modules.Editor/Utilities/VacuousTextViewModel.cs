﻿using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    internal class VacuousTextViewModel : ITextViewModel
    {
        public ITextBuffer DataBuffer => DataModel.DataBuffer;
        public ITextDataModel DataModel { get; }
        public ITextBuffer EditBuffer { get; }

        public PropertyCollection Properties { get; }
        public ITextBuffer VisualBuffer => EditBuffer;

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

        public SnapshotPoint GetNearestPointInVisualBuffer(SnapshotPoint editBufferPoint)
        {
            return editBufferPoint;
        }

        public SnapshotPoint GetNearestPointInVisualSnapshot(SnapshotPoint editBufferPoint,
            ITextSnapshot targetVisualSnapshot,
            PointTrackingMode trackingMode)
        {
            return editBufferPoint.TranslateTo(targetVisualSnapshot, trackingMode);
        }

        public bool IsPointInVisualBuffer(SnapshotPoint editBufferPoint, PositionAffinity affinity)
        {
            return true;
        }
    }
}