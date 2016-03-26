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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    public class NavigatorWindow : Window, IHasTheme
    {
        private static readonly DependencyPropertyKey AnchorablesPropertyKey
            = DependencyProperty.RegisterReadOnly("Anchorables", typeof (IEnumerable<LayoutAnchorableItem>),
                typeof (NavigatorWindow),
                new FrameworkPropertyMetadata((IEnumerable<LayoutAnchorableItem>) null));

        public static readonly DependencyProperty AnchorablesProperty
            = AnchorablesPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey DocumentsPropertyKey
            = DependencyProperty.RegisterReadOnly("Documents", typeof (IEnumerable<LayoutDocumentItem>),
                typeof (NavigatorWindow),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty DocumentsProperty
            = DocumentsPropertyKey.DependencyProperty;

        public static readonly DependencyProperty SelectedDocumentProperty =
            DependencyProperty.Register("SelectedDocument", typeof (LayoutDocumentItem), typeof (NavigatorWindow),
                new FrameworkPropertyMetadata(null,
                    OnSelectedDocumentChanged));

        public static readonly DependencyProperty SelectedAnchorableProperty =
            DependencyProperty.Register("SelectedAnchorable", typeof (LayoutAnchorableItem), typeof (NavigatorWindow),
                new FrameworkPropertyMetadata(null,
                    OnSelectedAnchorableChanged));


        private readonly DockingManager _manager;
        private bool _internalSetSelectedDocument;


        private Theme _theme;

        internal NavigatorWindow(DockingManager manager)
        {
            _manager = manager;

            _internalSetSelectedDocument = true;
            SetAnchorables(
                _manager.Layout.Descendents()
                    .OfType<LayoutAnchorable>()
                    .Where(a => a.IsVisible)
                    .Select(d => (LayoutAnchorableItem) _manager.GetLayoutItemFromModel(d))
                    .ToArray());
            SetDocuments(
                _manager.Layout.Descendents()
                    .OfType<LayoutDocument>()
                    .OrderByDescending(d => d.LastActivationTimeStamp.GetValueOrDefault())
                    .Select(d => (LayoutDocumentItem) _manager.GetLayoutItemFromModel(d))
                    .ToArray());
            _internalSetSelectedDocument = false;

            if (Documents.Length > 1)
                InternalSetSelectedDocument(Documents[1]);

            DataContext = this;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            ChangeTheme(null, null);
        }

        static NavigatorWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (NavigatorWindow),
                new FrameworkPropertyMetadata(typeof (NavigatorWindow)));
            ShowActivatedProperty.OverrideMetadata(typeof (NavigatorWindow), new FrameworkPropertyMetadata(false));
            ShowInTaskbarProperty.OverrideMetadata(typeof (NavigatorWindow), new FrameworkPropertyMetadata(false));
        }

        public event EventHandler<ThemeChangedEventArgs> OnThemeChanged;

        public Theme Theme
        {
            get { return _theme; }
            set
            {
                if (value == null)
                    throw new NoNullAllowedException();
                if (Equals(value, _theme))
                    return;
                var oldTheme = _theme;
                _theme = value;
                ChangeTheme(oldTheme, _theme);
                OnRaiseThemeChanged(new ThemeChangedEventArgs(value, oldTheme));
            }
        }

        public IEnumerable<LayoutAnchorableItem> Anchorables
            => (IEnumerable<LayoutAnchorableItem>) GetValue(AnchorablesProperty);

        public LayoutDocumentItem[] Documents => (LayoutDocumentItem[]) GetValue(DocumentsProperty);

        public LayoutAnchorableItem SelectedAnchorable
        {
            get { return (LayoutAnchorableItem) GetValue(SelectedAnchorableProperty); }
            set { SetValue(SelectedAnchorableProperty, value); }
        }

        public LayoutDocumentItem SelectedDocument
        {
            get { return (LayoutDocumentItem) GetValue(SelectedDocumentProperty); }
            set { SetValue(SelectedDocumentProperty, value); }
        }

        public void ChangeTheme(Theme oldValue, Theme newValue)
        {
            if (oldValue != null)
            {
                var resourceDictionaryToRemove =
                    Resources.MergedDictionaries.FirstOrDefault(r => r.Source == oldValue.GetResourceUri());
                if (resourceDictionaryToRemove != null)
                    Resources.MergedDictionaries.Remove(
                        resourceDictionaryToRemove);
            }
            if (newValue != null)
                Resources.MergedDictionaries.Add(new ResourceDictionary {Source = newValue.GetResourceUri()});
        }

        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Tab)
            {
                SelectNextDocument();
                e.Handled = true;
            }


            base.OnPreviewKeyDown(e);
        }

        protected override void OnPreviewKeyUp(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Tab)
            {
                if (SelectedAnchorable != null &&
                    SelectedAnchorable.ActivateCommand.CanExecute(null))
                    SelectedAnchorable.ActivateCommand.Execute(null);

                if (SelectedAnchorable == null &&
                    SelectedDocument != null &&
                    SelectedDocument.ActivateCommand.CanExecute(null))
                    SelectedDocument.ActivateCommand.Execute(null);
                Close();
                e.Handled = true;
            }


            base.OnPreviewKeyUp(e);
        }

        protected virtual void OnRaiseThemeChanged(ThemeChangedEventArgs e)
        {
            var handler = OnThemeChanged;
            handler?.Invoke(this, e);
        }

        protected virtual void OnSelectedAnchorableChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedAnchorable != null &&
                SelectedAnchorable.ActivateCommand.CanExecute(null))
            {
                SelectedAnchorable.ActivateCommand.Execute(null);
                Close();
            }
        }

        protected virtual void OnSelectedDocumentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_internalSetSelectedDocument)
                return;

            if (SelectedDocument != null &&
                SelectedDocument.ActivateCommand.CanExecute(null))
            {
                System.Diagnostics.Trace.WriteLine("OnSelectedDocumentChanged()");
                SelectedDocument.ActivateCommand.Execute(null);
                Hide();
            }
        }

        protected void SetAnchorables(IEnumerable<LayoutAnchorableItem> value)
        {
            SetValue(AnchorablesPropertyKey, value);
        }

        protected void SetDocuments(LayoutDocumentItem[] value)
        {
            SetValue(DocumentsPropertyKey, value);
        }

        internal void SelectNextDocument()
        {
            if (SelectedDocument != null)
            {
                int docIndex = Documents.IndexOf(SelectedDocument);
                docIndex++;
                if (docIndex == Documents.Length)
                    docIndex = 0;
                InternalSetSelectedDocument(Documents[docIndex]);
            }
        }

        private static void OnSelectedAnchorableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NavigatorWindow) d).OnSelectedAnchorableChanged(e);
        }

        private static void OnSelectedDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NavigatorWindow) d).OnSelectedDocumentChanged(e);
        }

        private void InternalSetSelectedDocument(LayoutDocumentItem documentToSelect)
        {
            _internalSetSelectedDocument = true;
            SelectedDocument = documentToSelect;
            _internalSetSelectedDocument = false;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            Focus();

            //this.SetParentToMainWindowOf(_manager);
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= OnUnloaded;

            //_hwndSrc.RemoveHook(_hwndSrcHook);
            //_hwndSrc.Dispose();
            //_hwndSrc = null;
        }
    }
}