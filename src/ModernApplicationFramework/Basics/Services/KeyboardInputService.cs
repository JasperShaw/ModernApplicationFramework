using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Core.MenuModeHelper;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Services
{
    /// <inheritdoc />
    /// <summary>
    /// An implementation of an <see cref="IKeyboardInputService"/> using weak references for the registered windows
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Services.IKeyboardInputService" />
    [Export(typeof(IKeyboardInputService))]
    internal class KeyboardInputService : IKeyboardInputService
    {
        private static IKeyboardInputService _instance;

        private readonly List<WeakReference> _list = new List<WeakReference>();
        private bool _enabled;

        public static IKeyboardInputService Instance => _instance ??
                                                        (_instance = IoC.Get<IKeyboardInputService>());

        /// <inheritdoc />
        /// <summary>
        /// Fires when a key was pressed inside a registered window
        /// </summary>
        public event KeyEventHandler PreviewKeyDown;
        public event KeyEventHandler PreviewKeyUp;

        public KeyboardInputService()
        {
            Enabled = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="KeyboardInputService"/> is enabled.
        /// </summary>
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (value == _enabled)
                    return;
                _enabled = value;
                UpdateHandlers();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Registers a window the service
        /// </summary>
        /// <param name="window">The window.</param>
        public void Register(Window window)
        {
            if (Enabled)
            {
                WeakEventManager<Window, EventArgs>.AddHandler(window, "PreviewKeyDown", DownHandler);
                WeakEventManager<Window, EventArgs>.AddHandler(window, "PreviewKeyUp", UpHandler);
            }
            _list.Add(new WeakReference(window));
        }

        /// <inheritdoc />
        /// <summary>
        /// Unregisters a window from the service
        /// </summary>
        /// <param name="window">The window.</param>
        public void Unregister(Window window)
        {
            WeakEventManager<Window, EventArgs>.RemoveHandler(window, "PreviewKeyDown", DownHandler);
            WeakEventManager<Window, EventArgs>.RemoveHandler(window, "PreviewKeyUp", UpHandler);
            _list.Remove(_list.Find(x => x.Target.Equals(window)));
        }

        private void UpdateHandlers()
        {
            foreach (var weakReference in _list.Where(x => x.IsAlive))
                if (weakReference.Target is Window window)
                {
                    WeakEventManager<Window, EventArgs>.RemoveHandler(window, "PreviewKeyDown", DownHandler);
                    WeakEventManager<Window, EventArgs>.RemoveHandler(window, "PreviewKeyUp", UpHandler);
                }
                    
        }

        private void DownHandler(object sender, EventArgs eventArgs)
        {
            if (eventArgs is KeyEventArgs e && Enabled)
                OnPreviewKeyDown(e);
        }

        private void UpHandler(object sender, EventArgs eventArgs)
        {
            if (eventArgs is KeyEventArgs e && Enabled)
                OnPreviewKeyUp(e);
        }

        private void OnPreviewKeyUp(KeyEventArgs e)
        {
            var realKey = KeyboardUtilities.GetRealPressedKey(e);
            e.Handled = MenuModeHelper.TranslateAccelerator(realKey, true);
            PreviewKeyUp?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:PreviewKeyDown" /> event.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Handled)
                MenuModeHelper.ResetState();
            var realKey = KeyboardUtilities.GetRealPressedKey(e);
            e.Handled = MenuModeHelper.TranslateAccelerator(realKey, false);
            Trace.WriteLine(e.Handled + InputManager.Current.IsInMenuMode.ToString());
            PreviewKeyDown?.Invoke(this, e);
        }
    }
}