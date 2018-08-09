using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    //TODO: Add undo stuff

    internal class DragDropStateManager
    {
        internal DragDropInitializer DoDragDrop = DragDrop.DoDragDrop;
        internal DragDropState _state;
        internal bool _isInternalDragDrop;
        private readonly ITextView _textView;
        private readonly IRtfBuilderService2 _rtfBuilderService;
        internal IDropHandler CurrentDropHandler;
        //private readonly ITextUndoHistory _undoHistory;
        private readonly DropHandlerManager _dropHandlerManager;
        internal IList<IReadOnlyRegion> ReadOnlyRegions;
        private readonly DragDropVisualManager _visualManager;
        private readonly GuardedOperations _guardedOperations;

        public DragDropStateManager(ITextView textView, IRtfBuilderService2 rtfBuilderService, DropHandlerManager dropHandlerManager, DragDropVisualManager visualManager, /*ITextUndoHistory undoHistory,*/ GuardedOperations guardedOperations)
        {
            _textView = textView;
            _rtfBuilderService = rtfBuilderService;
            _dropHandlerManager = dropHandlerManager;
            CurrentDropHandler = null;
            ReadOnlyRegions = null;
            _isInternalDragDrop = false;
            _visualManager = visualManager;
            _state = DragDropState.Start;
            ReadOnlyRegions = new List<IReadOnlyRegion>();
            //_undoHistory = undoHistory;
            _guardedOperations = guardedOperations;
        }

        public void SetToStart()
        {
            if (_state == DragDropState.Dragging)
                HandleFromDraggingToStart();
            _state = DragDropState.Start;
        }

        public void SetToMouseDown()
        {
            if (_state != DragDropState.Start)
                return;
            _state = DragDropState.MouseDown;
        }

        public void SetToCanceled()
        {
            if (_state == DragDropState.Dragging)
                HandleFromDraggingToCanceled();
            if (_state == DragDropState.Start)
                HandleFromStartToCanceled();
            _state = DragDropState.Canceled;
        }

        public DragDropPointerEffects SetToDragging(DragDropInfo dragDropInfo)
        {
            var dropPointerEffects = DragDropPointerEffects.None;
            if (_state == DragDropState.Start)
            {
                dropPointerEffects = HandleFromStartToDragging(dragDropInfo);
                if (CurrentDropHandler != null)
                    _state = DragDropState.Dragging;
            }
            return dropPointerEffects;
        }

        public void StartAndFinishDragDrop()
        {
            var selectedSpans = _textView.Selection.SelectedSpans;
            IList<ITrackingSpan> trackingSpanList = new List<ITrackingSpan>(selectedSpans.Count);
            var textSnapshot1 = _textView.TextSnapshot;
            foreach (var snapshotSpan in selectedSpans)
                trackingSpanList.Add(textSnapshot1.CreateTrackingSpan(snapshotSpan, SpanTrackingMode.EdgeExclusive));
            //ITextUndoTransaction transaction = _undoHistory.CreateTransaction();
            AddReadOnlyRegions();
            SetToStart();
            var dragDropEffects = PerformDragDrop();
            if (_state != DragDropState.Dropped && _state != DragDropState.Canceled)
            {
                RemoveReadOnlyRegions();
                if ((dragDropEffects & DragDropEffects.Move) == DragDropEffects.Move && !_textView.Options.GetOptionValue(DefaultTextViewOptions.ViewProhibitUserInputId))
                {
                    using (var edit = _textView.TextBuffer.CreateEdit())
                    {
                        var textSnapshot2 = _textView.TextSnapshot;
                        foreach (var trackingSpan in trackingSpanList)
                            edit.Delete(trackingSpan.GetSpan(textSnapshot2));
                        edit.Apply();
                    }
                }
            }
            //if (dragDropEffects == DragDropEffects.None)
            //    transaction.Cancel();
            //else
            //    transaction.Complete();
            //transaction.Dispose();
            _state = DragDropState.Start;
        }

        public DragDropPointerEffects SetToDropped(DragDropInfo dragDropInfo)
        {
            var dropPointerEffects = DragDropPointerEffects.None;
            if (_state == DragDropState.Dragging)
                dropPointerEffects = HandleFromDraggingToDropped(dragDropInfo);
            _state = DragDropState.Dropped;
            return dropPointerEffects;
        }

        private DragDropEffects PerformDragDrop()
        {
            var dataObject = CreateDataObject();
            _isInternalDragDrop = true;
            var num = (int)DoDragDrop(_textView.VisualElement, dataObject, DragDropEffects.All);
            _isInternalDragDrop = false;
            return (DragDropEffects)num;
        }

        internal DataObject CreateDataObject()
        {
            var dataObject = new DataObject();
            var selection = _textView.Selection;
            var flag = selection.Mode == TextSelectionMode.Box;
            var selectedSpans = selection.SelectedSpans;
            var textData = string.Join(Environment.NewLine, selectedSpans.Select(span => span.GetText()).ToArray());
            dataObject.SetText(textData);
            dataObject.SetText(textData, TextDataFormat.Text);
            if (textData.Length < 1000000)
            {
                using (var cancellationTokenSource = new CancellationTokenSource(250))
                {
                    try
                    {
                        dataObject.SetData(DataFormats.Rtf, _rtfBuilderService.GenerateRtf(selectedSpans, cancellationTokenSource.Token));
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            }
            if (flag)
                dataObject.SetData("ColumnSelect", new object());
            return dataObject;
        }

        private DragDropPointerEffects HandleFromDraggingToDropped(DragDropInfo dragDropInfo)
        {
            var dropPointerEffects = DragDropPointerEffects.None;
            if (dragDropInfo.IsInternal)
                RemoveReadOnlyRegions();
            if (CurrentDropHandler != null)
            {
                dropPointerEffects = _guardedOperations.CallExtensionPoint(() => CurrentDropHandler.HandleDataDropped(dragDropInfo), DragDropPointerEffects.None);
                CurrentDropHandler = null;
            }
            _visualManager.DisableDragDropVisuals();
            return dropPointerEffects;
        }

        private void RemoveReadOnlyRegions()
        {
            using (var readOnlyRegionEdit = _textView.TextBuffer.CreateReadOnlyRegionEdit())
            {
                foreach (var readOnlyRegion in ReadOnlyRegions)
                    readOnlyRegionEdit.RemoveReadOnlyRegion(readOnlyRegion);
                readOnlyRegionEdit.Apply();
            }
            ReadOnlyRegions = new List<IReadOnlyRegion>();
        }

        private DragDropPointerEffects HandleFromStartToDragging(DragDropInfo dragDropInfo)
        {
            var dropHandlerRequest = DragDropPointerEffects.None;
            CurrentDropHandler = _dropHandlerManager.GetSupportingHandler(dragDropInfo);
            if (CurrentDropHandler != null)
            {
                _visualManager.EnableDragDropVisuals();
                dropHandlerRequest = _guardedOperations.CallExtensionPoint(() => CurrentDropHandler.HandleDragStarted(dragDropInfo), DragDropPointerEffects.None);
                DisplayTracker(dragDropInfo, dropHandlerRequest);
            }
            return dropHandlerRequest;
        }

        private void HandleFromDraggingToStart()
        {
            CancelDropHandler();
            _visualManager.DisableDragDropVisuals();
        }

        private void HandleFromDraggingToCanceled()
        {
            RemoveReadOnlyRegions();
            CancelDropHandler();
            _visualManager.DisableDragDropVisuals();
        }

        private void HandleFromStartToCanceled()
        {
            RemoveReadOnlyRegions();
        }

        private void AddReadOnlyRegions()
        {
            using (var readOnlyRegionEdit = _textView.TextBuffer.CreateReadOnlyRegionEdit())
            {
                foreach (var selectedSpan in _textView.Selection.SelectedSpans)
                    ReadOnlyRegions.Add(readOnlyRegionEdit.CreateReadOnlyRegion(selectedSpan));
                readOnlyRegionEdit.Apply();
            }
        }

        private void CancelDropHandler()
        {
            if (CurrentDropHandler == null)
                return;
            _guardedOperations.CallExtensionPoint(() => CurrentDropHandler.HandleDragCanceled());
            CurrentDropHandler = null;
        }

        private void DisplayTracker(DragDropInfo dragDropInfo, DragDropPointerEffects dropHandlerRequest)
        {
            if ((dropHandlerRequest & DragDropPointerEffects.Track) == DragDropPointerEffects.Track)
            {
                VirtualSnapshotPoint bufferPosition = dragDropInfo.VirtualBufferPosition.TranslateTo(_textView.TextSnapshot);
                _visualManager.DrawTracker(_textView.GetTextViewLineContainingBufferPosition(bufferPosition.Position).GetExtendedCharacterBounds(bufferPosition));
            }
            else
                _visualManager.ClearTracker();
        }

        public bool IsInternalDragDrop => _isInternalDragDrop;

        public DragDropState State => _state;

        public IDropHandler DropHandler => CurrentDropHandler;

        internal delegate DragDropEffects DragDropInitializer(DependencyObject source, object data, DragDropEffects allowedEffects);
    }
}