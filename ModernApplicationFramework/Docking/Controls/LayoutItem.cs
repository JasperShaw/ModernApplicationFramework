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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ModernApplicationFramework.Docking.Commands;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    public abstract class LayoutItem : FrameworkElement
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof (string), typeof (LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnTitleChanged));

        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register("IconSource", typeof (ImageSource), typeof (LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnIconSourceChanged));

        public static readonly DependencyProperty ContentIdProperty =
            DependencyProperty.Register("ContentId", typeof (string), typeof (LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnContentIdChanged));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof (bool), typeof (LayoutItem),
                new FrameworkPropertyMetadata(false,
                    OnIsSelectedChanged));

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof (bool), typeof (LayoutItem),
                new FrameworkPropertyMetadata(false,
                    OnIsActiveChanged));

        public static readonly DependencyProperty CanCloseProperty =
            DependencyProperty.Register("CanClose", typeof (bool), typeof (LayoutItem),
                new FrameworkPropertyMetadata(true,
                    OnCanCloseChanged));

        public static readonly DependencyProperty CanFloatProperty =
            DependencyProperty.Register("CanFloat", typeof (bool), typeof (LayoutItem),
                new FrameworkPropertyMetadata(true,
                    OnCanFloatChanged));

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof (ICommand), typeof (LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnCloseCommandChanged,
                    CoerceCloseCommandValue));

        public static readonly DependencyProperty FloatCommandProperty =
            DependencyProperty.Register("FloatCommand", typeof (ICommand), typeof (LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnFloatCommandChanged,
                    CoerceFloatCommandValue));

        public static readonly DependencyProperty DockAsDocumentCommandProperty =
            DependencyProperty.Register("DockAsDocumentCommand", typeof (ICommand), typeof (LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnDockAsDocumentCommandChanged,
                    CoerceDockAsDocumentCommandValue));

        public static readonly DependencyProperty CloseAllButThisCommandProperty =
            DependencyProperty.Register("CloseAllButThisCommand", typeof (ICommand), typeof (LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnCloseAllButThisCommandChanged,
                    CoerceCloseAllButThisCommandValue));

        public static readonly DependencyProperty ActivateCommandProperty =
            DependencyProperty.Register("ActivateCommand", typeof (ICommand), typeof (LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnActivateCommandChanged,
                    CoerceActivateCommandValue));

        public static readonly DependencyProperty NewVerticalTabGroupCommandProperty =
            DependencyProperty.Register("NewVerticalTabGroupCommand", typeof (ICommand), typeof (LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnNewVerticalTabGroupCommandChanged));

        public static readonly DependencyProperty NewHorizontalTabGroupCommandProperty =
            DependencyProperty.Register("NewHorizontalTabGroupCommand", typeof (ICommand), typeof (LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnNewHorizontalTabGroupCommandChanged));

        public static readonly DependencyProperty MoveToNextTabGroupCommandProperty =
            DependencyProperty.Register("MoveToNextTabGroupCommand", typeof (ICommand), typeof (LayoutItem),
                new FrameworkPropertyMetadata(null, OnMoveToNextTabGroupCommandChanged));

        public static readonly DependencyProperty MoveToPreviousTabGroupCommandProperty =
            DependencyProperty.Register("MoveToPreviousTabGroupCommand", typeof (ICommand), typeof (LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnMoveToPreviousTabGroupCommandChanged));

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof (ICommand), typeof (LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnAddCommandChanged,
                    CoerceAddCommandValue));


        public static readonly DependencyProperty CloseAllCommandProperty =
            DependencyProperty.Register("CloseAllCommand", typeof (ICommand), typeof (LayoutItem),
                new FrameworkPropertyMetadata(null,
                    OnCloseAllCommandChanged,
                    CoerceCloseAllCommandValue));


        public static readonly DependencyProperty IsFloatingProperty = DependencyProperty.Register("IsFloating",
            typeof (bool), typeof (LayoutItem),
            new UIPropertyMetadata(false));


        private readonly ReentrantFlag _isActiveReentrantFlag = new ReentrantFlag();
        private readonly ReentrantFlag _isSelectedReentrantFlag = new ReentrantFlag();
        private ICommand _defaultActivateCommand;
        private ICommand _defaultAddCommand;
        private ICommand _defaultCloseAllButThisCommand;
        private ICommand _defaultCloseAllCommand;
        private ICommand _defaultCloseCommand;
        private ICommand _defaultDockAsDocumentCommand;
        private ICommand _defaultFloatCommand;
        private ICommand _defaultMoveToNextTabGroupCommand;
        private ICommand _defaultMoveToPreviousTabGroupCommand;
        private ICommand _defaultNewHorizontalTabGroupCommand;
        private ICommand _defaultNewVerticalTabGroupCommand;
        private ContentPresenter _view;

        internal LayoutItem()
        {
        }

        static LayoutItem()
        {
            ToolTipProperty.OverrideMetadata(typeof (LayoutItem), new FrameworkPropertyMetadata(null, OnToolTipChanged));
            VisibilityProperty.OverrideMetadata(typeof (LayoutItem),
                new FrameworkPropertyMetadata(Visibility.Visible, OnVisibilityChanged));
        }

        public ContentPresenter View
        {
            get
            {
                if (_view == null)
                {
                    _view = new ContentPresenter();

                    _view.SetBinding(ContentPresenter.ContentProperty, new Binding("Content") {Source = LayoutElement});
                    _view.SetBinding(ContentPresenter.ContentTemplateProperty,
                        new Binding("LayoutItemTemplate") {Source = LayoutElement.Root.Manager});
                    _view.SetBinding(ContentPresenter.ContentTemplateSelectorProperty,
                        new Binding("LayoutItemTemplateSelector") {Source = LayoutElement.Root.Manager});
                    LayoutElement.Root.Manager.InternalAddLogicalChild(_view);
                }

                return _view;
            }
        }

        public ICommand ActivateCommand
        {
            get { return (ICommand) GetValue(ActivateCommandProperty); }
            set { SetValue(ActivateCommandProperty, value); }
        }

        public ICommand AddCommand
        {
            get { return (ICommand) GetValue(AddCommandProperty); }
            set { SetValue(AddCommandProperty, value); }
        }

        public bool CanClose
        {
            get { return (bool) GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }

        public bool CanFloat
        {
            get { return (bool) GetValue(CanFloatProperty); }
            set { SetValue(CanFloatProperty, value); }
        }

        public ICommand CloseAllButThisCommand
        {
            get { return (ICommand) GetValue(CloseAllButThisCommandProperty); }
            set { SetValue(CloseAllButThisCommandProperty, value); }
        }

        public ICommand CloseAllCommand
        {
            get { return (ICommand) GetValue(CloseAllCommandProperty); }
            set { SetValue(CloseAllCommandProperty, value); }
        }

        public ICommand CloseCommand
        {
            get { return (ICommand) GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        public string ContentId
        {
            get { return (string) GetValue(ContentIdProperty); }
            set { SetValue(ContentIdProperty, value); }
        }

        public ICommand DockAsDocumentCommand
        {
            get { return (ICommand) GetValue(DockAsDocumentCommandProperty); }
            set { SetValue(DockAsDocumentCommandProperty, value); }
        }

        public ICommand FloatCommand
        {
            get { return (ICommand) GetValue(FloatCommandProperty); }
            set { SetValue(FloatCommandProperty, value); }
        }

        public ImageSource IconSource
        {
            get { return (ImageSource) GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        public bool IsActive
        {
            get { return (bool) GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public bool IsFloating
        {
            [ExcludeFromCodeCoverage] get { return (bool) GetValue(IsFloatingProperty); }

            [ExcludeFromCodeCoverage] set { SetValue(IsFloatingProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public LayoutContent LayoutElement { get; private set; }
        public object Model { get; private set; }

        public ICommand MoveToNextTabGroupCommand
        {
            get { return (ICommand) GetValue(MoveToNextTabGroupCommandProperty); }
            set { SetValue(MoveToNextTabGroupCommandProperty, value); }
        }

        public ICommand MoveToPreviousTabGroupCommand
        {
            get { return (ICommand) GetValue(MoveToPreviousTabGroupCommandProperty); }
            set { SetValue(MoveToPreviousTabGroupCommandProperty, value); }
        }

        public ICommand NewHorizontalTabGroupCommand
        {
            get { return (ICommand) GetValue(NewHorizontalTabGroupCommandProperty); }
            set { SetValue(NewHorizontalTabGroupCommandProperty, value); }
        }

        public ICommand NewVerticalTabGroupCommand
        {
            get { return (ICommand) GetValue(NewVerticalTabGroupCommandProperty); }
            set { SetValue(NewVerticalTabGroupCommandProperty, value); }
        }

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        protected virtual void ClearDefaultBindings()
        {
            if (AddCommand == _defaultAddCommand)
                BindingOperations.ClearBinding(this, AddCommandProperty);
            if (CloseAllCommand == _defaultCloseAllCommand)
                BindingOperations.ClearBinding(this, CloseAllCommandProperty);
            if (CloseCommand == _defaultCloseCommand)
                BindingOperations.ClearBinding(this, CloseCommandProperty);
            if (FloatCommand == _defaultFloatCommand)
                BindingOperations.ClearBinding(this, FloatCommandProperty);
            if (DockAsDocumentCommand == _defaultDockAsDocumentCommand)
                BindingOperations.ClearBinding(this, DockAsDocumentCommandProperty);
            if (CloseAllButThisCommand == _defaultCloseAllButThisCommand)
                BindingOperations.ClearBinding(this, CloseAllButThisCommandProperty);
            if (ActivateCommand == _defaultActivateCommand)
                BindingOperations.ClearBinding(this, ActivateCommandProperty);
            if (NewVerticalTabGroupCommand == _defaultNewVerticalTabGroupCommand)
                BindingOperations.ClearBinding(this, NewVerticalTabGroupCommandProperty);
            if (NewHorizontalTabGroupCommand == _defaultNewHorizontalTabGroupCommand)
                BindingOperations.ClearBinding(this, NewHorizontalTabGroupCommandProperty);
            if (MoveToNextTabGroupCommand == _defaultMoveToNextTabGroupCommand)
                BindingOperations.ClearBinding(this, MoveToNextTabGroupCommandProperty);
            if (MoveToPreviousTabGroupCommand == _defaultMoveToPreviousTabGroupCommand)
                BindingOperations.ClearBinding(this, MoveToPreviousTabGroupCommandProperty);
        }

        protected abstract void Close();

        protected virtual void Float()
        {
        }

        protected virtual void InitDefaultCommands()
        {
            _defaultAddCommand = new RelayCommand(ExecuteAddCommand, CanExecuteAddCommand);
            _defaultCloseAllCommand = new RelayCommand(p => ExecuteCloseAllCommand(),
                p => CanExecuteCloseAllCommand());
            _defaultCloseCommand = new RelayCommand(ExecuteCloseCommand, CanExecuteCloseCommand);
            _defaultFloatCommand = new RelayCommand(ExecuteFloatCommand, CanExecuteFloatCommand);
            _defaultDockAsDocumentCommand = new RelayCommand(ExecuteDockAsDocumentCommand,
                CanExecuteDockAsDocumentCommand);
            _defaultCloseAllButThisCommand = new RelayCommand(ExecuteCloseAllButThisCommand,
                CanExecuteCloseAllButThisCommand);
            _defaultActivateCommand = new RelayCommand(ExecuteActivateCommand, CanExecuteActivateCommand);
            _defaultNewVerticalTabGroupCommand = new RelayCommand(ExecuteNewVerticalTabGroupCommand,
                CanExecuteNewVerticalTabGroupCommand);
            _defaultNewHorizontalTabGroupCommand = new RelayCommand(ExecuteNewHorizontalTabGroupCommand,
                CanExecuteNewHorizontalTabGroupCommand);
            _defaultMoveToNextTabGroupCommand = new RelayCommand(ExecuteMoveToNextTabGroupCommand,
                CanExecuteMoveToNextTabGroupCommand);
            _defaultMoveToPreviousTabGroupCommand = new RelayCommand(ExecuteMoveToPreviousTabGroupCommand,
                CanExecuteMoveToPreviousTabGroupCommand);
        }

        protected virtual void OnActivateCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnAddCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnCanCloseChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LayoutElement != null)
                LayoutElement.CanClose = (bool) e.NewValue;
        }

        protected virtual void OnCanFloatChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LayoutElement != null)
                LayoutElement.CanFloat = (bool) e.NewValue;
        }

        protected virtual void OnCloseAllButThisCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnCloseAllCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnCloseCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnContentIdChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LayoutElement != null)
                LayoutElement.ContentId = (string) e.NewValue;
        }

        protected virtual void OnDockAsDocumentCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnFloatCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnIconSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LayoutElement != null)
                LayoutElement.IconSource = IconSource;
        }

        protected virtual void OnIsActiveChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_isActiveReentrantFlag.CanEnter)
            {
                using (_isActiveReentrantFlag.Enter())
                {
                    if (LayoutElement != null)
                        LayoutElement.IsActive = (bool) e.NewValue;
                }
            }
        }

        protected virtual void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_isSelectedReentrantFlag.CanEnter)
            {
                using (_isSelectedReentrantFlag.Enter())
                {
                    if (LayoutElement != null)
                        LayoutElement.IsSelected = (bool) e.NewValue;
                }
            }
        }

        protected virtual void OnMoveToNextTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnMoveToPreviousTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnNewHorizontalTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnNewVerticalTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnTitleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LayoutElement != null)
                LayoutElement.Title = (string) e.NewValue;
        }

        protected virtual void OnVisibilityChanged()
        {
            if (LayoutElement != null &&
                Visibility == Visibility.Collapsed)
                LayoutElement.Close();
        }

        protected virtual void SetDefaultBindings()
        {
            if (CloseCommand == null)
                CloseCommand = _defaultCloseCommand;
            if (FloatCommand == null)
                FloatCommand = _defaultFloatCommand;
            if (DockAsDocumentCommand == null)
                DockAsDocumentCommand = _defaultDockAsDocumentCommand;
            if (CloseAllButThisCommand == null)
                CloseAllButThisCommand = _defaultCloseAllButThisCommand;
            if (ActivateCommand == null)
                ActivateCommand = _defaultActivateCommand;
            if (NewVerticalTabGroupCommand == null)
                NewVerticalTabGroupCommand = _defaultNewVerticalTabGroupCommand;
            if (NewHorizontalTabGroupCommand == null)
                NewHorizontalTabGroupCommand = _defaultNewHorizontalTabGroupCommand;
            if (MoveToNextTabGroupCommand == null)
                MoveToNextTabGroupCommand = _defaultMoveToNextTabGroupCommand;
            if (MoveToPreviousTabGroupCommand == null)
                MoveToPreviousTabGroupCommand = _defaultMoveToPreviousTabGroupCommand;
            if (AddCommand == null)
                AddCommand = _defaultAddCommand;
            if (CloseAllCommand == null)
                CloseAllCommand = _defaultCloseAllCommand;


            IsSelected = LayoutElement.IsSelected;
            IsActive = LayoutElement.IsActive;
        }

        internal void _ClearDefaultBindings()
        {
            ClearDefaultBindings();
        }

        internal void _SetDefaultBindings()
        {
            SetDefaultBindings();
        }

        internal virtual void Attach(LayoutContent model)
        {
            LayoutElement = model;
            Model = model.Content;

            InitDefaultCommands();

            LayoutElement.IsSelectedChanged += LayoutElement_IsSelectedChanged;
            LayoutElement.IsActiveChanged += LayoutElement_IsActiveChanged;

            DataContext = this;
            System.Diagnostics.Trace.WriteLine($"Attach({LayoutElement.Title})");
        }

        internal virtual void Detach()
        {
            System.Diagnostics.Trace.WriteLine($"Detach({LayoutElement.Title})");
            LayoutElement.IsSelectedChanged -= LayoutElement_IsSelectedChanged;
            LayoutElement.IsActiveChanged -= LayoutElement_IsActiveChanged;
            LayoutElement = null;
            Model = null;
        }

        private static object CoerceActivateCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceAddCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceCloseAllButThisCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceCloseAllCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceCloseCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceDockAsDocumentCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceFloatCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static void OnActivateCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnActivateCommandChanged(e);
        }

        private static void OnAddCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnAddCommandChanged(e);
        }

        private static void OnCanCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnCanCloseChanged(e);
        }

        private static void OnCanFloatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnCanFloatChanged(e);
        }

        private static void OnCloseAllButThisCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnCloseAllButThisCommandChanged(e);
        }

        private static void OnCloseAllCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnCloseAllCommandChanged(e);
        }

        private static void OnCloseCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnCloseCommandChanged(e);
        }

        private static void OnContentIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnContentIdChanged(e);
        }

        private static void OnDockAsDocumentCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnDockAsDocumentCommandChanged(e);
        }

        private static void OnFloatCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnFloatCommandChanged(e);
        }

        private static void OnIconSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnIconSourceChanged(e);
        }

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnIsActiveChanged(e);
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnIsSelectedChanged(e);
        }

        private static void OnMoveToNextTabGroupCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnMoveToNextTabGroupCommandChanged(e);
        }

        private static void OnMoveToPreviousTabGroupCommandChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnMoveToPreviousTabGroupCommandChanged(e);
        }

        private static void OnNewHorizontalTabGroupCommandChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnNewHorizontalTabGroupCommandChanged(e);
        }

        private static void OnNewVerticalTabGroupCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnNewVerticalTabGroupCommandChanged(e);
        }

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) d).OnTitleChanged(e);
        }

        private static void OnToolTipChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) s).OnToolTipChanged();
        }

        private static void OnVisibilityChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem) s).OnVisibilityChanged();
        }

        private bool CanExecuteActivateCommand(object parameter)
        {
            return LayoutElement != null;
        }

        private bool CanExecuteAddCommand(object parameter)
        {
            var root = LayoutElement?.Root;
            if (root == null)
                return false;

            if (!LayoutElement.Root.Manager.CanAdd)
            {
                return false;
            }

            return
                LayoutElement.Root.Manager.Layout.Descendents()
                    .OfType<LayoutContent>()
                    .Any(
                        d =>
                            !Equals(d, LayoutElement) &&
                            (d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow));
        }

        private bool CanExecuteCloseAllButThisCommand(object parameter)
        {
            var root = LayoutElement?.Root;
            if (root == null)
                return false;

            if (!LayoutElement.Root.Manager.CanCloseAllButThis)
                return false;

            return LayoutElement.Root.Manager.Layout.
                Descendents()
                .OfType<LayoutContent>()
                .Any(
                    d =>
                        !Equals(d, LayoutElement) &&
                        (d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow));
        }

        private bool CanExecuteCloseAllCommand()
        {
            var root = LayoutElement?.Root;
            if (root == null)
                return false;
            if (!LayoutElement.Root.Manager.CanCloseAll)
                return false;

            return
                LayoutElement.Root.Manager.Layout
                    .Descendents()
                    .OfType<LayoutContent>()
                    .Any(d => d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow);
        }

        private bool CanExecuteCloseCommand(object parameter)
        {
#if DEBUG
            if (LayoutElement != null)
                System.Diagnostics.Trace.WriteLine(
                    $"CanExecuteCloseCommand({LayoutElement.Title}) = {LayoutElement.CanClose}");
#endif
            return LayoutElement != null && LayoutElement.CanClose;
        }

        private bool CanExecuteDockAsDocumentCommand(object parameter)
        {
            return LayoutElement != null && LayoutElement.FindParent<LayoutDocumentPane>() == null;
        }

        private bool CanExecuteFloatCommand(object anchorable)
        {
            return LayoutElement != null && LayoutElement.CanFloat &&
                   LayoutElement.FindParent<LayoutFloatingWindow>() == null;
        }

        private bool CanExecuteMoveToNextTabGroupCommand(object parameter)
        {
            if (LayoutElement == null)
                return false;

            var parentDocumentGroup = LayoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = LayoutElement.Parent as LayoutDocumentPane;
            return (parentDocumentGroup != null &&
                    parentDocumentPane != null &&
                    parentDocumentGroup.ChildrenCount > 1 &&
                    parentDocumentGroup.IndexOfChild(parentDocumentPane) < parentDocumentGroup.ChildrenCount - 1 &&
                    parentDocumentGroup.Children[parentDocumentGroup.IndexOfChild(parentDocumentPane) + 1] is
                        LayoutDocumentPane);
        }

        private bool CanExecuteMoveToPreviousTabGroupCommand(object parameter)
        {
            if (LayoutElement == null)
                return false;
            var parentDocumentGroup = LayoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = LayoutElement.Parent as LayoutDocumentPane;
            return (parentDocumentGroup != null &&
                    parentDocumentPane != null &&
                    parentDocumentGroup.ChildrenCount > 1 &&
                    parentDocumentGroup.IndexOfChild(parentDocumentPane) > 0 &&
                    parentDocumentGroup.Children[parentDocumentGroup.IndexOfChild(parentDocumentPane) - 1] is
                        LayoutDocumentPane);
        }

        private bool CanExecuteNewHorizontalTabGroupCommand(object parameter)
        {
            if (LayoutElement == null)
                return false;
            var parentDocumentGroup = LayoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = LayoutElement.Parent as LayoutDocumentPane;
            return ((parentDocumentGroup == null ||
                     parentDocumentGroup.ChildrenCount == 1 ||
                     parentDocumentGroup.Root.Manager.AllowMixedOrientation ||
                     parentDocumentGroup.Orientation == Orientation.Vertical) &&
                    parentDocumentPane != null &&
                    parentDocumentPane.ChildrenCount > 1);
        }

        private bool CanExecuteNewVerticalTabGroupCommand(object parameter)
        {
            if (LayoutElement == null)
                return false;
            var parentDocumentGroup = LayoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = LayoutElement.Parent as LayoutDocumentPane;
            return ((parentDocumentGroup == null ||
                     parentDocumentGroup.ChildrenCount == 1 ||
                     parentDocumentGroup.Root.Manager.AllowMixedOrientation ||
                     parentDocumentGroup.Orientation == Orientation.Horizontal) &&
                    parentDocumentPane != null &&
                    parentDocumentPane.ChildrenCount > 1);
        }

        private void ExecuteActivateCommand(object parameter)
        {
            LayoutElement.Root.Manager._ExecuteContentActivateCommand(LayoutElement);
        }

        private void ExecuteAddCommand(object parameter)
        {
            LayoutElement.Root.Manager._ExecuteAddCommand(LayoutElement);
        }

        private void ExecuteCloseAllButThisCommand(object parameter)
        {
            LayoutElement.Root.Manager._ExecuteCloseAllButThisCommand(LayoutElement);
        }

        private void ExecuteCloseAllCommand()
        {
            LayoutElement.Root.Manager._ExecuteCloseAllCommand();
        }

        private void ExecuteCloseCommand(object parameter)
        {
            Close();
        }

        private void ExecuteDockAsDocumentCommand(object parameter)
        {
            LayoutElement.Root.Manager._ExecuteDockAsDocumentCommand(LayoutElement);
            IsFloating = false;
        }

        private void ExecuteFloatCommand(object parameter)
        {
            LayoutElement.Root.Manager._ExecuteFloatCommand(LayoutElement);
            IsFloating = true;
        }

        private void ExecuteMoveToNextTabGroupCommand(object parameter)
        {
            var layoutElement = LayoutElement;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;
            int indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
            var nextDocumentPane = parentDocumentGroup.Children[indexOfParentPane + 1] as LayoutDocumentPane;
            nextDocumentPane?.InsertChildAt(0, layoutElement);
            layoutElement.IsActive = true;
            layoutElement.Root.CollectGarbage();
        }

        private void ExecuteMoveToPreviousTabGroupCommand(object parameter)
        {
            var layoutElement = LayoutElement;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;
            int indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
            var nextDocumentPane = parentDocumentGroup.Children[indexOfParentPane - 1] as LayoutDocumentPane;
            nextDocumentPane?.InsertChildAt(0, layoutElement);
            layoutElement.IsActive = true;
            layoutElement.Root.CollectGarbage();
        }

        private void ExecuteNewHorizontalTabGroupCommand(object parameter)
        {
            var layoutElement = LayoutElement;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;

            if (parentDocumentGroup == null)
            {
                if (parentDocumentPane != null)
                {
                    var grandParent = parentDocumentPane.Parent;
                    parentDocumentGroup = new LayoutDocumentPaneGroup() {Orientation = Orientation.Vertical};
                    grandParent.ReplaceChild(parentDocumentPane, parentDocumentGroup);
                }
                parentDocumentGroup?.Children.Add(parentDocumentPane);
            }
            if (parentDocumentGroup != null)
            {
                parentDocumentGroup.Orientation = Orientation.Vertical;
                int indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
                parentDocumentGroup.InsertChildAt(indexOfParentPane + 1, new LayoutDocumentPane(layoutElement));
            }
            layoutElement.IsActive = true;
            layoutElement.Root.CollectGarbage();
        }

        private void ExecuteNewVerticalTabGroupCommand(object parameter)
        {
            var layoutElement = LayoutElement;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;

            if (parentDocumentGroup == null)
            {
                if (parentDocumentPane != null)
                {
                    var grandParent = parentDocumentPane.Parent;
                    parentDocumentGroup = new LayoutDocumentPaneGroup() {Orientation = Orientation.Horizontal};
                    grandParent.ReplaceChild(parentDocumentPane, parentDocumentGroup);
                }
                parentDocumentGroup?.Children.Add(parentDocumentPane);
            }
            if (parentDocumentGroup != null)
            {
                parentDocumentGroup.Orientation = Orientation.Horizontal;
                int indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
                parentDocumentGroup.InsertChildAt(indexOfParentPane + 1, new LayoutDocumentPane(layoutElement));
            }
            layoutElement.IsActive = true;
            layoutElement.Root.CollectGarbage();
        }

        private void LayoutElement_IsActiveChanged(object sender, EventArgs e)
        {
            if (!_isActiveReentrantFlag.CanEnter)
                return;
            using (_isActiveReentrantFlag.Enter())
            {
                BindingOperations.GetBinding(this, IsActiveProperty);
                IsActive = LayoutElement.IsActive;
                BindingOperations.GetBinding(this, IsActiveProperty);
            }
        }

        private void LayoutElement_IsSelectedChanged(object sender, EventArgs e)
        {
            if (!_isSelectedReentrantFlag.CanEnter)
                return;
            using (_isSelectedReentrantFlag.Enter())
            {
                IsSelected = LayoutElement.IsSelected;
            }
        }

        private void OnToolTipChanged()
        {
            if (LayoutElement != null)
                LayoutElement.ToolTip = ToolTip;
        }
    }
}