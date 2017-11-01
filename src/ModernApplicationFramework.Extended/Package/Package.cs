using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Package
{
    /// <summary>
    /// <see cref="Package"/>s are modules that extend the application by providing UI elements, services, etc.
    /// </summary>
    /// <seealso cref="ModernApplicationFramework.Utilities.DisposableObject" />
    /// <seealso cref="IMafPackage" />
    public abstract class Package : DisposableObject, IMafPackage
    {
        /// <summary>
        /// The docking shell of the application
        /// </summary>
        protected IDockingHostViewModel DockingHostViewModel => _dockingHostViewModel;

        /// <summary>
        /// The Main Window of the application
        /// </summary>
        protected IDockingMainWindowViewModel MainWindow => _useDockingHost;

        /// <summary>
        /// The load option.
        /// </summary>
        public abstract PackageLoadOption LoadOption { get; }

        /// <summary>
        /// The close option.
        /// </summary>
        public abstract PackageCloseOption CloseOption { get; }

        /// <inheritdoc />
        /// <summary>
        /// Gets a value indicating whether this <see cref="T:ModernApplicationFramework.Extended.Package.IMafPackage" /> is initialized.
        /// </summary>
        public bool Initialized { get; private set; }

        /// <inheritdoc />
        /// <summary>
        /// The identifier.
        /// </summary>
        public abstract Guid Id { get; }

        /// <inheritdoc />
        /// <summary>
        /// The default tools of this <see cref="T:ModernApplicationFramework.Extended.Package.IMafPackage" />.
        /// </summary>
        public virtual IEnumerable<Type> DefaultTools
        {
            get { yield break; }
        }

        /// <inheritdoc />
        /// <summary>
        /// Global resource dictionaries of the <see cref="T:ModernApplicationFramework.Extended.Package.IMafPackage" />.
        /// </summary>
        public virtual IEnumerable<ResourceDictionary> GlobalResourceDictionaries
        {
            get { yield break; }
        }

        /// <inheritdoc />
        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            if (IsDisposed)
                return;
            foreach (var defaultTool in DefaultTools)
            {
                if (!defaultTool.IsAssignableFrom(typeof(ITool)))
                    continue;
                if (IoC.GetInstance(defaultTool, null) is ITool tool)
                    DockingHostViewModel.HideTool(tool, true);
            }
            Dispose(true);
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Initialize()
        {
            foreach (var globalResourceDictionary in GlobalResourceDictionaries)
                Application.Current.Resources.MergedDictionaries.Add(globalResourceDictionary);
            Initialized = true;      
        }

#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _useDockingHost;

        [Import] private IDockingHostViewModel _dockingHostViewModel;
#pragma warning restore 649
    }
}
