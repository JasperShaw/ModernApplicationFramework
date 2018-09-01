/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using JetBrains.Annotations;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    public class LayoutDocumentControl : ViewPresenter, INotifyPropertyChanged
    {
        private static readonly DependencyPropertyKey LayoutItemPropertyKey
            = DependencyProperty.RegisterReadOnly("LayoutItem", typeof (LayoutItem), typeof (LayoutDocumentControl),
                new FrameworkPropertyMetadata((LayoutItem) null));

        public static readonly DependencyProperty LayoutItemProperty
            = LayoutItemPropertyKey.DependencyProperty;

        static LayoutDocumentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutDocumentControl),
                new FrameworkPropertyMetadata(typeof (LayoutDocumentControl)));
            FocusableProperty.OverrideMetadata(typeof (LayoutDocumentControl), new FrameworkPropertyMetadata(false));
        }

        public override LayoutItem LayoutItem => (LayoutItem) GetValue(LayoutItemProperty);

        protected override void OnModelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                ((LayoutContent)e.OldValue).PropertyChanged -= Model_PropertyChanged;
            }

            if (Model != null)
            {
                Model.PropertyChanged += Model_PropertyChanged;
                var model = Model.Root.Manager.GetLayoutItemFromModel(Model);
                Model.Root.Manager.RemoveChild(model.View);
                SetLayoutItem(model);
                model.Content = model.View;
                Content = model.Content;
            }
            else
            {
                SetLayoutItem(null);
                Content = null;  
            }
                
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsEnabled")
                return;
            if (Model == null)
                return;
            IsEnabled = Model.IsEnabled;
            if (!IsEnabled && Model.IsActive)
            {
                (Model.Parent as LayoutDocumentPane)?.SetNextSelectedIndex();
            }
        }

        protected override void OnPreviewGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (Model != null)
                Model.IsActive = true;
            base.OnPreviewGotKeyboardFocus(e);
        }

        protected void SetLayoutItem(LayoutItem value)
        {
            SetValue(LayoutItemPropertyKey, value);
        }

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutDocumentControl) d).OnModelChanged(e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}