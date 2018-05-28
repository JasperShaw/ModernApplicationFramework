using System;
using System.Linq;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

namespace ModernApplicationFramework.Modules.Toolbox
{
    internal class ToolboxDragHandler : IDragSource
    {
        public void StartDrag(IDragInfo dragInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDragHandler.StartDrag(dragInfo);
            if (dragInfo.SourceItem is IToolboxItem toolbox)
            {
                var format = toolbox.Data.GetFormats(false).First();
                dragInfo.DataFormat = DataFormats.GetDataFormat(format);
                //dragInfo.Data = toolbox.Data.GetData(format);
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
