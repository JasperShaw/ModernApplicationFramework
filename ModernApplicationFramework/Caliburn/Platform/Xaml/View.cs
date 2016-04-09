﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using ModernApplicationFramework.Caliburn.Logger;
using ModernApplicationFramework.Caliburn.Platform.Core;
using DependencyPropertyHelper = ModernApplicationFramework.Caliburn.Platform.Utilities.DependencyPropertyHelper;

namespace ModernApplicationFramework.Caliburn.Platform.Xaml
{
    /// <summary>
    ///     Hosts attached properties related to view models.
    /// </summary>
    public static class View
    {
        /// <summary>
        ///     A dependency property which allows the override of convention application behavior.
        /// </summary>
        public static readonly DependencyProperty ApplyConventionsProperty =
            DependencyPropertyHelper.RegisterAttached(
                "ApplyConventions",
                typeof(bool?),
                typeof(View)
                );

        /// <summary>
        ///     A dependency property which allows the framework to track whether a certain element has already been loaded in
        ///     certain scenarios.
        /// </summary>
        public static readonly DependencyProperty IsLoadedProperty =
            DependencyPropertyHelper.RegisterAttached(
                "IsLoaded",
                typeof(bool),
                typeof(View),
                false
                );

        /// <summary>
        ///     A dependency property which marks an element as a name scope root.
        /// </summary>
        public static readonly DependencyProperty IsScopeRootProperty =
            DependencyPropertyHelper.RegisterAttached(
                "IsScopeRoot",
                typeof(bool),
                typeof(View),
                false
                );

        private static readonly ContentPropertyAttribute DefaultContentProperty = new ContentPropertyAttribute("Content");
        private static readonly ILog Log = LogManager.GetLog(typeof(View));

        /// <summary>
        ///     A dependency property for assigning a context to a particular portion of the UI.
        /// </summary>
        public static readonly DependencyProperty ContextProperty =
            DependencyPropertyHelper.RegisterAttached(
                "Context",
                typeof(object),
                typeof(View),
                null,
                OnContextChanged
                );

        /// <summary>
        ///     A dependency property for attaching a model to the UI.
        /// </summary>
        public static DependencyProperty ModelProperty =
            DependencyPropertyHelper.RegisterAttached(
                "Model",
                typeof(object),
                typeof(View),
                null,
                OnModelChanged
                );

        /// <summary>
        ///     Used by the framework to indicate that this element was generated.
        /// </summary>
        public static readonly DependencyProperty IsGeneratedProperty =
            DependencyPropertyHelper.RegisterAttached(
                "IsGenerated",
                typeof(bool),
                typeof(View),
                false
                );


        /// <summary>
        ///     Used to retrieve the root, non-framework-created view.
        /// </summary>
        /// <returns>The root element that was not created by the framework.</returns>
        /// <remarks>
        ///     In certain instances the services create UI elements.
        ///     For example, if you ask the window manager to show a UserControl as a dialog, it creates a window to host the
        ///     UserControl in.
        ///     The WindowManager marks that element as a framework-created element so that it can determine what it created vs.
        ///     what was intended by the developer.
        ///     Calling GetFirstNonGeneratedView allows the framework to discover what the original element was.
        /// </remarks>
        public static Func<object, object> GetFirstNonGeneratedView = view =>
        {
            var dependencyObject = view as DependencyObject;
            if (dependencyObject == null)
            {
                return view;
            }

            if ((bool) dependencyObject.GetValue(IsGeneratedProperty))
            {
                var o = dependencyObject as ContentControl;
                if (o != null)
                {
                    return o.Content;
                }
                var type = dependencyObject.GetType();
                var contentProperty = type.GetAttributes<ContentPropertyAttribute>(true)
                                          .FirstOrDefault() ?? DefaultContentProperty;

                return type.GetProperty(contentProperty.Name)
                           .GetValue(dependencyObject, null);
            }

            return dependencyObject;
        };


        private static bool? _inDesignMode;

        /// <summary>
        ///     Gets a value that indicates whether the process is running in design mode.
        /// </summary>
        public static bool InDesignMode
        {
            get
            {
                if (_inDesignMode != null)
                    return _inDesignMode.GetValueOrDefault(false);
                var descriptor = DependencyPropertyDescriptor.FromProperty(
                    DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement));
                _inDesignMode = (bool) descriptor.Metadata.DefaultValue;

                return _inDesignMode.GetValueOrDefault(false);
            }
        }


        /// <summary>
        ///     Executes the handler the next time the elements's LayoutUpdated event fires.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        public static void ExecuteOnLayoutUpdated(FrameworkElement element, EventHandler handler)
        {
            EventHandler onLayoutUpdate = null;
            onLayoutUpdate = (s, e) =>
            {
                element.LayoutUpdated -= onLayoutUpdate;
                handler(element, e);
            };
            element.LayoutUpdated += onLayoutUpdate;
        }

