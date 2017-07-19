using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.Core;

namespace ModernApplicationFramework.Controls
{
    public abstract class ElementContainer : VisualWrapper
    {
        private readonly object _syncObject = new object();
        private Size _arrangedSize = Size.Empty;
        private Dispatcher _backgroundDispatcher;
        private VisualTargetPresentationSource _backgroundPresentationSource;
        private UIElement _containedElement;
        private bool _initializedOnce;
        private Size _measuredSize = Size.Empty;
        private Dictionary<DependencyProperty, object> _propertyCache;


        protected ElementContainer()
        {
            Child = new HostVisual();
            PresentationSource.AddSourceChangedHandler(this, OnPresentationSourceChanged);
        }

        protected virtual string DispatcherGroup => null;

        protected virtual int StackSize => 0;

        private void OnPresentationSourceChanged(object sender, SourceChangedEventArgs e)
        {
            if (e.OldSource != null && e.NewSource != null)
                return;
            if (e.OldSource != null)
            {
                if (_backgroundDispatcher == null)
                    return;
                _backgroundDispatcher.BeginInvoke(new Action(DisconnectHostedVisualFromSourceWorker));
            }
            else
            {
                if (e.NewSource == null)
                    return;
                if (!_initializedOnce)
                {
                    _initializedOnce = true;
                    if (_backgroundDispatcher == null)
                    {
                        var dispatcherName = DispatcherGroup;
                        if (string.IsNullOrEmpty(dispatcherName))
                            dispatcherName = "ElementContainer" + Guid.NewGuid();
                        _backgroundDispatcher = BackgroundDispatcher.GetBackgroundDispatcher(dispatcherName, StackSize);
                    }
                    _backgroundDispatcher.BeginInvoke(new Action(CreateHostedVisualWorker));
                }
                else
                {
                    _backgroundDispatcher.BeginInvoke(new Action(ConnectHostedVisualToSourceWorker));
                }
            }
        }

        protected abstract UIElement CreateRootUiElement();

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (!ShouldForwardPropertyChange(e))
                return;
            ForwardPropertyChange(e.Property, e.NewValue);
        }

        private void ForwardPropertyChange(DependencyProperty dp, object newValue)
        {
            lock (_syncObject)
            {
                if (_containedElement == null)
                {
                    if (_propertyCache == null)
                        _propertyCache = new Dictionary<DependencyProperty, object>();
                    if (newValue == DependencyProperty.UnsetValue)
                        _propertyCache.Remove(dp);
                    else
                        _propertyCache[dp] = newValue;
                }
                else
                {
                    _containedElement.Dispatcher.BeginInvoke((Action) (() => _containedElement.SetValue(dp, newValue)),
                        DispatcherPriority.DataBind);
                }
            }
        }

        protected virtual bool ShouldForwardPropertyChange(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.ReadOnly)
                return false;
            var metadata = e.Property.GetMetadata(typeof(FrameworkElement)) as FrameworkPropertyMetadata;
            return metadata != null && metadata.Inherits;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            lock (_syncObject)
            {
                if (_containedElement == null)
                {
                    _measuredSize = availableSize;
                    return Size.Empty;
                }
            }
            _containedElement.Dispatcher.BeginInvoke((Action) (() => InnerMeasure(availableSize)),
                DispatcherPriority.Render);
            return _containedElement.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            lock (_syncObject)
            {
                if (_containedElement == null)
                {
                    _arrangedSize = finalSize;
                    return finalSize;
                }
            }
            _containedElement.Dispatcher.BeginInvoke((Action) (() => InnerArrange(finalSize)),
                DispatcherPriority.Render);
            return finalSize;
        }

        private void CreateHostedVisualWorker()
        {
            _backgroundDispatcher.VerifyAccess();
            var rootUiElement = CreateRootUiElement();
            lock (_syncObject)
            {
                _containedElement = rootUiElement;
                if (!_measuredSize.IsEmpty)
                    InnerMeasure(_measuredSize);
                if (!_arrangedSize.IsEmpty)
                    InnerArrange(_arrangedSize);
                ForwardCachedProperties(_containedElement);
            }
            ConnectHostedPresentationSource();
        }

        private void ConnectHostedVisualToSourceWorker()
        {
            _backgroundDispatcher.VerifyAccess();
            ConnectHostedPresentationSource();
        }

        private void ConnectHostedPresentationSource()
        {
            var presentationSource = new VisualTargetPresentationSource((HostVisual) Child);
            var containedElement = _containedElement;
            presentationSource.RootVisual = containedElement;
            _backgroundPresentationSource = presentationSource;
        }

        private void DisconnectHostedVisualFromSourceWorker()
        {
            _backgroundDispatcher.VerifyAccess();
            using (_backgroundPresentationSource)
            {
                _backgroundPresentationSource = null;
            }
        }

        private void ForwardCachedProperties(Visual visual)
        {
            if (_propertyCache == null)
                return;
            foreach (var keyValuePair in _propertyCache)
                visual.SetValue(keyValuePair.Key, keyValuePair.Value);
            _propertyCache.Clear();
            _propertyCache = null;
        }

        private void InnerMeasure(Size availableSize)
        {
            var desiredSize = _containedElement.DesiredSize;
            _containedElement.Measure(availableSize);
            if (!(desiredSize != _containedElement.DesiredSize))
                return;
            Dispatcher.BeginInvoke((Action) InvalidateMeasure, DispatcherPriority.Render);
        }

        private void InnerArrange(Size finalSize)
        {
            var renderSize = _containedElement.RenderSize;
            _containedElement.Arrange(new Rect(finalSize));
            if (!(renderSize != _containedElement.RenderSize))
                return;
            Dispatcher.BeginInvoke((Action) InvalidateVisual, DispatcherPriority.Render);
        }
    }
}