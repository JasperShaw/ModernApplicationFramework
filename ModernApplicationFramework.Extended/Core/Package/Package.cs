using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Core.Package
{
    public abstract class Package : DisposableObject, IMafPackage
    {
        protected IDockingHostViewModel DockingHostViewModel => _dockingHostViewModel;

        protected IDockingMainWindowViewModel MainWindow => _useDockingHost;

        public bool Initialized { get; private set; }

        public abstract Guid Id { get; }

        public virtual IEnumerable<Type> DefaultTools
        {
            get { yield break; }
        }

        public virtual IEnumerable<ResourceDictionary> GlobalResourceDictionaries
        {
            get { yield break; }
        }

        public void Close()
        {
            foreach (var defaultTool in DefaultTools)
            {
                if (!defaultTool.IsAssignableFrom(typeof(ITool)))
                    continue;
                if (IoC.GetInstance(defaultTool, null) is ITool tool)
                    DockingHostViewModel.HideTool(tool, true);
            }
            Dispose(true);
        }

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

    public interface IMafPackage
    {
        bool Initialized { get; }

        Guid Id { get; }

        IEnumerable<Type> DefaultTools { get; }

        IEnumerable<ResourceDictionary> GlobalResourceDictionaries { get; }

        void Close();

        void Initialize();
    }
}
