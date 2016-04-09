﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ModernApplicationFramework.Caliburn.Extensions;
using ModernApplicationFramework.Caliburn.Interfaces;
using ModernApplicationFramework.Caliburn.Platform.Core;
using ModernApplicationFramework.Caliburn.Platform.Xaml;
using ModernApplicationFramework.Caliburn.Result;

namespace ModernApplicationFramework.Caliburn.Platform.Utilities
{
    public abstract class BootstrapperBase
    {
        private readonly bool _useApplication;
        private bool _isInitialized;

        /// <summary>
        ///     Creates an instance of the bootstrapper.
        /// </summary>
        /// <param name="useApplication">
        ///     Set this to false when hosting Caliburn.Micro inside and Office or WinForms application.
        ///     The default is true.
        /// </param>
        protected BootstrapperBase(bool useApplication = true)
        {
            _useApplication = useApplication;
        }

        /// <summary>
        ///     The application.
        /// </summary>
        protected Application Application { get; set; }

        /// <summary>
        ///     Initialize the framework.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;

            PlatformProvider.PlatformProvider.Current = new XamlPlatformProvider();

            var baseExtractTypes = AssemblySourceCache.ExtractTypes;

            AssemblySourceCache.ExtractTypes = assembly =>
            {
                var baseTypes = baseExtractTypes(assembly);
                var elementTypes = assembly.GetExportedTypes()
                                           .Where(t => typeof(UIElement).IsAssignableFrom(t));

                return baseTypes.Union(elementTypes);
            };

            AssemblySource.Instance.Refresh();

            if (Execute.InDesignMode)
            {
                try
                {
                    StartDesignTime();
                }
                catch
                {
                    //if something fails at design-time, there's really nothing we can do...
                    _isInitialized = false;
                    throw;
                }
            }
            else
            {
                StartRuntime();
            }
        }

        /// <summary>
        ///     Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="instance">The instance to perform injection on.</param>
        protected virtual void BuildUp(object instance) {}

        /// <summary>
        ///     Override to configure the framework and setup your IoC container.
        /// </summary>
        protected virtual void Configure() {}

        /// <summary>
        ///     Override this to provide an IoC specific implementation
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <returns>The located services.</returns>
        protected virtual IEnumerable<object> GetAllInstances(Type service)
        {
            return new[] {Activator.CreateInstance(service)};
        }

        /// <summary>
        ///     Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <param name="key">The key to locate.</param>
        /// <returns>The located service.</returns>
        protected virtual object GetInstance(Type service, string key)
        {
            if (service == typeof(IWindowManager))
                service = typeof(WindowManager);
            return Activator.CreateInstance(service);
        }

        /// <summary>
        ///     Override this to add custom behavior on exit.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        protected virtual void OnExit(object sender, System.EventArgs e) {}

        /// <summary>
        ///     Override this to add custom behavior to execute after the application starts.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The args.</param>
        protected virtual void OnStartup(object sender, StartupEventArgs e) {}


        /// <summary>
        ///     Override this to add custom behavior for unhandled exceptions.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        protected virtual void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {}

        /// <summary>
        ///     Provides an opportunity to hook into the application object.
        /// </summary>
        protected virtual void PrepareApplication()
        {
            Application.Startup += OnStartup;
            Application.DispatcherUnhandledException += OnUnhandledException;
            Application.Exit += OnExit;
        }

        /// <summary>
        ///     Override to tell the framework where to find assemblies to inspect for views, etc.
        /// </summary>
        /// <returns>A list of assemblies to inspect.</returns>
        protected virtual IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] {GetType().Assembly};
        }

        /// <summary>
        ///     Called by the bootstrapper's constructor at design time to start the framework.
        /// </summary>
        protected virtual void StartDesignTime()
        {
            AssemblySource.Instance.Clear();
            AssemblySource.Instance.AddRange(SelectAssemblies());

            Configure();
            IoC.GetInstance = GetInstance;
            IoC.GetAllInstances = GetAllInstances;
            IoC.BuildUp = BuildUp;
        }

        /// <summary>
        ///     Called by the bootstrapper's constructor at runtime to start the framework.
        /// </summary>
        protected virtual void StartRuntime()
        {
            EventAggregator.HandlerResultProcessing = (target, result) =>
            {
                var task = result as Task;
                if (task != null)
                {
                    result = new IResult[] {task.AsResult()};
                }

                var coroutine = result as IEnumerable<IResult>;
                if (coroutine != null)
                {
                    var viewAware = target as IViewAware;
                    var view = viewAware?.GetView();
                    var context = new CoroutineExecutionContext {Target = target, View = view};

                    Coroutine.BeginExecute(coroutine.GetEnumerator(), context);
                }
            };

            AssemblySourceCache.Install();
            AssemblySource.Instance.AddRange(SelectAssemblies());

            if (_useApplication)
            {
                Application = Application.Current;
                PrepareApplication();
            }

            Configure();
            IoC.GetInstance = GetInstance;
            IoC.GetAllInstances = GetAllInstances;
            IoC.BuildUp = BuildUp;
        }

        /// <summary>
        ///     Locates the view model, locates the associate view, binds them and shows it as the root view.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <param name="settings">The optional window settings.</param>
        protected void DisplayRootViewFor(Type viewModelType, IDictionary<string, object> settings = null)
        {
            var windowManager = IoC.Get<IWindowManager>();
            windowManager.ShowWindow(IoC.GetInstance(viewModelType, null), null, settings);
        }

        /// <summary>
        ///     Locates the view model, locates the associate view, binds them and shows it as the root view.
        /// </summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="settings">The optional window settings.</param>
        protected void DisplayRootViewFor<TViewModel>(IDictionary<string, object> settings = null)
        {
            DisplayRootViewFor(typeof(TViewModel), settings);
        }
    }
}