using System;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Extended.Controls.DockingHost.Views;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Package;

namespace ModernApplicationFramework.Extended.Controls.DockingHost.ViewModels
{
    [Export(typeof(IDockingHostViewModel))]
    public class DockingHostViewModel : Conductor<ILayoutItem>.Collection.OneActive, IDockingHostViewModel
    {
        public event EventHandler<ILayoutItem> ActiveLayoutItemChanged;
        public event EventHandler<ILayoutItem> ActiveLayoutItemChanging;


        private readonly BindableCollection<ITool> _tools;
        private ILayoutItemBase _activeLayoutItemBase;
        private bool _closing;

        private bool _showFloatingWindowsInTaskbar;

        public IObservableCollection<ITool> Tools => _tools;

        public ILayoutItemBase ActiveLayoutItemBase
        {
            get => _activeLayoutItemBase;
            set
            {
                if (ReferenceEquals(_activeLayoutItemBase, value))
                    return;
                _activeLayoutItemBase = value;
                if (value is ILayoutItem item)
                    ActivateItem(item);
                NotifyOfPropertyChange(() => ActiveLayoutItemBase);
            }
        }

        public bool ShowFloatingWindowsInTaskbar
        {
            get => _showFloatingWindowsInTaskbar;
            set
            {
                _showFloatingWindowsInTaskbar = value;
                NotifyOfPropertyChange(() => ShowFloatingWindowsInTaskbar);
                DockingHostView?.UpdateFloatingWindows();
            }
        }

        public DockingHostViewModel()
        {
            ((IActivate) this).Activate();
            _tools = new BindableCollection<ITool>();
        }

        public virtual void Close()
        {
            Application.Current.MainWindow?.Close();
        }

        public virtual bool CloseLayoutItem(ILayoutItem document)
        {
            if (DockingHostView is IInternalDockingHost internalDocking && internalDocking.RaiseDocumentClosing(document))
                return false;
            DeactivateItem(document, true);
            return true;
        }

        public IDockingHost DockingHostView { get; protected set; }

        public IObservableCollection<ILayoutItem> LayoutItems => Items;

        public void OpenLayoutItem(ILayoutItem model)
        {
            ActivateItem(model);
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

        public void HideTool<TTool>(bool remove)
            where TTool : ITool
        {
            HideTool(IoC.Get<TTool>(), remove);
        }

        public void HideTool(ITool model, bool remove)
        {
            if (Tools.Contains(model))
                model.IsVisible = false;
            else
                return;
            if (!remove)
                return;
            Tools.Remove(model);
        }

        public bool ContainsLayoutItem(ILayoutItem layoutItem)
        {
            return LayoutItems.Contains(layoutItem);
        }

        public override void ActivateItem(ILayoutItem item)
        {
            if (_closing)
                return;
            RaiseActiveDocumentChanging(item);
            var currentActiveItem = ActiveItem;
            base.ActivateItem(item);
            if (!ReferenceEquals(item, currentActiveItem))
                RaiseActiveDocumentChanged(item);
        }

        public override void DeactivateItem(ILayoutItem item, bool close)
        {
            RaiseActiveDocumentChanging(item);

            base.DeactivateItem(item, close);

            RaiseActiveDocumentChanged(item);
        }

        protected override void OnActivationProcessed(ILayoutItem item, bool success)
        {
            if (!ReferenceEquals(ActiveLayoutItemBase, item))
                ActiveLayoutItemBase = item;

            base.OnActivationProcessed(item, success);
        }

        protected sealed override void OnDeactivate(bool close)
        {
            // Workaround for a complex _bug that occurs when
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
            // - The document no longer exists in Items, because that collection was cleared,
            //   but Caliburn.Micro helpfully adds it again - which causes the crash.
            //
            // My workaround is to use the following _closing variable, and ignore activation
            // requests that occur when _closing is true.
            _closing = true;
            PreviewDeactivating(close);
            PackageManager.Instance.ClosePackages(PackageCloseOption.OnMainWindowClosed);
            base.OnDeactivate(close);
        }

        protected virtual void PreviewDeactivating(bool close)
        {
            
        }

        protected override void OnViewAttached(object view, object context)
        {
            DockingHostView = (IDockingHost)view;
            DockingHostView.LayoutItemsClosed += DockingHostViewLayoutItemsClosed;
            PackageManager.Instance.LoadPackages(PackageLoadOption.PreviewWindowLoaded);
            base.OnViewAttached(view, context);
        }

        private void DockingHostViewLayoutItemsClosed(object sender, LayoutItemsClosedEventArgs e)
        {
            e.LayoutItems.ForEach(x => DeactivateItem(x, true));
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            PackageManager.Instance.LoadPackages(PackageLoadOption.OnMainWindowLoaded);
        }

        private void RaiseActiveDocumentChanged(ILayoutItem item)
        {
            var handler = ActiveLayoutItemChanged;
            handler?.Invoke(this, item);
        }

        private void RaiseActiveDocumentChanging(ILayoutItem item)
        {
            var handler = ActiveLayoutItemChanging;
            handler?.Invoke(this, item);
        }
    }
}