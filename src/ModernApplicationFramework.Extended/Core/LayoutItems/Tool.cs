using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.InfoBar;
using ModernApplicationFramework.Controls.InfoBar;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Extended.Core.Pane;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Controls.InfoBar;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Core.LayoutItems
{
    public abstract class Tool : LayoutItemBase, ITool
    {
        public event EventHandler<InfoBarEventArgs> InfoBarClosed;

        public event EventHandler<InfoBarActionItemEventArgs> InfoBarActionItemClicked;


        private List<object> _pendingInfoBars;
        private ICommand _closeCommand;
        private bool _isVisible;

        private LayoutAnchorableItem _frame;

        private ConditionalWeakTable<InfoBarModel, IInfoBarUiElement> _infoBars;

        public override ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new DelegateCommand(p => IsVisible = false)); }
        }

        public override bool ShouldReopenOnStart => true;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                NotifyOfPropertyChange(() => IsVisible);
            }
        }

        public virtual double PreferredHeight => 200;

        public abstract PaneLocation PreferredLocation { get; }

        public virtual double PreferredWidth => 200;


        protected Tool()
        {
            IsVisible = true; 
        }

        public void AddInfoBar(InfoBarModel infoBar)
        {
            if (_infoBars == null)
                _infoBars = new ConditionalWeakTable<InfoBarModel, IInfoBarUiElement>();
            if (_infoBars.TryGetValue(infoBar, out var uiElement))
                throw new InvalidOperationException("Duplicate");
            if (!TryCreateInfoBarUI(infoBar, out uiElement))
                throw new InvalidOperationException("Creation failed");
            _infoBars.Add(infoBar, uiElement);
            AddInfoBar(new InfoBarEvents(this, uiElement, infoBar));
        }

        public void AddInfoBar(IInfoBarUiElement uiElement)
        {
            if (uiElement != null)
                AddInfoBar(new InfoBarEvents(this, uiElement));
        }

        public void RemoveInfoBar(InfoBarModel infoBar)
        {
            if (_infoBars == null || !_infoBars.TryGetValue(infoBar, out var infoBarUiElement))
                return;
            RemoveInfoBar(infoBarUiElement);
        }

        public void RemoveInfoBar(IInfoBarUiElement uiElement)
        {
            uiElement?.Close();
        }


        public virtual void OnToolWindowCreated()
        {
            ConnectInfoBars();
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            if (view == null)
            {
                _frame = null;
                return;
            }
            if (!(view is UIElement uiElement))
                throw new InvalidCastException("View is not typeof UIElement");
            _frame = uiElement.FindLogicalAncestor<LayoutAnchorableControl>()?.LayoutItem as LayoutAnchorableItem;
            //if (_frame == null)
            //    throw new InvalidCastException("View parent is not typeof LayoutAnchorableItem"); ;
            OnToolWindowCreated();
        }

        protected virtual void OnInfoBarClosed(IInfoBarUiElement infoBarUi, InfoBarModel infoBar)
        {
            if (infoBar != null)
                _infoBars?.Remove(infoBar);
            EventHandler<InfoBarEventArgs> infoBarClosed = InfoBarClosed;
            infoBarClosed?.Invoke(this, new InfoBarEventArgs(infoBarUi, infoBar));
        }

        protected virtual void OnInfoBarActionItemClicked(IInfoBarUiElement infoBarUi, InfoBarModel infoBar, IInfoBarActionItem actionItem)
        {
            var actionItemClicked = InfoBarActionItemClicked;
            actionItemClicked?.Invoke(this, new InfoBarActionItemEventArgs(infoBarUi, infoBar, actionItem));
        }

        private void AddInfoBar(InfoBarEvents events)
        {
            if (_frame == null)
                AddPendingInfoBar(events);
            else
                AddInfoBarDirect(events.InfoBarUiElement);
        }

        private void AddPendingInfoBar(object uiElementOrInfoBarEvents)
        {
            if (_pendingInfoBars == null)
                _pendingInfoBars = new List<object>();
            _pendingInfoBars.Add(uiElementOrInfoBarEvents);
        }

        private void ConnectInfoBars()
        {
            if (_pendingInfoBars == null)
                return;
            IEnumerable<object> pendingInfoBars = _pendingInfoBars;
            _pendingInfoBars = null;
            ConnectInfoBarsDirect(pendingInfoBars);
        }

        private void ConnectInfoBarsDirect(IEnumerable<object> infoBarObjects)
        {
            foreach (var infoBarObject in infoBarObjects)
            {
                if (infoBarObject is InfoBarEvents infoBarEvents)
                    AddInfoBarDirect(infoBarEvents.InfoBarUiElement);
                else
                    if (infoBarObject is IInfoBarUiElement uiElement)
                        AddInfoBarDirect(uiElement);
            }
        }

        private void AddInfoBarDirect(IInfoBarUiElement infoBarUiElement)
        {
            if (!TryGetInfoBarHost(out var infoBarHost))
                throw new InvalidOperationException("InfoBar host failed");
            infoBarHost.AddInfoBar(infoBarUiElement);
        }

        private bool TryGetInfoBarHost(out IInfoBarHost infoBarHost)
        {
            if (_frame == null)
            {
                infoBarHost = null;
                return false;
            }
            infoBarHost = _frame.InfoBarHost;
            return infoBarHost != null;
        }

        private bool TryCreateInfoBarUI(InfoBarModel infoBar, out IInfoBarUiElement uiElement)
        {
            var service = IoC.Get<IInfoBarUiFactory>();
            if (service == null)
            {
                uiElement = null;
                return false;
            }
            uiElement = service.CreateInfoBar(infoBar);
            return uiElement != null;
        }


        private class InfoBarEvents : IInfoBarUiEvents
        {
            private readonly Tool _pane;
            private readonly InfoBarModel _infoBar;
            private readonly IInfoBarUiElement _infoBarUi;
            private readonly uint _eventCookie;

            public InfoBarEvents(Tool pane, IInfoBarUiElement infoBarUi, InfoBarModel infoBar = null)
            {
                Validate.IsNotNull(pane, nameof(pane));
                Validate.IsNotNull(infoBarUi, nameof(infoBarUi));
                _pane = pane;
                _infoBar = infoBar;
                _infoBarUi = infoBarUi;
                _infoBarUi.Advise(this, out _eventCookie);
            }

            public IInfoBarUiElement InfoBarUiElement => _infoBarUi;

            public void OnActionItemClicked(IInfoBarUiElement infoBarUiElement, IInfoBarActionItem actionItem)
            {
                _pane.OnInfoBarActionItemClicked(infoBarUiElement, _infoBar, actionItem);
            }

            public void OnClosed(IInfoBarUiElement infoBarUiElement)
            {
                _infoBarUi.Unadvise(_eventCookie);
                _pane.OnInfoBarClosed(infoBarUiElement, _infoBar);
            }
        }


    }
}