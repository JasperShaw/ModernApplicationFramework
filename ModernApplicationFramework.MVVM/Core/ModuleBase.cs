using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Core
{
    public abstract class ModuleBase : IModule
    {
#pragma warning disable 649
        [Import]
        private IMainWindowViewModel _mainWindowViewModel;

        [Import]
        private IDockingHostViewModel _dockingHostViewModel;
#pragma warning restore 649

        protected IMainWindowViewModel MainWindow => _mainWindowViewModel;

        protected IDockingHostViewModel DockingHostViewModel => _dockingHostViewModel;


        public virtual IEnumerable<ResourceDictionary> GlobalResourceDictionaries
        {
            get { yield break; }
        }

        public virtual IEnumerable<IDocument> DefaultDocuments
        {
            get { yield break; }
        }

        public virtual IEnumerable<Type> DefaultTools
        {
            get { yield break; }
        }

        public virtual void PreInitialize()
        {

        }

        public virtual void Initialize()
        {

        }

        public virtual void PostInitialize()
        {

        }
    }
}
