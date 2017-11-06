/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Controls.InfoBar;
using ModernApplicationFramework.Core.MenuModeHelper;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    public class LayoutAnchorableControl : Control, IInfoBarUiEvents
    {
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof (LayoutAnchorable), typeof (LayoutAnchorableControl),
                new FrameworkPropertyMetadata(null, OnModelChanged));

        private static readonly DependencyPropertyKey LayoutItemPropertyKey
            = DependencyProperty.RegisterReadOnly("LayoutItem", typeof (LayoutItem), typeof (LayoutAnchorableControl),
                new FrameworkPropertyMetadata((LayoutItem) null));

        public static readonly DependencyProperty LayoutItemProperty
            = LayoutItemPropertyKey.DependencyProperty;

        private AdornmentHostingPanel _adornmentHost;
        


        private AdornmentHostingPanel AdornmentHost
        {
            get
            {
                if (_adornmentHost == null)
                {
                    _adornmentHost = new AdornmentHostingPanel();
                    Grid.SetRow(_adornmentHost, 0);
                    Grid.SetColumn(_adornmentHost, 0);
                    Grid.SetColumnSpan(_adornmentHost, 3);
                    HostingPanel.Children.Add(_adornmentHost);
                }
                return _adornmentHost;
            }
        }




        private FrameworkElement _contentControl;
        public FrameworkElement Content
        {
            get => _contentControl;
            set
            {
                if (value == null)
                {
                    _contentControl = null;
                }
                else
                {
                    if (value is ContentHostingPanel)
                        _contentControl = value;
                    else
                        HostingPanel.Content = value;
                }
            }
        }

        internal FrameworkElement InnerContent => Content;


        private ContentHostingPanel HostingPanel
        {
            get
            {
                if (_contentControl == null)
                {
                    _contentControl = new ContentHostingPanel();
                    //this.ConnectContentEvents();
                }
                return _contentControl as ContentHostingPanel;
            }
        }




        static LayoutAnchorableControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutAnchorableControl),
                new FrameworkPropertyMetadata(typeof (LayoutAnchorableControl)));
            FocusableProperty.OverrideMetadata(typeof (LayoutAnchorableControl), new FrameworkPropertyMetadata(false));
        }

        public LayoutItem LayoutItem => (LayoutItem) GetValue(LayoutItemProperty);

        public LayoutAnchorable Model
        {
            get => (LayoutAnchorable) GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (Model != null)
                Model.IsActive = true;

            base.OnGotKeyboardFocus(e);
        }

        protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                ((LayoutContent)e.OldValue).PropertyChanged -= Model_PropertyChanged;
            }

            if (Model != null)
            {
                Model.PropertyChanged += Model_PropertyChanged;


                var model = Model.Root.Manager.GetLayoutItemFromModel(Model);
                Model.Root.Manager.InternalRemoveLogicalChild(model.View);

                Content = model.View;




                var i = AdornmentHost;
                var ih = i.InfoBarHost;



                var infoBarTextSpanArray = new[]
                {
                    new InfoBarTextSpan("Test Text "),
                    new InfoBarHyperlink("www.google.de")
                };

                var imodel = new InfoBarModel(infoBarTextSpanArray);

                var ui = ih.CreateInfoBar(this, imodel);

                ih.AddInfoBar(ui);




                //SetLayoutItem(Model.Root.Manager.GetLayoutItemFromModel(Model));
            }
            else
                // SetLayoutItem(null);
                Content = null;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsEnabled")
                return;
            if (Model == null)
                return;
            IsEnabled = Model.IsEnabled;
            if (!IsEnabled && Model.IsActive)
            {
                (Model.Parent as LayoutAnchorablePane)?.SetNextSelectedIndex();
            }
        }

        protected void SetLayoutItem(LayoutItem value)
        {
            SetValue(LayoutItemPropertyKey, value);
        }

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutAnchorableControl) d).OnModelChanged(e);
        }




        private class AdornmentHostingPanel : StackPanel
        {
            private InfoBarHostControl _infoBarHost;

            public AdornmentHostingPanel()
            {
                SetResourceReference(BackgroundProperty, EnvironmentColors.CommandBarMenuBackgroundGradientBegin);
                Focusable = false;
                KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetControlTabNavigation(this, KeyboardNavigationMode.Cycle);
            }

            public InfoBarHostControl InfoBarHost
            {
                get
                {
                    if (_infoBarHost != null)
                        return _infoBarHost;
                    _infoBarHost = new InfoBarHostControl();
                    Children.Add(_infoBarHost);
                    CommandBarNavigationHelper.SetCommandFocusMode(_infoBarHost, CommandBarNavigationHelper.CommandFocusMode.Container);
                    return _infoBarHost;
                }
            }
        }


        private class ContentHostingPanel : Grid
        {
            private FrameworkElement _content;

            public ContentHostingPanel()
            {
                RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(0.0, GridUnitType.Auto)
                });
                RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(1.0, GridUnitType.Star)
                });
                RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(0.0, GridUnitType.Auto)
                });
                ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(0.0, GridUnitType.Auto)
                });
                ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(1.0, GridUnitType.Star)
                });
                ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(0.0, GridUnitType.Auto)
                });
            }

            public FrameworkElement Content
            {
                get => _content;
                set
                {
                    if (_content != null)
                        Children.Remove(_content);
                    _content = null;
                    if (value == null)
                        return;
                    if (value.Parent is Panel parent)
                        parent.Children.Remove(value);
                    _content = value;
                    SetRow(_content, 1);
                    SetColumn(_content, 1);
                    Children.Add(_content);
                }
            }
        }

        public void OnClosed(IInfoBarUiElement infoBarUiElement)
        {
            
        }

        public void OnActionItemClicked(IInfoBarUiElement infoBarUiElement, IInfoBarActionItem actionItem)
        {
        }
    }
}