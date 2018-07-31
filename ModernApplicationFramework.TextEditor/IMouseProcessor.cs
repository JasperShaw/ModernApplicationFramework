using System.Windows;
using System.Windows.Input;

namespace ModernApplicationFramework.TextEditor
{
    public interface IMouseProcessor
    {
        void PreprocessMouseLeftButtonDown(MouseButtonEventArgs e);

        void PostprocessMouseLeftButtonDown(MouseButtonEventArgs e);

        void PreprocessMouseRightButtonDown(MouseButtonEventArgs e);

        void PostprocessMouseRightButtonDown(MouseButtonEventArgs e);

        void PreprocessMouseLeftButtonUp(MouseButtonEventArgs e);

        void PostprocessMouseLeftButtonUp(MouseButtonEventArgs e);

        void PreprocessMouseRightButtonUp(MouseButtonEventArgs e);

        void PostprocessMouseRightButtonUp(MouseButtonEventArgs e);

        void PreprocessMouseUp(MouseButtonEventArgs e);

        void PostprocessMouseUp(MouseButtonEventArgs e);

        void PreprocessMouseDown(MouseButtonEventArgs e);

        void PostprocessMouseDown(MouseButtonEventArgs e);

        void PreprocessMouseMove(MouseEventArgs e);

        void PostprocessMouseMove(MouseEventArgs e);

        void PreprocessMouseWheel(MouseWheelEventArgs e);

        void PostprocessMouseWheel(MouseWheelEventArgs e);

        void PreprocessMouseEnter(MouseEventArgs e);

        void PostprocessMouseEnter(MouseEventArgs e);

        void PreprocessMouseLeave(MouseEventArgs e);

        void PostprocessMouseLeave(MouseEventArgs e);

        void PreprocessDragLeave(DragEventArgs e);

        void PostprocessDragLeave(DragEventArgs e);

        void PreprocessDragOver(DragEventArgs e);

        void PostprocessDragOver(DragEventArgs e);

        void PreprocessDragEnter(DragEventArgs e);

        void PostprocessDragEnter(DragEventArgs e);

        void PreprocessDrop(DragEventArgs e);

        void PostprocessDrop(DragEventArgs e);

        void PreprocessQueryContinueDrag(QueryContinueDragEventArgs e);

        void PostprocessQueryContinueDrag(QueryContinueDragEventArgs e);

        void PreprocessGiveFeedback(GiveFeedbackEventArgs e);

        void PostprocessGiveFeedback(GiveFeedbackEventArgs e);
    }

    public interface IMouseProcessor2 : IMouseProcessor
    {
        void PreprocessTouchDown(TouchEventArgs e);

        void PostprocessTouchDown(TouchEventArgs e);

        void PreprocessTouchUp(TouchEventArgs e);

        void PostprocessTouchUp(TouchEventArgs e);

        void PreprocessStylusSystemGesture(StylusSystemGestureEventArgs e);

        void PostprocessStylusSystemGesture(StylusSystemGestureEventArgs e);

        void PreprocessManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e);

        void PostprocessManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e);

        void PreprocessManipulationStarting(ManipulationStartingEventArgs e);

        void PostprocessManipulationStarting(ManipulationStartingEventArgs e);

        void PreprocessManipulationDelta(ManipulationDeltaEventArgs e);

        void PostprocessManipulationDelta(ManipulationDeltaEventArgs e);

        void PreprocessManipulationCompleted(ManipulationCompletedEventArgs e);

        void PostprocessManipulationCompleted(ManipulationCompletedEventArgs e);
    }
}