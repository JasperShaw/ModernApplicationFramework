using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Controls.InfoBar.Utilities;
using ModernApplicationFramework.Core.MenuModeHelper;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.InfoBar
{
    public sealed class InfoBarHostControl : Control, IInfoBarHost, IInfoBarUiEvents
    {
        private readonly InfoBarFrameworkElementCollection _infoBarFrameworkElements;

        private readonly ObservableCollection<IInfoBarUiElement> _infoBars;


        private HybridDictionary<IInfoBarUiElement, uint> _eventCookies;

        public IReadOnlyList<FrameworkElement> InfoBars => _infoBarFrameworkElements;

        private HybridDictionary<IInfoBarUiElement, uint> EventCookies =>
            _eventCookies ?? (_eventCookies = new HybridDictionary<IInfoBarUiElement, uint>());


        static InfoBarHostControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InfoBarHostControl),
                new FrameworkPropertyMetadata(typeof(InfoBarHostControl)));
        }

        public InfoBarHostControl()
        {
            _infoBars = new ObservableCollection<IInfoBarUiElement>();
            _infoBarFrameworkElements = new InfoBarFrameworkElementCollection(_infoBars);
            
        }

        public void AddInfoBar(IInfoBarUiElement uiElement)
        {
            Validate.IsNotNull(uiElement, nameof(uiElement));
            if (_infoBars.Contains(uiElement))
                throw new InvalidOperationException("Duplicate");
            _infoBars.Add(uiElement);

            IInfoBarUiElement key = uiElement;
            if(key != null && key.Advise(this, out var cookie)== 0)
            EventCookies.Add(key, cookie);
        }


        public void RemoveInfoBar(IInfoBarUiElement uiElement)
        {
            Validate.IsNotNull(uiElement, nameof(uiElement));
            _infoBars.Remove(uiElement);
            var key = uiElement;
            if (key == null || !EventCookies.TryGetValue(key, out var cookie))
                return;
            key.Unadvise(cookie);
            EventCookies.Remove(key);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key != Key.Escape || !RestoreKeyboardFocus())
                return;
            e.Handled = true;
        }

        private bool RestoreKeyboardFocus()
        {
            if (CommandBarNavigationHelper.GetCommandFocusMode(this) == CommandBarNavigationHelper.CommandFocusMode.None)
                return false;
            Keyboard.Focus(null);
            return true;
        }

        private sealed class InfoBarFrameworkElementCollection : CollectionAdapter<IInfoBarUiElement, FrameworkElement>
        {
            public InfoBarFrameworkElementCollection(ObservableCollection<IInfoBarUiElement> baseCollection)
            {
                Initialize(baseCollection);
            }

            protected override FrameworkElement AdaptItem(IInfoBarUiElement item)
            {
                return item.CreateControl();
            }
        }

        public void OnClosed(IInfoBarUiElement infoBarUIElement)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                RestoreKeyboardFocus();
                RemoveInfoBar(infoBarUIElement);
            }));
        }

        public void OnActionItemClicked(IInfoBarUiElement infoBarUIElement, IInfoBarActionItem actionItem)
        {
        }
    }
}
