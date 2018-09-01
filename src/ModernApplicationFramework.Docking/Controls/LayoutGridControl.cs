/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Native;

namespace ModernApplicationFramework.Docking.Controls
{
    public abstract class LayoutGridControl<T> : Grid, ILayoutControl where T : class, ILayoutPanelElement
    {
        private readonly ReentrantFlag _fixingChildrenDockLengths = new ReentrantFlag();
        private readonly LayoutPositionableGroup<T> _model;
        private ChildrenTreeChange? _asyncRefreshCalled;
        private bool _initialized;
        private Vector _initialStartPoint;
        private Border _resizerGhost;
        private Window _resizerWindowHost;

        internal LayoutGridControl(LayoutPositionableGroup<T> model, Orientation orientation)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));

            FlowDirection = FlowDirection.LeftToRight;
        }

        public ILayoutElement Model => _model;
        public Orientation Orientation => ((ILayoutOrientableGroup) _model).Orientation;
        private bool AsyncRefreshCalled => _asyncRefreshCalled != null;

        protected void FixChildrenDockLengths()
        {
            using (_fixingChildrenDockLengths.Enter())
                OnFixChildrenDockLengths();
        }

        protected abstract void OnFixChildrenDockLengths();

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _model.ChildrenTreeChanged += (s, args) =>
            {
                if (_asyncRefreshCalled.HasValue &&
                    _asyncRefreshCalled.Value == args.Change)
                    return;
                _asyncRefreshCalled = args.Change;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _asyncRefreshCalled = null;
                    UpdateChildren();
                }), DispatcherPriority.Normal, null);
            };

            LayoutUpdated += OnLayoutUpdated;
        }

        private void AttachNewSplitters()
        {
            foreach (var splitter in Children.OfType<LayoutGridResizerControl>())
            {
                splitter.DragStarted += OnSplitterDragStarted;
                splitter.DragDelta += OnSplitterDragDelta;
                splitter.DragCompleted += OnSplitterDragCompleted;
            }
        }

        private void AttachPropertyChangeHandler()
        {
            foreach (var child in InternalChildren.OfType<ILayoutControl>())
            {
                child.Model.PropertyChanged += OnChildModelPropertyChanged;
            }
        }

        private void CreateSplitters()
        {
            for (int iChild = 1; iChild < Children.Count; iChild++)
            {
                var splitter = new LayoutGridResizerControl
                {
                    Cursor = Orientation == Orientation.Horizontal ? Cursors.SizeWE : Cursors.SizeNS
                };
                Children.Insert(iChild, splitter);
                iChild++;
            }
        }

        private void DetachOldSplitters()
        {
            foreach (var splitter in Children.OfType<LayoutGridResizerControl>())
            {
                splitter.DragStarted -= OnSplitterDragStarted;
                splitter.DragDelta -= OnSplitterDragDelta;
                splitter.DragCompleted -= OnSplitterDragCompleted;
            }
        }

        private void DetachPropertChangeHandler()
        {
            foreach (var child in InternalChildren.OfType<ILayoutControl>())
            {
                child.Model.PropertyChanged -= OnChildModelPropertyChanged;
            }
        }

        private FrameworkElement GetNextVisibleChild(int index)
        {
            for (int i = index + 1; i < InternalChildren.Count; i++)
            {
                if (InternalChildren[i] is LayoutGridResizerControl)
                    continue;

                if (Orientation == Orientation.Horizontal)
                {
                    if (ColumnDefinitions[i].Width.IsStar || ColumnDefinitions[i].Width.Value > 0)
                        return InternalChildren[i] as FrameworkElement;
                }
                else
                {
                    if (RowDefinitions[i].Height.IsStar || RowDefinitions[i].Height.Value > 0)
                        return InternalChildren[i] as FrameworkElement;
                }
            }

            return null;
        }

        private void HideResizerOverlayWindow()
        {
            if (_resizerWindowHost == null)
                return;
            _resizerWindowHost.Close();
            _resizerWindowHost = null;
        }

        private void OnChildModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (AsyncRefreshCalled)
                return;

            if (_fixingChildrenDockLengths.CanEnter && e.PropertyName == "DockWidth" &&
                Orientation == Orientation.Horizontal)
            {
                if (ColumnDefinitions.Count != InternalChildren.Count)
                    return;
                var changedElement = sender as ILayoutPositionableElement;
                var childFromModel =
                    InternalChildren.OfType<ILayoutControl>().First(ch => ch.Model == changedElement) as UIElement;
                int indexOfChild = InternalChildren.IndexOf(childFromModel);
                if (changedElement != null) ColumnDefinitions[indexOfChild].Width = changedElement.DockWidth;
            }
            else if (_fixingChildrenDockLengths.CanEnter && e.PropertyName == "DockHeight" &&
                     Orientation == Orientation.Vertical)
            {
                if (RowDefinitions.Count != InternalChildren.Count)
                    return;
                var changedElement = sender as ILayoutPositionableElement;
                var childFromModel =
                    InternalChildren.OfType<ILayoutControl>().First(ch => ch.Model == changedElement) as UIElement;
                int indexOfChild = InternalChildren.IndexOf(childFromModel);
                if (changedElement != null)
                    RowDefinitions[indexOfChild].Height = changedElement.DockHeight;
            }
            else if (e.PropertyName == "IsVisible")
            {
                UpdateRowColDefinitions();
            }
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            var modelWithActualSize = _model as ILayoutPositionableElementWithActualSize;
            modelWithActualSize.ActualWidth = ActualWidth;
            modelWithActualSize.ActualHeight = ActualHeight;

            if (_initialized)
                return;
            _initialized = true;
            UpdateChildren();
        }

        private void OnSplitterDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            LayoutGridResizerControl splitter = sender as LayoutGridResizerControl;
            var rootVisual = this.FindVisualTreeRoot() as Visual;

            if (rootVisual != null)
            {
                var trToWnd = TransformToAncestor(rootVisual);
                Vector transformedDelta = trToWnd.Transform(new Point(e.HorizontalChange, e.VerticalChange)) -
                                          trToWnd.Transform(new Point());
            }

            double delta;
            if (Orientation == Orientation.Horizontal)
                delta = Canvas.GetLeft(_resizerGhost) - _initialStartPoint.X;
            else
                delta = Canvas.GetTop(_resizerGhost) - _initialStartPoint.Y;

            int indexOfResizer = InternalChildren.IndexOf(splitter);

            var prevChild = InternalChildren[indexOfResizer - 1] as FrameworkElement;
            var nextChild = GetNextVisibleChild(indexOfResizer);

            var prevChildActualSize = prevChild.TransformActualSizeToAncestor();
            var nextChildActualSize = nextChild.TransformActualSizeToAncestor();

            var layoutControl = (ILayoutControl) prevChild;
            if (layoutControl != null)
            {
                var prevChildModel = (ILayoutPositionableElement) layoutControl.Model;
                var control = nextChild as ILayoutControl;
                if (control != null)
                {
                    var nextChildModel = (ILayoutPositionableElement) control.Model;

                    if (Orientation == Orientation.Horizontal)
                    {
                        //Trace.WriteLine(string.Format("PrevChild From {0}", prevChildModel.DockWidth));
                        prevChildModel.DockWidth = prevChildModel.DockWidth.IsStar
                            ? new GridLength(
                                prevChildModel.DockWidth.Value*(prevChildActualSize.Width + delta)/
                                prevChildActualSize.Width,
                                GridUnitType.Star)
                            : new GridLength(prevChildModel.DockWidth.Value + delta, GridUnitType.Pixel);
                        //Trace.WriteLine(string.Format("PrevChild To {0}", prevChildModel.DockWidth));

                        //Trace.WriteLine(string.Format("NextChild From {0}", nextChildModel.DockWidth));
                        nextChildModel.DockWidth = nextChildModel.DockWidth.IsStar
                            ? new GridLength(
                                nextChildModel.DockWidth.Value*(nextChildActualSize.Width - delta)/
                                nextChildActualSize.Width,
                                GridUnitType.Star)
                            : new GridLength(nextChildModel.DockWidth.Value - delta, GridUnitType.Pixel);
                        //Trace.WriteLine(string.Format("NextChild To {0}", nextChildModel.DockWidth));
                    }
                    else
                    {
                        //Trace.WriteLine(string.Format("PrevChild From {0}", prevChildModel.DockHeight));
                        prevChildModel.DockHeight = prevChildModel.DockHeight.IsStar
                            ? new GridLength(
                                prevChildModel.DockHeight.Value*(prevChildActualSize.Height + delta)/
                                prevChildActualSize.Height,
                                GridUnitType.Star)
                            : new GridLength(prevChildModel.DockHeight.Value + delta, GridUnitType.Pixel);
                        //Trace.WriteLine(string.Format("PrevChild To {0}", prevChildModel.DockHeight));

                        //Trace.WriteLine(string.Format("NextChild From {0}", nextChildModel.DockHeight));
                        nextChildModel.DockHeight = nextChildModel.DockHeight.IsStar
                            ? new GridLength(
                                nextChildModel.DockHeight.Value*(nextChildActualSize.Height - delta)/
                                nextChildActualSize.Height,
                                GridUnitType.Star)
                            : new GridLength(nextChildModel.DockHeight.Value - delta, GridUnitType.Pixel);
                        //Trace.WriteLine(string.Format("NextChild To {0}", nextChildModel.DockHeight));
                    }
                }
            }

            HideResizerOverlayWindow();
        }

        private void OnSplitterDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var rootVisual = this.FindVisualTreeRoot() as Visual;

            if (rootVisual == null)
                return;
            var trToWnd = TransformToAncestor(rootVisual);
            Vector transformedDelta = trToWnd.Transform(new Point(e.HorizontalChange, e.VerticalChange)) -
                                      trToWnd.Transform(new Point());

            if (Orientation == Orientation.Horizontal)
            {
                Canvas.SetLeft(_resizerGhost,
                    MathHelper.MinMax(_initialStartPoint.X + transformedDelta.X, 0.0,
                        _resizerWindowHost.Width - _resizerGhost.Width));
            }
            else
            {
                Canvas.SetTop(_resizerGhost,
                    MathHelper.MinMax(_initialStartPoint.Y + transformedDelta.Y, 0.0,
                        _resizerWindowHost.Height - _resizerGhost.Height));
            }
        }

        private void OnSplitterDragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            var resizer = sender as LayoutGridResizerControl;
            ShowResizerOverlayWindow(resizer);
        }

        private void ShowResizerOverlayWindow(LayoutGridResizerControl splitter)
        {
            _resizerGhost = new Border()
            {
                Background = splitter.BackgroundWhileDragging,
                Opacity = splitter.OpacityWhileDragging
            };

            int indexOfResizer = InternalChildren.IndexOf(splitter);

            var prevChild = InternalChildren[indexOfResizer - 1] as FrameworkElement;
            var nextChild = GetNextVisibleChild(indexOfResizer);

            var prevChildActualSize = prevChild.TransformActualSizeToAncestor();
            var nextChildActualSize = nextChild.TransformActualSizeToAncestor();

            var layoutControl = prevChild as ILayoutControl;
            if (layoutControl != null)
            {
                var prevChildModel = (ILayoutPositionableElement) layoutControl.Model;
                var control = nextChild as ILayoutControl;
                if (control != null)
                {
                    var nextChildModel = (ILayoutPositionableElement) control.Model;

                    Point ptTopLeftScreen = prevChild.PointToScreenDpiWithoutFlowDirection(new Point());

                    Size actualSize;

                    if (Orientation == Orientation.Horizontal)
                    {
                        actualSize = new Size(
                            prevChildActualSize.Width - prevChildModel.DockMinWidth + splitter.ActualWidth +
                            nextChildActualSize.Width -
                            nextChildModel.DockMinWidth,
                            nextChildActualSize.Height);

                        _resizerGhost.Width = splitter.ActualWidth;
                        _resizerGhost.Height = actualSize.Height;
                        ptTopLeftScreen.Offset(prevChildModel.DockMinWidth, 0.0);
                    }
                    else
                    {
                        actualSize = new Size(
                            prevChildActualSize.Width,
                            prevChildActualSize.Height - prevChildModel.DockMinHeight + splitter.ActualHeight +
                            nextChildActualSize.Height -
                            nextChildModel.DockMinHeight);

                        _resizerGhost.Height = splitter.ActualHeight;
                        _resizerGhost.Width = actualSize.Width;

                        ptTopLeftScreen.Offset(0.0, prevChildModel.DockMinHeight);
                    }

                    _initialStartPoint = splitter.PointToScreenDpiWithoutFlowDirection(new Point()) - ptTopLeftScreen;

                    if (Orientation == Orientation.Horizontal)
                    {
                        Canvas.SetLeft(_resizerGhost, _initialStartPoint.X);
                    }
                    else
                    {
                        Canvas.SetTop(_resizerGhost, _initialStartPoint.Y);
                    }

                    Canvas panelHostResizer = new Canvas()
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };

                    panelHostResizer.Children.Add(_resizerGhost);


                    _resizerWindowHost = new Window()
                    {
                        SizeToContent = SizeToContent.Manual,
                        ResizeMode = ResizeMode.NoResize,
                        WindowStyle = WindowStyle.None,
                        ShowInTaskbar = false,
                        AllowsTransparency = true,
                        Background = null,
                        Width = actualSize.Width,
                        Height = actualSize.Height,
                        Left = ptTopLeftScreen.X,
                        Top = ptTopLeftScreen.Y,
                        ShowActivated = false,
                        //Owner = Window.GetWindow(this),
                        Content = panelHostResizer
                    };
                }
            }
            _resizerWindowHost.Loaded += (s, e) => { _resizerWindowHost.SetParentToMainWindowOf(this); };
            _resizerWindowHost.Show();
        }

        private void UpdateChildren()
        {
            var alreadyContainedChildren = Children.OfType<ILayoutControl>().ToArray();

            DetachOldSplitters();
            DetachPropertChangeHandler();

            Children.Clear();
            ColumnDefinitions.Clear();
            RowDefinitions.Clear();

            var manager = _model?.Root?.Manager;
            if (manager == null)
                return;


            foreach (T child in _model.Children)
            {
                var foundContainedChild = alreadyContainedChildren.FirstOrDefault(chVm => chVm.Model == child);
                if (foundContainedChild != null)
                    Children.Add(foundContainedChild as UIElement);
                else
                {
                    var c = manager.CreateUIElementForModel(child);
                    Children.Add(c);
                }
            }

            CreateSplitters();

            UpdateRowColDefinitions();

            AttachNewSplitters();
            AttachPropertyChangeHandler();
        }

        private void UpdateRowColDefinitions()
        {
            var root = _model.Root;
            var manager = root?.Manager;
            if (manager == null)
                return;

            FixChildrenDockLengths();

            //Debug.Assert(InternalChildren.Count == _model.ChildrenCount + (_model.ChildrenCount - 1));

            RowDefinitions.Clear();
            ColumnDefinitions.Clear();
            if (Orientation == Orientation.Horizontal)
            {
                int iColumn = 0;
                int iChild = 0;
                for (int iChildModel = 0; iChildModel < _model.Children.Count; iChildModel++, iColumn++, iChild++)
                {
                    var childModel = _model.Children[iChildModel] as ILayoutPositionableElement;
                    ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width =
                            childModel != null && childModel.IsVisible
                                ? childModel.DockWidth
                                : new GridLength(0.0, GridUnitType.Pixel),
                        MinWidth = childModel != null && childModel.IsVisible ? childModel.DockMinWidth : 0.0
                    });
                    SetColumn(InternalChildren[iChild], iColumn);

                    //append column for splitter
                    if (iChild >= InternalChildren.Count - 1)
                        continue;
                    iChild++;
                    iColumn++;

                    bool nextChildModelVisibleExist = false;
                    for (int i = iChildModel + 1; i < _model.Children.Count; i++)
                    {
                        var nextChildModel = _model.Children[i] as ILayoutPositionableElement;
                        if (nextChildModel == null || !nextChildModel.IsVisible)
                            continue;
                        nextChildModelVisibleExist = true;
                        break;
                    }

                    ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width =
                            childModel != null && (childModel.IsVisible && nextChildModelVisibleExist)
                                ? new GridLength(manager.GridSplitterWidth)
                                : new GridLength(0.0, GridUnitType.Pixel)
                    });
                    SetColumn(InternalChildren[iChild], iColumn);
                }
            }
            else //if (_model.Orientation == Orientation.Vertical)
            {
                int iRow = 0;
                int iChild = 0;
                for (int iChildModel = 0; iChildModel < _model.Children.Count; iChildModel++, iRow++, iChild++)
                {
                    var childModel = _model.Children[iChildModel] as ILayoutPositionableElement;
                    RowDefinitions.Add(new RowDefinition()
                    {
                        Height =
                            childModel != null && childModel.IsVisible
                                ? childModel.DockHeight
                                : new GridLength(0.0, GridUnitType.Pixel),
                        MinHeight = childModel != null && childModel.IsVisible ? childModel.DockMinHeight : 0.0
                    });
                    SetRow(InternalChildren[iChild], iRow);

                    //if (RowDefinitions.Last().Height.Value == 0.0)
                    //    System.Diagnostics.Debugger.Break();

                    //append row for splitter (if necessary)
                    if (iChild < InternalChildren.Count - 1)
                    {
                        iChild++;
                        iRow++;

                        bool nextChildModelVisibleExist = false;
                        for (int i = iChildModel + 1; i < _model.Children.Count; i++)
                        {
                            var nextChildModel = _model.Children[i] as ILayoutPositionableElement;
                            if (nextChildModel == null || !nextChildModel.IsVisible)
                                continue;
                            nextChildModelVisibleExist = true;
                            break;
                        }

                        RowDefinitions.Add(new RowDefinition()
                        {
                            Height =
                                childModel != null && (childModel.IsVisible && nextChildModelVisibleExist)
                                    ? new GridLength(manager.GridSplitterHeight)
                                    : new GridLength(0.0, GridUnitType.Pixel)
                        });
                        //if (RowDefinitions.Last().Height.Value == 0.0)
                        //    System.Diagnostics.Debugger.Break();
                        SetRow(InternalChildren[iChild], iRow);
                    }
                }
            }
        }
    }
}