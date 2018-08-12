using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    internal class WpfMouseProcessor : IDisposable
    {
        protected IList<IMouseProcessor> _mouseProcessors;
        protected UIElement Element;
        protected GuardedOperations GuardedOperations;
        protected bool IsDisposed;
        protected FrameworkElement ManipulationElement;

        public WpfMouseProcessor(UIElement element, IList<IMouseProcessor> mouseProcessors,
            GuardedOperations guardedOperations, FrameworkElement manipulationElement = null)
            : this(element, guardedOperations, manipulationElement)
        {
            _mouseProcessors = mouseProcessors;
        }

        protected WpfMouseProcessor(UIElement element, GuardedOperations guardedOperations,
            FrameworkElement manipulationElement)
        {
            Element = element;
            GuardedOperations = guardedOperations;
            if (manipulationElement != null)
            {
                ManipulationElement = manipulationElement;
                ManipulationElement.IsManipulationEnabled = true;
                ManipulationElement.AddHandler(UIElement.ManipulationInertiaStartingEvent,
                    new EventHandler<ManipulationInertiaStartingEventArgs>(UIElement_ManipulationInertiaStarting),
                    true);
                ManipulationElement.AddHandler(UIElement.ManipulationStartingEvent,
                    new EventHandler<ManipulationStartingEventArgs>(UIElement_ManipulationStarting), true);
                ManipulationElement.AddHandler(UIElement.ManipulationCompletedEvent,
                    new EventHandler<ManipulationCompletedEventArgs>(UIElement_ManipulationCompleted), true);
                ManipulationElement.AddHandler(UIElement.ManipulationDeltaEvent,
                    new EventHandler<ManipulationDeltaEventArgs>(UIElement_ManipulationDelta), true);
                ManipulationElement.AddHandler(UIElement.TouchDownEvent,
                    new EventHandler<TouchEventArgs>(UIElement_TouchDown), true);
                ManipulationElement.AddHandler(UIElement.TouchUpEvent,
                    new EventHandler<TouchEventArgs>(UIElement_TouchUp), true);
                ManipulationElement.AddHandler(UIElement.StylusSystemGestureEvent,
                    new StylusSystemGestureEventHandler(UIElement_StylusSystemGesture), true);
            }

            Element.AddHandler(UIElement.MouseLeftButtonDownEvent,
                new MouseButtonEventHandler(UIElement_MouseLeftButtonDown), true);
            Element.AddHandler(UIElement.MouseLeftButtonUpEvent,
                new MouseButtonEventHandler(UIElement_MouseLeftButtonUp), true);
            Element.AddHandler(UIElement.MouseRightButtonDownEvent,
                new MouseButtonEventHandler(UIElement_MouseRightButtonDown), true);
            Element.AddHandler(UIElement.MouseRightButtonUpEvent,
                new MouseButtonEventHandler(UIElement_MouseRightButtonUp), true);
            Element.AddHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(UIElement_MouseDown), true);
            Element.AddHandler(UIElement.MouseUpEvent, new MouseButtonEventHandler(UIElement_MouseUp), true);
            Element.AddHandler(UIElement.MouseMoveEvent, new MouseEventHandler(UIElement_MouseMove), true);
            Element.AddHandler(UIElement.MouseWheelEvent, new MouseWheelEventHandler(UIElement_MouseWheel), true);
            Element.AddHandler(UIElement.MouseEnterEvent, new MouseEventHandler(UIElement_MouseEnter), true);
            Element.AddHandler(UIElement.MouseLeaveEvent, new MouseEventHandler(UIElement_MouseLeave), true);
            Element.AddHandler(UIElement.DragEnterEvent, new DragEventHandler(UIElement_DragEnter), true);
            Element.AddHandler(UIElement.DragLeaveEvent, new DragEventHandler(UIElement_DragLeave), true);
            Element.AddHandler(UIElement.DragOverEvent, new DragEventHandler(UIElement_DragOver), true);
            Element.AddHandler(UIElement.DropEvent, new DragEventHandler(UIElement_Drop), true);
            Element.AddHandler(UIElement.GiveFeedbackEvent, new GiveFeedbackEventHandler(UIElement_GiveFeedback), true);
            Element.AddHandler(UIElement.QueryContinueDragEvent,
                new QueryContinueDragEventHandler(UIElement_QueryContinueDrag), true);
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            GC.SuppressFinalize(this);
            if (ManipulationElement != null)
            {
                ManipulationElement.ManipulationInertiaStarting -= UIElement_ManipulationInertiaStarting;
                ManipulationElement.ManipulationStarting -= UIElement_ManipulationStarting;
                ManipulationElement.ManipulationCompleted -= UIElement_ManipulationCompleted;
                ManipulationElement.ManipulationDelta -= UIElement_ManipulationDelta;
                ManipulationElement.TouchDown -= UIElement_TouchDown;
                ManipulationElement.TouchUp -= UIElement_TouchUp;
                ManipulationElement.StylusSystemGesture -= UIElement_StylusSystemGesture;
            }

            Element.MouseLeftButtonDown -= UIElement_MouseLeftButtonDown;
            Element.MouseLeftButtonUp -= UIElement_MouseLeftButtonUp;
            Element.MouseRightButtonDown -= UIElement_MouseRightButtonDown;
            Element.MouseRightButtonUp -= UIElement_MouseRightButtonUp;
            Element.MouseDown -= UIElement_MouseDown;
            Element.MouseUp -= UIElement_MouseUp;
            Element.MouseMove -= UIElement_MouseMove;
            Element.MouseWheel -= UIElement_MouseWheel;
            Element.MouseEnter -= UIElement_MouseEnter;
            Element.MouseLeave -= UIElement_MouseLeave;
            Element.DragEnter -= UIElement_DragEnter;
            Element.DragLeave -= UIElement_DragLeave;
            Element.DragOver -= UIElement_DragOver;
            Element.Drop -= UIElement_Drop;
            Element.GiveFeedback -= UIElement_GiveFeedback;
            Element.QueryContinueDrag -= UIElement_QueryContinueDrag;
        }

        public void UIElement_DragEnter(object sender, DragEventArgs e)
        {
            MouseProcessorHandler(e, p => p.PreprocessDragEnter(e), () => DefaultDragEnterHandler(sender, e),
                p => p.PostprocessDragEnter(e));
        }

        public void UIElement_DragLeave(object sender, DragEventArgs e)
        {
            MouseProcessorHandler(e, p => p.PreprocessDragLeave(e), () => DefaultDragLeaveHandler(sender, e),
                p => p.PostprocessDragLeave(e));
        }

        public void UIElement_DragOver(object sender, DragEventArgs e)
        {
            MouseProcessorHandler(e, p => p.PreprocessDragOver(e), () => DefaultDragOverHandler(sender, e),
                p => p.PostprocessDragOver(e));
        }

        public void UIElement_Drop(object sender, DragEventArgs e)
        {
            MouseProcessorHandler(e, p => p.PreprocessDrop(e), () => DefaultDropHandler(sender, e),
                p => p.PostprocessDrop(e));
        }

        public void UIElement_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            MouseProcessor2Handler(e, p => p.PreprocessGiveFeedback(e), () => DefaultGiveFeedbackHandler(sender, e),
                p => p.PostprocessGiveFeedback(e));
        }

        public void UIElement_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            MouseProcessor2Handler(e, p => p.PreprocessManipulationCompleted(e),
                () => DefaultManipulationCompletedHandler(sender, e), p => p.PostprocessManipulationCompleted(e));
        }

        public void UIElement_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            MouseProcessor2Handler(e, p => p.PreprocessManipulationDelta(e),
                () => DefaultManipulationDeltaHandler(sender, e), p => p.PostprocessManipulationDelta(e));
        }

        public void UIElement_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            MouseProcessor2Handler(e, p => p.PreprocessManipulationInertiaStarting(e),
                () => DefaultManipulationInertiaStartingHandler(sender, e),
                p => p.PostprocessManipulationInertiaStarting(e));
        }

        public void UIElement_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            MouseProcessor2Handler(e, p => p.PreprocessManipulationStarting(e),
                () => DefaultManipulationStartingHandler(sender, e), p => p.PostprocessManipulationStarting(e));
        }

        public void UIElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var flag = false;
            if (e.ChangedButton != MouseButton.Left && e.ChangedButton != MouseButton.Right)
                flag = TakeFocusFromMouseEvent(e);
            MouseProcessorHandler(e, p => p.PreprocessMouseDown(e), () => DefaultMouseDownHandler(sender, e),
                p => p.PostprocessMouseDown(e));
            e.Handled |= flag;
        }

        public void UIElement_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseProcessorHandler(e, p => p.PreprocessMouseEnter(e), () => DefaultMouseEnterHandler(sender, e),
                p => p.PostprocessMouseEnter(e));
        }

        public void UIElement_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseProcessorHandler(e, p => p.PreprocessMouseLeave(e), () => DefaultMouseLeaveHandler(sender, e),
                p => p.PostprocessMouseLeave(e));
        }

        public void UIElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var focusFromMouseEvent = TakeFocusFromMouseEvent(e);
            MouseProcessorHandler(e, p => p.PreprocessMouseLeftButtonDown(e),
                () => DefaultMouseLeftButtonDownHandler(sender, e), p => p.PostprocessMouseLeftButtonDown(e));
            e.Handled |= focusFromMouseEvent;
        }

        public void UIElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            foreach (var mouseProcessor1 in _mouseProcessors)
            {
                var mouseProcessor = mouseProcessor1;
                if (!e.Handled)
                    GuardedOperations.CallExtensionPoint(() => mouseProcessor.PreprocessMouseLeftButtonUp(e));
                else
                    break;
            }

            if (!e.Handled)
                DefaultMouseLeftButtonUpHandler(sender, e);
            DefaultMouseLeftButtonUpPostprocessor(e);
            foreach (var mouseProcessor1 in _mouseProcessors)
            {
                var mouseProcessor = mouseProcessor1;
                GuardedOperations.CallExtensionPoint(() => mouseProcessor.PostprocessMouseLeftButtonUp(e));
            }
        }

        public void UIElement_MouseMove(object sender, MouseEventArgs e)
        {
            MouseProcessorHandler(e, p => p.PreprocessMouseMove(e), () => DefaultMouseMoveHandler(sender, e),
                p => p.PostprocessMouseMove(e));
        }

        public void UIElement_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var focusFromMouseEvent = TakeFocusFromMouseEvent(e);
            MouseProcessorHandler(e, p => p.PreprocessMouseRightButtonDown(e),
                () => DefaultMouseRightButtonDownHandler(sender, e), p => p.PostprocessMouseRightButtonDown(e));
            e.Handled |= focusFromMouseEvent;
        }

        public void UIElement_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseProcessorHandler(e, p => p.PreprocessMouseRightButtonUp(e),
                () => DefaultMouseRightButtonUpHandler(sender, e), p => p.PostprocessMouseRightButtonUp(e));
        }

        public void UIElement_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MouseProcessorHandler(e, p => p.PreprocessMouseUp(e), () => DefaultMouseUpHandler(sender, e),
                p => p.PostprocessMouseUp(e));
        }

        public void UIElement_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            MouseProcessorHandler(e, p => p.PreprocessMouseWheel(e), () => DefaultMouseWheelHandler(sender, e),
                p => p.PostprocessMouseWheel(e));
        }

        public void UIElement_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            MouseProcessorHandler(e, p => p.PreprocessQueryContinueDrag(e),
                () => DefaultQueryContinueDragHandler(sender, e), p => p.PostprocessQueryContinueDrag(e));
        }

        public void UIElement_StylusSystemGesture(object sender, StylusSystemGestureEventArgs e)
        {
            MouseProcessor2Handler(e, p => p.PreprocessStylusSystemGesture(e),
                () => DefaultStylusSystemGestureHandler(sender, e), p => p.PostprocessStylusSystemGesture(e));
        }

        public void UIElement_TouchDown(object sender, TouchEventArgs e)
        {
            MouseProcessor2Handler(e, p => p.PreprocessTouchDown(e), () => DefaultTouchDownHandler(sender, e),
                p => p.PostprocessTouchDown(e));
        }

        public void UIElement_TouchUp(object sender, TouchEventArgs e)
        {
            MouseProcessor2Handler(e, p => p.PreprocessTouchUp(e), () => DefaultTouchDownHandler(sender, e),
                p => p.PostprocessTouchUp(e));
        }

        protected virtual void DefaultDragEnterHandler(object sender, DragEventArgs e)
        {
        }

        protected virtual void DefaultDragLeaveHandler(object sender, DragEventArgs e)
        {
        }

        protected virtual void DefaultDragOverHandler(object sender, DragEventArgs e)
        {
        }

        protected virtual void DefaultDropHandler(object sender, DragEventArgs e)
        {
        }

        protected virtual void DefaultGiveFeedbackHandler(object sender, GiveFeedbackEventArgs e)
        {
        }

        protected virtual void DefaultManipulationCompletedHandler(object sender, ManipulationCompletedEventArgs e)
        {
        }

        protected virtual void DefaultManipulationDeltaHandler(object sender, ManipulationDeltaEventArgs e)
        {
        }

        protected virtual void DefaultManipulationInertiaStartingHandler(object sender,
            ManipulationInertiaStartingEventArgs e)
        {
        }

        protected virtual void DefaultManipulationStartingHandler(object sender, ManipulationStartingEventArgs e)
        {
        }

        protected virtual void DefaultMouseDownHandler(object sender, MouseButtonEventArgs e)
        {
        }

        protected virtual void DefaultMouseEnterHandler(object sender, MouseEventArgs e)
        {
        }

        protected virtual void DefaultMouseLeaveHandler(object sender, MouseEventArgs e)
        {
        }

        protected virtual void DefaultMouseLeftButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
        }

        protected virtual void DefaultMouseLeftButtonUpHandler(object sender, MouseButtonEventArgs e)
        {
        }

        protected virtual void DefaultMouseLeftButtonUpPostprocessor(MouseButtonEventArgs e)
        {
        }

        protected virtual void DefaultMouseMoveHandler(object sender, MouseEventArgs e)
        {
        }

        protected virtual void DefaultMouseRightButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
        }

        protected virtual void DefaultMouseRightButtonUpHandler(object sender, MouseButtonEventArgs e)
        {
        }

        protected virtual void DefaultMouseUpHandler(object sender, MouseButtonEventArgs e)
        {
        }

        protected virtual void DefaultMouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
        }

        protected virtual void DefaultQueryContinueDragHandler(object sender, QueryContinueDragEventArgs e)
        {
        }

        protected virtual void DefaultStylusSystemGestureHandler(object sender, StylusSystemGestureEventArgs e)
        {
        }

        protected virtual void DefaultTouchDownHandler(object sender, TouchEventArgs e)
        {
        }

        protected virtual void DefaultTouchUpHandler(object sender, TouchEventArgs e)
        {
        }

        private void MouseProcessor2Handler(RoutedEventArgs e, Action<IMouseProcessor2> preprocess,
            Action defaultAction, Action<IMouseProcessor2> postprocess)
        {
            foreach (var mouseProcessor in _mouseProcessors)
                if (!e.Handled)
                {
                    if (mouseProcessor is IMouseProcessor2 mouseProcessor2)
                        GuardedOperations.CallExtensionPoint(() => preprocess(mouseProcessor2));
                }
                else
                {
                    break;
                }

            if (!e.Handled)
                defaultAction();
            foreach (var mouseProcessor in _mouseProcessors)
                if (mouseProcessor is IMouseProcessor2 mouseProcessor2)
                    GuardedOperations.CallExtensionPoint(() => postprocess(mouseProcessor2));
        }

        private void MouseProcessorHandler(RoutedEventArgs e, Action<IMouseProcessor> preprocess, Action defaultAction,
            Action<IMouseProcessor> postprocess)
        {
            foreach (var mouseProcessor1 in _mouseProcessors)
            {
                var mouseProcessor = mouseProcessor1;
                if (!e.Handled)
                    GuardedOperations.CallExtensionPoint(() => preprocess(mouseProcessor));
                else
                    break;
            }

            if (!e.Handled)
                defaultAction();
            foreach (var mouseProcessor1 in _mouseProcessors)
            {
                var mouseProcessor = mouseProcessor1;
                GuardedOperations.CallExtensionPoint(() => postprocess(mouseProcessor));
            }
        }

        private bool TakeFocusFromMouseEvent(MouseEventArgs e)
        {
            if (!Element.Focusable || (e.Handled || Element.IsKeyboardFocused) &&
                (!e.Handled || Element.IsKeyboardFocusWithin))
                return false;
            Keyboard.Focus(Element);
            return true;
        }
    }
}