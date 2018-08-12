using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class DragDropHelper : MouseProcessorBase
    {
        private readonly ITextViewMargin _margin;
        private readonly IDragDropMouseProcessor _processor;
        private readonly ITextView _wpfTextView;

        public DragDropHelper(ITextViewMargin margin, ITextView wpfTextView, IDragDropMouseProcessor processor)
        {
            _margin = margin;
            _wpfTextView = wpfTextView;
            _processor = processor;
        }

        public override void PostprocessMouseLeave(MouseEventArgs e)
        {
            _processor.DoPostprocessMouseLeave(e);
        }

        public override void PostprocessMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _processor.DoPostprocessMouseLeftButtonUp(e, GetClickedPoint(e));
        }

        public override void PreprocessDragEnter(DragEventArgs e)
        {
            _processor.DoPreprocessDragEnter(e, GetClickedPoint(e));
        }

        public override void PreprocessDragLeave(DragEventArgs e)
        {
            _processor.DoPreprocessDragLeave(e);
        }

        public override void PreprocessDragOver(DragEventArgs e)
        {
            _processor.DoPreprocessDragOver(e, GetClickedPoint(e));
        }

        public override void PreprocessDrop(DragEventArgs e)
        {
            _processor.DoPreprocessDrop(e, GetClickedPoint(e));
        }

        public override void PreprocessMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _processor.DoPreprocessMouseLeftButtonDown(e, GetClickedPoint(e));
        }

        public override void PreprocessMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _processor.DoPreprocessMouseLeftButtonUp(e, GetClickedPoint(e));
        }

        public override void PreprocessMouseMove(MouseEventArgs e)
        {
            _processor.DoPreprocessMouseMove(e, GetClickedPoint(e));
        }

        public override void PreprocessQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            _processor.DoPreprocessQueryContinueDrag(e);
        }

        private Point GetClickedPoint(MouseEventArgs e)
        {
            var position = e.GetPosition(_margin.VisualElement);
            position.X = 0.0;
            position.Y += _wpfTextView.ViewportTop;
            return position;
        }

        private Point GetClickedPoint(DragEventArgs e)
        {
            var position = e.GetPosition(_margin.VisualElement);
            position.X = 0.0;
            position.Y += _wpfTextView.ViewportTop;
            return position;
        }
    }
}