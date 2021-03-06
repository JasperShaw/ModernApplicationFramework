﻿using System.Windows;
using System.Windows.Input;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    public interface IDragDropMouseProcessor
    {
        void DoPostprocessMouseLeave(MouseEventArgs e);

        void DoPostprocessMouseLeftButtonUp(MouseButtonEventArgs e, Point position);

        void DoPreprocessDragEnter(DragEventArgs e, Point position);

        void DoPreprocessDragLeave(DragEventArgs e);

        void DoPreprocessDragOver(DragEventArgs e, Point position);

        void DoPreprocessDrop(DragEventArgs e, Point position);
        void DoPreprocessMouseLeftButtonDown(MouseButtonEventArgs e, Point position);

        void DoPreprocessMouseLeftButtonUp(MouseButtonEventArgs e, Point position);

        void DoPreprocessMouseMove(MouseEventArgs e, Point position);

        void DoPreprocessQueryContinueDrag(QueryContinueDragEventArgs e);
    }
}