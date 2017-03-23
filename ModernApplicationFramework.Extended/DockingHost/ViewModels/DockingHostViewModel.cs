using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.DockingHost.ViewModels
{
    [Export(typeof(IDockingHostViewModel))]
    public class DockingHostViewModel : Conductor<ILayoutItem>.Collection.OneActive, IDockingHostViewModel
    {
        private readonly BindableCollection<ITool> _tools;
        private ILayoutItemBase _activeLayoutItemBase;
        private bool _closing;
        private IDockingHost _dockingHostView;
#pragma warning disable 649
        [Import] private ILayoutItemStatePersister _layoutItemStatePersister;

        [ImportMany(typeof(IModule))] private IEnumerable<IModule> _modules;
#pragma warning disable 649
        private bool _showFloatingWindowsInTaskbar;

        public DockingHostViewModel()
        {
            ((IActivate) this).Activate();
            _tools = new BindableCollection<ITool>();
        }

        public event EventHandler ActiveDocumentChanged;
        public event EventHandler ActiveDocumentChanging;

        public ILayoutItemBase ActiveLayoutItemBase
        {
            get => _activeLayoutItemBase;
            set
            {
                if (ReferenceEquals(_activeLayoutItemBase, value))
                    return;

                _activeLayoutItemBase = value;

                var item = value as ILayoutItem;
                if (item != null)
                    ActivateItem(item);

                NotifyOfPropertyChange(() => ActiveLayoutItemBase);
            }
        }

        public void Close()
        {
            Application.Current.MainWindow.Close();
        }

        public void CloseDocument(ILayoutItem document)
        {
            DeactivateItem(document, true);
        }

        public IObservableCollection<ILayoutItem> Documents => Items;

        public void OpenDocument(ILayoutItem model)
        {
            ActivateItem(model);
        }

        public bool ShowFloatingWindowsInTaskbar
        {
            get => _showFloatingWindowsInTaskbar;
            set
            {
                _showFloatingWindowsInTaskbar = value;
                NotifyOfPropertyChange(() => ShowFloatingWindowsInTaskbar);
                _dockingHostView?.UpdateFloatingWindows();
            }
        }

        public void ShowTool<TTool>()
            where TTool : ITool
        {
            ShowTool(IoC.Get<TTool>());
        }

        public void ShowTool(ITool model)
        {
            if (Tools.Contains(model))
                model.IsVisible = true;
            else
                Tools.Add(model);
            model.IsSelected = true;
            ActiveLayoutItemBase = model;
        }

        public IObservableCollection<ITool> Tools => _tools;

        public bool HasPersistedState => File.Exists(StateFile);

        public virtual string StateFile => @".\ApplicationState.bin";

        public override void ActivateItem(ILayoutItem item)
        {
            if (_closing)
                return;

            RaiseActiveDocumentChanging();

            var currentActiveItem = ActiveItem;

            base.ActivateItem(item);

            if (!ReferenceEquals(item, currentActiveItem))
                RaiseActiveDocumentChanged();
        }

        public override void DeactivateItem(ILayoutItem item, bool close)
        {
            RaiseActiveDocumentChanging();

            base.DeactivateItem(item, close);

            RaiseActiveDocumentChanged();
        }

        protected override void OnActivationProcessed(ILayoutItem item, bool success)
        {
            if (!ReferenceEquals(ActiveLayoutItemBase, item))
                ActiveLayoutItemBase = item;

            base.OnActivationProcessed(item, success);
        }

        protected override void OnDeactivate(bool close)
        {
            // Workaround for a complex bug that occurs when
            // (a) the window has multiple documents open, and
            // (b) the last document is NOT active
            // 
            // The issue manifests itself with a crash in
            // the call to base.ActivateItem(itemBase), above,
            // saying that the collection can't be changed
            // in a CollectionChanged event handler.
            // 
            // The issue occurs because:
            // - Caliburn.Micro sees the window is closing, and calls Items.Clear()
            // - AvalonDock handles the CollectionChanged event, and calls Remove()
            //   on each of the open documents.
            // - If removing a document causes another to become active, then AvalonDock
            //   sets a new ActiveContent.
            // - We have a WPF binding from Caliburn.Micro's ActiveItem to AvalonDock's
            //   ActiveContent property, so ActiveItem gets updated.
            // - The document no longer exists in Items, beacuse that collection was cleared,
            //   but Caliburn.Micro helpfully adds it again - which causes the crash.
            //
            // My workaround is to use the following _closing variable, and ignore activation
            // requests that occur when _closing is true.
            _closing = true;

            _layoutItemStatePersister.SaveState(this, _dockingHostView, StateFile);

            base.OnDeactivate(close);
        }

        protected override void OnViewLoaded(object view)
        {
            foreach (var module in _modules)
            foreach (var globalResourceDictionary in module.GlobalResourceDictionaries)
                Application.Current.Resources.MergedDictionaries.Add(globalResourceDictionary);

            foreach (var module in _modules)
                module.PreInitialize();
            foreach (var module in _modules)
                module.Initialize();


            _dockingHostView = (IDockingHost) view;
            if (!HasPersistedState)
            {
                foreach (var defaultDocument in _modules.SelectMany(x => x.DefaultDocuments))
                    OpenDocument(defaultDocument);
                foreach (var defaultTool in _modules.SelectMany(x => x.DefaultTools))
                    ShowTool((ITool) IoC.GetInstance(defaultTool, null));
            }
            else
            {
                _layoutItemStatePersister.LoadState(this, _dockingHostView, StateFile);
            }

            foreach (var module in _modules)
                module.PostInitialize();
            base.OnViewLoaded(view);
        }

        private void RaiseActiveDocumentChanged()
        {
            var handler = ActiveDocumentChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseActiveDocumentChanging()
        {
            var handler = ActiveDocumentChanging;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}