        /// <summary>
        ///     Executes the handler immediately if the element is loaded, otherwise wires it to the Loaded event.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>true if the handler was executed immediately; false otherwise</returns>
        public static bool ExecuteOnLoad(FrameworkElement element, RoutedEventHandler handler)
        {
            if (element.IsLoaded)
            {
                handler(element, new RoutedEventArgs());
                return true;
            }

            RoutedEventHandler loaded = null;
            loaded = (s, e) =>
            {
                element.Loaded -= loaded;
                handler(s, e);
            };
            element.Loaded += loaded;
            return false;
        }

        /// <summary>
        ///     Executes the handler when the element is unloaded.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        public static void ExecuteOnUnload(FrameworkElement element, RoutedEventHandler handler)
        {
            RoutedEventHandler unloaded = null;
            unloaded = (s, e) =>
            {
                element.Unloaded -= unloaded;
                handler(s, e);
            };
            element.Unloaded += unloaded;
        }

        /// <summary>
        ///     Gets the convention application behavior.
        /// </summary>
        /// <param name="d">The element the property is attached to.</param>
        /// <returns>Whether or not to apply conventions.</returns>
        public static bool? GetApplyConventions(DependencyObject d)
        {
            return (bool?) d.GetValue(ApplyConventionsProperty);
        }

        /// <summary>
        ///     Gets the context.
        /// </summary>
        /// <param name="d">The element the context is attached to.</param>
        /// <returns>The context.</returns>
        public static object GetContext(DependencyObject d)
        {
            return d.GetValue(ContextProperty);
        }

        /// <summary>
        ///     Gets the model.
        /// </summary>
        /// <param name="d">The element the model is attached to.</param>
        /// <returns>The model.</returns>
        public static object GetModel(DependencyObject d)
        {
            return d.GetValue(ModelProperty);
        }

        /// <summary>
        ///     Sets the convention application behavior.
        /// </summary>
        /// <param name="d">The element to attach the property to.</param>
        /// <param name="value">Whether or not to apply conventions.</param>
        public static void SetApplyConventions(DependencyObject d, bool? value)
        {
            d.SetValue(ApplyConventionsProperty, value);
        }

        public static bool SetContentPropertyCore(object targetLocation, object view)
        {
            try
            {
                var type = targetLocation.GetType();
                var contentProperty = type.GetAttributes<ContentPropertyAttribute>(true)
                                          .FirstOrDefault() ?? DefaultContentProperty;

                type.GetProperty(contentProperty.Name)
                    .SetValue(targetLocation, view, null);

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e);

                return false;
            }
        }

        /// <summary>
        ///     Sets the context.
        /// </summary>
        /// <param name="d">The element to attach the context to.</param>
        /// <param name="value">The context.</param>
        public static void SetContext(DependencyObject d, object value)
        {
            d.SetValue(ContextProperty, value);
        }

        /// <summary>
        ///     Sets the model.
        /// </summary>
        /// <param name="d">The element to attach the model to.</param>
        /// <param name="value">The model.</param>
        public static void SetModel(DependencyObject d, object value)
        {
            d.SetValue(ModelProperty, value);
        }

        private static void OnContextChanged(DependencyObject targetLocation, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == e.NewValue)
            {
                return;
            }

            var model = GetModel(targetLocation);
            if (model == null)
            {
                return;
            }

            var view = ViewLocator.LocateForModel(model, targetLocation, e.NewValue);

            if (!SetContentProperty(targetLocation, view))
            {
                Log.Warn("SetContentProperty failed for ViewLocator.LocateForModel, falling back to LocateForModelType");

                view = ViewLocator.LocateForModelType(model.GetType(), targetLocation, e.NewValue);

                SetContentProperty(targetLocation, view);
            }

            ViewModelBinder.Bind(model, view, e.NewValue);
        }

        private static void OnModelChanged(DependencyObject targetLocation, DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue == args.NewValue)
            {
                return;
            }

            if (args.NewValue != null)
            {
                var context = GetContext(targetLocation);

                var view = ViewLocator.LocateForModel(args.NewValue, targetLocation, context);

                if (!SetContentProperty(targetLocation, view))
                {
                    Log.Warn(
                        "SetContentProperty failed for ViewLocator.LocateForModel, falling back to LocateForModelType");

                    view = ViewLocator.LocateForModelType(args.NewValue.GetType(), targetLocation, context);

                    SetContentProperty(targetLocation, view);
                }

                ViewModelBinder.Bind(args.NewValue, view, context);
            }
            else
            {
                SetContentProperty(targetLocation, args.NewValue);
            }
        }

        private static bool SetContentProperty(object targetLocation, object view)
        {
            var fe = view as FrameworkElement;
            if (fe?.Parent != null)
            {
                SetContentPropertyCore(fe.Parent, null);
            }

            return SetContentPropertyCore(targetLocation, view);
        }
    }
}