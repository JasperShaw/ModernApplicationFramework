using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Core.CommandFocus
{
    internal abstract class RestoreFocusScope
    {
        private List<DependencyObject> _focusAncestors;

        protected IInputElement RestoreFocus
        {
            get
            {
                IInputElement inputElement = null;
                if (_focusAncestors != null)
                    inputElement = _focusAncestors.Find(e => e.IsConnectedToPresentationSource()) as IInputElement;
                return inputElement;
            }
            private set
            {
                _focusAncestors = new List<DependencyObject>();
                for (var sourceElement = value as DependencyObject;
                    sourceElement != null;
                    sourceElement = sourceElement.GetVisualOrLogicalParent())
                {
                    IInputElement inputElement;
                    if ((inputElement = sourceElement as IInputElement) != null && inputElement.Focusable)
                        _focusAncestors.Add(sourceElement);
                }
            }
        }

        protected IntPtr RestoreFocusWindow { get; }

        protected RestoreFocusScope(IInputElement restoreFocus, IntPtr restoreFocusWindow)
        {
            RestoreFocusWindow = restoreFocusWindow;
            RestoreFocus = restoreFocus;
        }

        public virtual void PerformRestoration()
        {
            if (!IsRestorationTargetValid())
                return;
            if (RestoreFocusWindow != IntPtr.Zero)
            {
                User32.SetFocus(RestoreFocusWindow);
            }
            else
            {
                if (RestoreFocus == null)
                    return;
                Keyboard.Focus(RestoreFocus);
            }
        }

        protected abstract bool IsRestorationTargetValid();
    }
}