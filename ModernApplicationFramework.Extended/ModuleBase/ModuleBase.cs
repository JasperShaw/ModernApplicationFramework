using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.ModuleBase
{
    public abstract class ModuleBase : IModule
    {
        protected IDockingHostViewModel DockingHostViewModel => _dockingHostViewModel;

        protected IDockingMainWindowViewModel MainWindow => _useDockingHost;

        public virtual IEnumerable<ILayoutItem> DefaultDocuments
        {
            get { yield break; }
        }

        public virtual IEnumerable<Type> DefaultTools
        {
            get { yield break; }
        }


        public virtual IEnumerable<ResourceDictionary> GlobalResourceDictionaries
        {
            get { yield break; }
        }

        public virtual void Initialize()
        {
        }

        public virtual void PostInitialize()
        {
        }

        public virtual void PreInitialize()
        {
        }

#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _useDockingHost;

        [Import] private IDockingHostViewModel _dockingHostViewModel;
#pragma warning restore 649
    }
}