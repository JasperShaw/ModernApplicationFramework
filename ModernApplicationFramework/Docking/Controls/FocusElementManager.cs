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
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
	internal static class FocusElementManager
	{
		private static readonly List<DockingManager> Managers = new List<DockingManager>();

		private static readonly FullWeakDictionary<ILayoutElement, IInputElement> ModelFocusedElement =
			new FullWeakDictionary<ILayoutElement, IInputElement>();

		private static readonly WeakDictionary<ILayoutElement, IntPtr> ModelFocusedWindowHandle =
			new WeakDictionary<ILayoutElement, IntPtr>();

		private static WeakReference _lastFocusedElement;
		private static WeakReference _lastFocusedElementBeforeEnterMenuMode;
		private static DispatcherOperation _setFocusAsyncOperation;
		private static WindowHookHandler _windowHandler;

		internal static void FinalizeFocusManagement(DockingManager manager)
		{
			manager.PreviewGotKeyboardFocus -= manager_PreviewGotKeyboardFocus;
			Managers.Remove(manager);

			if (Managers.Count != 0)
				return;
			//InputManager.Current.EnterMenuMode -= new EventHandler(InputManager_EnterMenuMode);
			//InputManager.Current.LeaveMenuMode -= new EventHandler(InputManager_LeaveMenuMode);
			if (_windowHandler == null)
				return;
			_windowHandler.FocusChanged -= WindowFocusChanging;
			//_windowHandler.Activate -= new EventHandler<WindowActivateEventArgs>(WindowActivating);
			_windowHandler.Detach();
			_windowHandler = null;
		}

		/// <summary>
		/// Get the input element that was focused before user left the layout element
		/// </summary>
		/// <param name="model">Element to look for</param>
		/// <returns>Input element </returns>
		internal static IInputElement GetLastFocusedElement(ILayoutElement model)
		{
			IInputElement objectWithFocus;
			if (ModelFocusedElement.GetValue(model, out objectWithFocus))
				return objectWithFocus;

			return null;
		}

		/// <summary>
		/// Get the last window handle focused before user left the element passed as argument
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		internal static IntPtr GetLastWindowHandle(ILayoutElement model)
		{
			IntPtr handleWithFocus;
			return ModelFocusedWindowHandle.GetValue(model, out handleWithFocus) ? handleWithFocus : IntPtr.Zero;
		}

		/// <summary>
		/// Given a layout element tries to set the focus of the keyword where it was before user moved to another element
		/// </summary>
		/// <param name="model"></param>
		internal static void SetFocusOnLastElement(ILayoutElement model)
		{
			bool focused = false;
			IInputElement objectToFocus;
			if (ModelFocusedElement.GetValue(model, out objectToFocus))
			{
				focused = objectToFocus == Keyboard.Focus(objectToFocus);
			}

			IntPtr handleToFocus;
			if (ModelFocusedWindowHandle.GetValue(model, out handleToFocus))
				focused = IntPtr.Zero != Win32Helper.SetFocus(handleToFocus);

			Trace.WriteLine(string.Format("SetFocusOnLastElement(focused={0}, model={1}, element={2})", focused, model,
				handleToFocus == IntPtr.Zero ? (objectToFocus?.ToString() ?? "") : handleToFocus.ToString()));

			if (focused)
			{
				_lastFocusedElement = new WeakReference(model);
			}
		}

		internal static void SetupFocusManagement(DockingManager manager)
		{
			if (Managers.Count == 0)
			{
				//InputManager.Current.EnterMenuMode += new EventHandler(InputManager_EnterMenuMode);
				//InputManager.Current.LeaveMenuMode += new EventHandler(InputManager_LeaveMenuMode);
				_windowHandler = new WindowHookHandler();
				_windowHandler.FocusChanged += WindowFocusChanging;
				//_windowHandler.Activate += new EventHandler<WindowActivateEventArgs>(WindowActivating);
				_windowHandler.Attach();

				if (Application.Current != null)
					Application.Current.Exit += Current_Exit;
			}

			manager.PreviewGotKeyboardFocus += manager_PreviewGotKeyboardFocus;
			Managers.Add(manager);
		}

		private static void Current_Exit(object sender, ExitEventArgs e)
		{
			Application.Current.Exit -= Current_Exit;
			if (_windowHandler == null)
				return;
			_windowHandler.FocusChanged -= WindowFocusChanging;
			//_windowHandler.Activate -= new EventHandler<WindowActivateEventArgs>(WindowActivating);
			_windowHandler.Detach();
			_windowHandler = null;
		}

		private static void InputManager_EnterMenuMode(object sender, EventArgs e)
		{
			if (Keyboard.FocusedElement == null)
				return;

			var lastfocusDepObj = Keyboard.FocusedElement as DependencyObject;
			if (lastfocusDepObj.FindLogicalAncestor<DockingManager>() == null)
			{
				_lastFocusedElementBeforeEnterMenuMode = null;
				return;
			}

			_lastFocusedElementBeforeEnterMenuMode = new WeakReference(Keyboard.FocusedElement);
		}

		private static void InputManager_LeaveMenuMode(object sender, EventArgs e)
		{
			if (_lastFocusedElementBeforeEnterMenuMode != null &&
			    _lastFocusedElementBeforeEnterMenuMode.IsAlive)
			{
				var lastFocusedInputElement = _lastFocusedElementBeforeEnterMenuMode.GetValueOrDefault<UIElement>();
				if (lastFocusedInputElement != null)
				{
					if (lastFocusedInputElement != Keyboard.Focus(lastFocusedInputElement))
						Debug.WriteLine("Unable to activate the element");
				}
			}
		}

		private static void manager_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			var focusedElement = e.NewFocus as Visual;
			if (focusedElement == null ||
			    (focusedElement is LayoutAnchorableTabItem || focusedElement is LayoutDocumentTabItem))
				return;
			var parentAnchorable = focusedElement.FindVisualAncestor<LayoutAnchorableControl>();
			if (parentAnchorable != null)
			{
				ModelFocusedElement[parentAnchorable.Model] = e.NewFocus;
			}
			else
			{
				var parentDocument = focusedElement.FindVisualAncestor<LayoutDocumentControl>();
				if (parentDocument != null)
				{
					ModelFocusedElement[parentDocument.Model] = e.NewFocus;
				}
			}
		}

		private static void WindowActivating(object sender, WindowActivateEventArgs e)
		{
			Trace.WriteLine("WindowActivating");

			if (Keyboard.FocusedElement == null &&
			    _lastFocusedElement != null &&
			    _lastFocusedElement.IsAlive)
			{
				var elementToSetFocus = _lastFocusedElement.Target as ILayoutElement;
				if (elementToSetFocus != null)
				{
					var manager = elementToSetFocus.Root.Manager;
					if (manager == null)
						return;

					IntPtr parentHwnd;
					if (!manager.GetParentWindowHandle(out parentHwnd))
						return;

					if (e.HwndActivating != parentHwnd)
						return;

					_setFocusAsyncOperation = Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
					{
						try
						{
							SetFocusOnLastElement(elementToSetFocus);
						}
						finally
						{
							_setFocusAsyncOperation = null;
						}
					}), DispatcherPriority.Background);
				}
			}
		}

		private static void WindowFocusChanging(object sender, FocusChangeEventArgs e)
		{
			foreach (var manager in Managers)
			{
				var hostContainingFocusedHandle =
					manager.FindLogicalChildren<HwndHost>().FirstOrDefault(hw => Win32Helper.IsChild(hw.Handle, e.GotFocusWinHandle));

				if (hostContainingFocusedHandle != null)
				{
					var parentAnchorable = hostContainingFocusedHandle.FindVisualAncestor<LayoutAnchorableControl>();
					if (parentAnchorable != null)
					{
						ModelFocusedWindowHandle[parentAnchorable.Model] = e.GotFocusWinHandle;
						if (parentAnchorable.Model != null)
							parentAnchorable.Model.IsActive = true;
					}
					else
					{
						var parentDocument = hostContainingFocusedHandle.FindVisualAncestor<LayoutDocumentControl>();
						if (parentDocument != null)
						{
							ModelFocusedWindowHandle[parentDocument.Model] = e.GotFocusWinHandle;
							if (parentDocument.Model != null)
								parentDocument.Model.IsActive = true;
						}
					}
				}
			}
		}
	}
}