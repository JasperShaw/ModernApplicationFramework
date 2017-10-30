using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
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

        /// <summary>
        /// Fires when a key was pressed inside a registered window
        /// </summary>
        public event KeyEventHandler PreviewKeyDown;

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
                WeakEventManager<Window, EventArgs>.AddHandler(window, "PreviewKeyDown", Handler);
            _list.Add(new WeakReference(window));
        }

        /// <inheritdoc />
        /// <summary>
        /// Unregisters a window from the service
        /// </summary>
        /// <param name="window">The window.</param>
        public void Unregister(Window window)
        {
            WeakEventManager<Window, EventArgs>.RemoveHandler(window, "PreviewKeyDown", Handler);
            _list.Remove(_list.Find(x => x.Target.Equals(window)));
        }

        private void UpdateHandlers()
        {
            foreach (var weakReference in _list.Where(x => x.IsAlive))
                if (weakReference.Target is Window window)
                    WeakEventManager<Window, EventArgs>.RemoveHandler(window, "PreviewKeyDown", Handler);
        }

        private void Handler(object sender, EventArgs eventArgs)
        {
            if (eventArgs is KeyEventArgs e && Enabled)
                OnPreviewKeyDown(e);
        }

        /// <summary>
        /// Raises the <see cref="E:PreviewKeyDown" /> event.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPreviewKeyDown(KeyEventArgs e)
        {
            PreviewKeyDown?.Invoke(this, e);
        }
    }
}