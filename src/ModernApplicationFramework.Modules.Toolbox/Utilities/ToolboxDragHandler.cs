﻿using System;
using System.Windows;
using ModernApplicationFramework.DragDrop;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Utilities
{
    internal class ToolboxDragHandler : IDragSource
    {
        public void StartDrag(IDragInfo dragInfo)
        {
            DragDrop.DragDrop.DefaultDragHandler.StartDrag(dragInfo);
            if (dragInfo.SourceItem is IToolboxItem toolbox)
            {
                var format = toolbox.DataSource.Data.Format;
                dragInfo.DataFormat = DataFormats.GetDataFormat(format);
                dragInfo.Data = toolbox.DataSource.Data.Data;
                dragInfo.Effects = dragInfo.Data != null ? DragDropEffects.Copy | DragDropEffects.Move : DragDropEffects.None;
            }
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        public void Dropped(IDropInfo dropInfo)
        {
        }

        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
        }

        public void DragCancelled()
        {
        }

        public bool TryCatchOccurredException(Exception exception)
        {
            return false;
        }
    }
}
