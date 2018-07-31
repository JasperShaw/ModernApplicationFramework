using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal class ContainerMargin : Grid, ITextViewMargin
    {
        private bool _isDisposed;
        private bool _ignoreChildVisibilityEvents;
        internal List<Tuple<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>, ITextViewMargin>> CurrentMargins;
        private int _nonMarginChildren;
        private readonly string _marginName;
        private readonly Orientation _orientation;
        private readonly IReadOnlyList<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>> _marginProviders;
        private readonly GuardedOperations _guardedOperations;
        private Dictionary<string, int> _optionSubscriptions;
        protected readonly ITextViewHost TextViewHost;
        private static Lazy<ITextViewMarginProvider, ITextViewMarginMetadata> _workaroundMarginProvider;

        protected ContainerMargin(string name, Orientation orientation, ITextViewHost textViewHost, GuardedOperations guardedOperations, TextViewMarginState marginState)
        {
            _marginName = name;
            _orientation = orientation;
            _guardedOperations = guardedOperations;
            TextViewHost = textViewHost;
            _marginProviders = marginState.GetMarginProviders(_marginName);
        }

        public static ITextViewMargin Create(string name, Orientation orientation, ITextViewHost textViewHost, GuardedOperations guardedOperations, TextViewMarginState marginState)
        {
            ContainerMargin containerMargin = new ContainerMargin(name, orientation, textViewHost, guardedOperations, marginState);
            containerMargin.Initialize();
            return containerMargin;
        }

        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();
                if (_orientation != Orientation.Horizontal)
                    return ActualWidth;
                return ActualHeight;
            }
        }

        public virtual bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return true;
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (string.Compare(marginName, _marginName, StringComparison.OrdinalIgnoreCase) == 0)
                return this;
            for (int marginIndex = 0; marginIndex < CurrentMargins.Count; ++marginIndex)
            {
                Tuple<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>, ITextViewMargin> currentMargin = CurrentMargins[marginIndex];
                if (currentMargin.Item2 != null)
                {
                    ITextViewMargin textViewMargin = currentMargin.Item2.GetTextViewMargin(marginName);
                    if (textViewMargin != null)
                        return textViewMargin;
                }
                else if (string.Compare(marginName, currentMargin.Item1.Metadata.Name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    bool collapse = false;
                    if (_optionSubscriptions != null)
                    {
                        string optionName = currentMargin.Item1.Metadata.OptionName;
                        int num;
                        if (optionName != null && _optionSubscriptions.TryGetValue(optionName, out num) && !TextViewHost.TextView.Options.GetOptionValue<bool>(optionName))
                            collapse = true;
                    }
                    return InsertDeferredMargin(marginIndex, collapse);
                }
            }
            return null;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            Close();
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        protected void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("ContainerMarginMargin");
        }

        protected virtual void Initialize()
        {
            TextViewHost.TextView.TextDataModel.ContentTypeChanged += OnContentTypeChanged;
            IsVisibleChanged += (sender, e) =>
            {
                if ((bool)e.NewValue)
                    RegisterEvents();
                else
                    UnregisterEvents();
            };
            _nonMarginChildren = Children.Count;
            AddMargins(GetMarginProviders(), null);
        }

        protected virtual void RegisterEvents()
        {
        }

        protected virtual void UnregisterEvents()
        {
        }

        protected virtual void AddMargins(IList<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>> providers, List<Tuple<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>, ITextViewMargin>> oldMargins)
        {
            CurrentMargins = new List<Tuple<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>, ITextViewMargin>>(providers.Count);
            try
            {
                _ignoreChildVisibilityEvents = true;
                if (oldMargins != null)
                {
                    RowDefinitions.Clear();
                    ColumnDefinitions.Clear();
                    Children.RemoveRange(_nonMarginChildren, Children.Count - _nonMarginChildren);
                    if (_optionSubscriptions != null)
                    {
                        _optionSubscriptions = null;
                        TextViewHost.TextView.Options.OptionChanged -= OnOptionChanged;
                    }
                }
                int marginIndex = 0;
                foreach (var provider in providers)
                {
                    Lazy<ITextViewMarginProvider, ITextViewMarginMetadata> marginProvider = provider;
                    string optionName = marginProvider.Metadata.OptionName;
                    bool flag1 = false;
                    bool flag2 = false;
                    if (optionName != null)
                    {
                        if (optionName == "" || !TextViewHost.TextView.Options.IsOptionDefined(optionName, false))
                        {
                            flag1 = true;
                        }
                        else
                        {
                            object optionValue = TextViewHost.TextView.Options.GetOptionValue(optionName);
                            if (optionValue is bool b)
                            {
                                flag2 = true;
                                flag1 = !b;
                            }
                            else
                                flag1 = true;
                        }
                    }
                    ITextViewMargin margin = null;
                    Tuple<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>, ITextViewMargin> tuple = oldMargins?.Find(a => marginProvider == a.Item1);
                    if (tuple?.Item2 != null)
                        margin = tuple.Item2;
                    else if (!flag1)
                    {
                        margin = _guardedOperations.InstantiateExtension(marginProvider, marginProvider, mp => mp.CreateMargin(TextViewHost, this));
                        if (margin == null)
                            continue;
                    }
                    else if (marginIndex == 0)
                    {
                        if (_workaroundMarginProvider == null)
                            _workaroundMarginProvider = new Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>(new WorkaroundMetadata());
                        AddMargin(new WorkaroundMargin(), _workaroundMarginProvider, false);
                        ++marginIndex;
                    }
                    if (flag2)
                        SubscribeToOptionChange(optionName, marginIndex);
                    AddMargin(margin, marginProvider, tuple == null);
                    ++marginIndex;
                }
            }
            finally
            {
                _ignoreChildVisibilityEvents = false;
            }
            Visibility = HasVisibleChild() ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AddMargin(ITextViewMargin margin, Lazy<ITextViewMarginProvider, ITextViewMarginMetadata> marginProvider, bool trackVisibility)
        {
            CurrentMargins.Add(new Tuple<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>, ITextViewMargin>(marginProvider, margin));
            GridLength gridLength = margin == null ? GridLength.Auto : new GridLength(marginProvider.Metadata.GridCellLength, marginProvider.Metadata.GridUnitType);
            if (_orientation == Orientation.Horizontal)
            {
                RowDefinitions.Add(new RowDefinition()
                {
                    Height = gridLength
                });
                if (margin != null)
                {
                    SetColumn(margin.VisualElement, 0);
                    SetRow(margin.VisualElement, RowDefinitions.Count - 1);
                }
            }
            else
            {
                ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = gridLength
                });
                if (margin != null)
                {
                    SetColumn(margin.VisualElement, ColumnDefinitions.Count - 1);
                    SetRow(margin.VisualElement, 0);
                }
            }
            if (margin == null)
                return;
            Children.Add(margin.VisualElement);
            if (!trackVisibility)
                return;
            DependencyPropertyDescriptor.FromProperty(VisibilityProperty, typeof(UIElement))?.AddValueChanged(margin.VisualElement, OnChildMarginVisibilityChanged);
        }

        private ITextViewMargin InsertDeferredMargin(int marginIndex, bool collapse)
        {
            Tuple<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>, ITextViewMargin> currentMargin = CurrentMargins[marginIndex];
            var wpfTextViewMargin = _guardedOperations.InstantiateExtension(currentMargin.Item1, currentMargin.Item1, mp => mp.CreateMargin(TextViewHost, this));
            if (wpfTextViewMargin != null)
            {
                CurrentMargins[marginIndex] = Tuple.Create(currentMargin.Item1, wpfTextViewMargin);
                GridLength gridLength = new GridLength(currentMargin.Item1.Metadata.GridCellLength, currentMargin.Item1.Metadata.GridUnitType);
                if (_orientation == Orientation.Horizontal)
                {
                    RowDefinitions[marginIndex].Height = gridLength;
                    SetColumn(wpfTextViewMargin.VisualElement, 0);
                    SetRow(wpfTextViewMargin.VisualElement, marginIndex);
                }
                else
                {
                    ColumnDefinitions[marginIndex].Width = gridLength;
                    SetColumn(wpfTextViewMargin.VisualElement, marginIndex);
                    SetRow(wpfTextViewMargin.VisualElement, 0);
                }
                if (collapse)
                    wpfTextViewMargin.VisualElement.Visibility = Visibility.Collapsed;
                Children.Add(wpfTextViewMargin.VisualElement);
                DependencyPropertyDescriptor.FromProperty(VisibilityProperty, typeof(UIElement))?.AddValueChanged(wpfTextViewMargin.VisualElement, OnChildMarginVisibilityChanged);
            }
            return wpfTextViewMargin;
        }

        private void SubscribeToOptionChange(string optionName, int marginIndex)
        {
            if (_optionSubscriptions == null)
            {
                _optionSubscriptions = new Dictionary<string, int>();
                TextViewHost.TextView.Options.OptionChanged += OnOptionChanged;
            }
            _optionSubscriptions.Add(optionName, marginIndex);
        }

        private void OnOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (!_optionSubscriptions.TryGetValue(e.OptionId, out var marginIndex))
                return;
            ITextViewMargin wpfTextViewMargin = CurrentMargins[marginIndex].Item2;
            if (TextViewHost.TextView.Options.GetOptionValue<bool>(e.OptionId))
            {
                if (wpfTextViewMargin == null)
                    InsertDeferredMargin(marginIndex, false);
                if (wpfTextViewMargin == null)
                    return;
                wpfTextViewMargin.VisualElement.Visibility = Visibility.Visible;
            }
            else
                wpfTextViewMargin.VisualElement.Visibility = Visibility.Collapsed;
        }

        protected virtual void Close()
        {
            TextViewHost.TextView.TextDataModel.ContentTypeChanged -= OnContentTypeChanged;
            if (_optionSubscriptions != null)
                TextViewHost.TextView.Options.OptionChanged -= OnOptionChanged;
            foreach (Tuple<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>, ITextViewMargin> currentMargin in CurrentMargins)
                DisposeMargin(currentMargin.Item2);
            CurrentMargins.Clear();
        }

        private IList<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>> GetMarginProviders()
        {
            ITextView textView = TextViewHost.TextView;
            List<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>> lazyList = UiExtensionSelector.SelectMatchingExtensions(_marginProviders, textView.TextDataModel.ContentType, null, textView.Roles);
            HashSet<string> stringSet = new HashSet<string>();
            foreach (Lazy<ITextViewMarginProvider, ITextViewMarginMetadata> lazy in lazyList)
            {
                IEnumerable<string> replaces = lazy.Metadata.Replaces;
                if (replaces != null)
                {
                    foreach (string str in replaces)
                        stringSet.Add(str.ToLowerInvariant());
                }
            }
            if (stringSet.Count > 0)
            {
                for (int index = lazyList.Count - 1; index >= 0; --index)
                {
                    string name = lazyList[index].Metadata.Name;
                    if (!string.IsNullOrWhiteSpace(name) && stringSet.Contains(name.ToLowerInvariant()))
                        lazyList.RemoveAt(index);
                }
            }
            return lazyList;
        }

        private void DisposeMargin(ITextViewMargin margin)
        {
            if (margin == null)
                return;
            DependencyPropertyDescriptor.FromProperty(VisibilityProperty, typeof(UIElement))?.RemoveValueChanged(margin.VisualElement, OnChildMarginVisibilityChanged);
            margin.Dispose();
        }

        protected void OnChildMarginVisibilityChanged(object sender, EventArgs e)
        {
            if (_ignoreChildVisibilityEvents)
                return;
            if (Visibility == Visibility.Collapsed)
            {
                if (!HasVisibleChild())
                    return;
                Visibility = Visibility.Visible;
            }
            else
            {
                if (Visibility != Visibility.Visible || HasVisibleChild())
                    return;
                Visibility = Visibility.Collapsed;
            }
        }

        protected virtual bool HasVisibleChild()
        {
            foreach (Tuple<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>, ITextViewMargin> currentMargin in CurrentMargins)
            {
                if (currentMargin.Item2 != null && currentMargin.Item2.VisualElement.Visibility == Visibility.Visible)
                    return true;
            }
            return false;
        }

        private void OnContentTypeChanged(object sender, TextDataModelContentTypeChangedEventArgs e)
        {
            IList<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>> marginProviders = GetMarginProviders();
            for (int index = CurrentMargins.Count - 1; index >= 0; --index)
            {
                Tuple<Lazy<ITextViewMarginProvider, ITextViewMarginMetadata>, ITextViewMargin> currentMargin = CurrentMargins[index];
                if (!marginProviders.Contains(currentMargin.Item1))
                {
                    DisposeMargin(currentMargin.Item2);
                    CurrentMargins.RemoveAt(index);
                }
            }
            AddMargins(marginProviders, CurrentMargins);
        }
    }
}