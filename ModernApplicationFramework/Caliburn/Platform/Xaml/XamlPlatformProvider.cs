﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ModernApplicationFramework.Caliburn.Conductor;
using ModernApplicationFramework.Caliburn.Extensions;
using ModernApplicationFramework.Caliburn.Interfaces;
using ModernApplicationFramework.Caliburn.Logger;
using ModernApplicationFramework.Caliburn.PlatformProvider;

namespace ModernApplicationFramework.Caliburn.Platform.Xaml
{
    /// <summary>
    /// A <see cref="IPlatformProvider"/> implementation for the XAML platfrom.
    /// </summary>
    public class XamlPlatformProvider : IPlatformProvider
    {
        private static readonly DependencyProperty PreviouslyAttachedProperty = DependencyProperty.RegisterAttached(
            "PreviouslyAttached",
            typeof (bool),
            typeof (XamlPlatformProvider),
            null
            );

        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlPlatformProvider"/> class.
        /// </summary>
        public XamlPlatformProvider()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// Executes the action on the UI thread asynchronously.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public void BeginOnUiThread(System.Action action)
        {
            ValidateDispatcher();
            _dispatcher.BeginInvoke(action);
        }

        /// <summary>
        /// Executes the handler the fist time the view is loaded.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="handler">The handler.</param>
        public void ExecuteOnFirstLoad(object view, Action<object> handler)
        {
            var element = view as FrameworkElement;
            if (element == null || (bool) element.GetValue(PreviouslyAttachedProperty))
                return;
            element.SetValue(PreviouslyAttachedProperty, true);
            View.ExecuteOnLoad(element, (s, e) => handler(s));
        }

        /// <summary>
        /// Executes the handler the next time the view's LayoutUpdated event fires.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="handler">The handler.</param>
        public void ExecuteOnLayoutUpdated(object view, Action<object> handler)
        {
            var element = view as FrameworkElement;
            if (element != null)
            {
                View.ExecuteOnLayoutUpdated(element, (s, e) => handler(s));
            }
        }

        /// <summary>
        /// Used to retrieve the root, non-framework-created view.
        /// </summary>
        /// <param name="view">The view to search.</param>
        /// <returns>
        /// The root element that was not created by the framework.
        /// </returns>
        /// <remarks>
        /// In certain instances the services create UI elements.
        /// For example, if you ask the window manager to show a UserControl as a dialog, it creates a window to host the UserControl in.
        /// The WindowManager marks that element as a framework-created element so that it can determine what it created vs. what was intended by the developer.
        /// Calling GetFirstNonGeneratedView allows the framework to discover what the original element was.
        /// </remarks>
        public object GetFirstNonGeneratedView(object view)
        {
            return View.GetFirstNonGeneratedView(view);
        }

        /// <summary>
        /// Get the close action for the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model to close.</param>
        /// <param name="views">The associated views.</param>
        /// <param name="dialogResult">The dialog result.</param>
        /// <returns>
        /// An <see cref="Platform.Action.Action" /> to close the view model.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public System.Action GetViewCloseAction(object viewModel, ICollection<object> views, bool? dialogResult)
        {
            var child = viewModel as IChild;
            var conductor = child?.Parent as IConductor;
            if (conductor != null)
            {
                return () => conductor.CloseItem(viewModel);
            }

            foreach (var contextualView in views)
            {
                var viewType = contextualView.GetType();
                var closeMethod = viewType.GetMethod("Close");
                if (closeMethod != null)
                    return () =>
                    {
                        var isClosed = false;
                        if (dialogResult != null)
                        {
                            var resultProperty = contextualView.GetType().GetProperty("DialogResult");
                            if (resultProperty != null)
                            {
                                resultProperty.SetValue(contextualView, dialogResult, null);
                                isClosed = true;
                            }
                        }

                        if (!isClosed)
                        {
                            closeMethod.Invoke(contextualView, null);
                        }
                    };
                var isOpenProperty = viewType.GetProperty("IsOpen");
                if (isOpenProperty != null)
                {
                    return () => isOpenProperty.SetValue(contextualView, false, null);
                }
            }

            return
                () =>
                    LogManager.GetLog(typeof (Screen))
                        .Info("TryClose requires a parent IConductor or a view with a Close method or IsOpen property.");
        }

        /// <summary>
        /// Indicates whether or not the framework is in design-time mode.
        /// </summary>
        public bool InDesignMode => View.InDesignMode;

        /// <summary>
        /// Executes the action on the UI thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void OnUiThread(System.Action action)
        {
            if (CheckAccess())
                action();
            else
            {
                Exception exception = null;
                System.Action method = () =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                };
                _dispatcher.Invoke(method);
                if (exception != null)
                    throw new System.Reflection.TargetInvocationException(
                        "An error occurred while dispatching a call to the UI Thread", exception);
            }
        }

        /// <summary>
        /// Executes the action on the UI thread asynchronously.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns></returns>
        public Task OnUiThreadAsync(System.Action action)
        {
            ValidateDispatcher();
            return _dispatcher.InvokeAsync(action).Task;
        }

        private bool CheckAccess()
        {
            return _dispatcher == null || _dispatcher.CheckAccess();
        }

        private void ValidateDispatcher()
        {
            if (_dispatcher == null)
                throw new InvalidOperationException("Not initialized with dispatcher.");
        }
    }
}