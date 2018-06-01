using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Docking;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Package;

namespace ModernApplicationFramework.Extended.Controls.DockingHost.ViewModels
{
    [Export(typeof(IDockingHostViewModel))]
    public class DockingHostViewModel : Conductor<ILayoutItem>.Collection.OneActive, IDockingHostViewModel
    {
        public event EventHandler<LayoutChangeEventArgs> ActiveLayoutItemChanged;
        public event EventHandler<LayoutChangeEventArgs> ActiveLayoutItemChanging;
        public event EventHandler<LayoutItemsClosingEventArgs> LayoutItemsClosing;
        public event EventHandler<LayoutItemsClosedEventArgs> LayoutItemsClosed;
        public event EventHandler<ToolsClosingEventArgs> ToolsClosing; 
        public event EventHandler<ToolsClosedEventArgs> ToolsClosed; 


        private readonly BindableCollection<ITool> _tools;
        private ILayoutItemBase _activeLayoutItemBase;
        private bool _closing;

        private bool _showFloatingWindowsInTaskbar;
        private bool _changingItem;

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
                    OpenLayoutItem(item);
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

        public IReadOnlyList<ILayoutItemBase> AllOpenLayoutItemsAsDocuments
        {
            get
            {
                return DockingHostView.DockingManager.AllOpenDocuments.Where(x => x.Content is ILayoutItemBase).Select(x => x.Content)
                    .OfType<ILayoutItemBase>().ToList();
            }
        }

        public DockingHostViewModel()
        {
            ((IActivate)this).Activate();
            _tools = new BindableCollection<ITool>();
        }

        public virtual void Close()
        {
            Application.Current.MainWindow?.Close();
        }

        protected override void ChangeActiveItem(ILayoutItem newItem, bool closePrevious)
        {
            if (_closing || _changingItem)
                return;
            _changingItem = true;
            var currentActiveItem = ActiveItem;
            RaiseActiveDocumentChanging(ActiveItem, newItem);
            base.ChangeActiveItem(newItem, closePrevious);
            if (!ReferenceEquals(newItem, currentActiveItem))
                RaiseActiveDocumentChanged(currentActiveItem, newItem);
            _changingItem = false;
        }

        public virtual bool CloseLayoutItem(ILayoutItem document)
        {
            DeactivateItem(document, true);
            return !Items.Contains(document);
        }

        public void LoadLayout(Stream stream, Action<ITool> addToolCallback, Action<ILayoutItem> addDocumentCallback,
            Dictionary<string, ILayoutItemBase> itemsState)
        {
            LayoutUtilities.LoadLayout(DockingHostView.DockingManager, stream, addDocumentCallback, addToolCallback, itemsState);
        }

        public void SaveLayout(Stream stream)
        {
            LayoutUtilities.SaveLayout(DockingHostView.DockingManager, stream);
        }

        protected IDockingHost DockingHostView { get; set; }

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
            if (_closing || _changingItem)
                return;
            base.ActivateItem(item);

        }

        public override void DeactivateItem(ILayoutItem item, bool close)
        {
            item.TryClose();
        }

        protected virtual void PreviewClosing(bool close)
        {

        }

        protected virtual void RaiseActiveDocumentChanged(ILayoutItem oldItem, ILayoutItem newItem)
        {
            ActiveLayoutItemChanged?.Invoke(this, new LayoutChangeEventArgs(oldItem, newItem));
        }

        protected virtual void RaiseActiveDocumentChanging(ILayoutItem oldItem, ILayoutItem newItem)
        {
            ActiveLayoutItemChanging?.Invoke(this, new LayoutChangeEventArgs(oldItem, newItem));
        }

        protected virtual void OnLayoutItemsClosing(LayoutItemsClosingEventArgs e)
        {
            LayoutItemsClosing?.Invoke(this, e);
        }

        protected virtual void OnLayoutItemsClosed(LayoutItemsClosedEventArgs e)
        {
            e.LayoutItems.ForEach(x => base.DeactivateItem(x, true));
            LayoutItemsClosed?.Invoke(this, e);
        }

        protected virtual void OnToolsClosing(ToolsClosingEventArgs eventArgs)
        {
            ToolsClosing?.Invoke(this, eventArgs);
        }

        protected virtual void OnToolsClosed(ToolsClosedEventArgs eventArgs)
        {
            ToolsClosed?.Invoke(this, eventArgs);
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
            PreviewClosing(close);
            PackageManager.Instance.ClosePackages(PackageCloseOption.OnMainWindowClosed);
            base.OnDeactivate(close);
        }

        protected override void OnViewAttached(object view, object context)
        {
            DockingHostView = (IDockingHost) view;
            DockingHostView.DockingManager.DocumentsClosing += InternalDockingHost_LayoutItemsClosing;
            DockingHostView.DockingManager.DocumentsClosed += DockingHostViewLayoutItemsClosed;
            DockingHostView.DockingManager.AnchorablesClosing += DockingManager_AnchorablesClosing;
            DockingHostView.DockingManager.AnchorablesClosed += DockingManager_AnchorablesClosed;
            PackageManager.Instance.LoadPackages(PackageLoadOption.PreviewWindowLoaded);
            base.OnViewAttached(view, context);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            PackageManager.Instance.LoadPackages(PackageLoadOption.OnMainWindowLoaded);
        }

    
        private void DockingHostViewLayoutItemsClosed(object sender, DocumentsClosedEventArgs e)
        {
            var layoutItems = new List<ILayoutItem>();
            foreach (var layoutDocument in e.Documents)
                if (layoutDocument.Content is ILayoutItem layoutItem)
                    layoutItems.Add(layoutItem);
            var eventArgs = new LayoutItemsClosedEventArgs(layoutItems);
            OnLayoutItemsClosed(eventArgs);
        }

        private void InternalDockingHost_LayoutItemsClosing(object sender, DocumentsClosingEventArgs e)
        {
            var layoutItems = new List<ILayoutItem>();
            foreach (var layoutDocument in e.Documents)
            {
                if (layoutDocument.Content is ILayoutItem layoutItem)
                    layoutItems.Add(layoutItem);
            }
            var eventArgs = new LayoutItemsClosingEventArgs(layoutItems);
            OnLayoutItemsClosing(eventArgs);
            e.Cancel = eventArgs.Cancel;
        }

        private void DockingManager_AnchorablesClosing(object sender, AnchorablesClosingEventArgs e)
        {
            var tools = new List<ITool>();
            foreach (var anchor in e.Anchors)
            {
                if (anchor.Content is ITool tool)
                    tools.Add(tool);
            }
            var eventArgs = new ToolsClosingEventArgs(tools, e.Mode);
            OnToolsClosing(eventArgs);
            e.Cancel = eventArgs.Cancel;
        }

        private void DockingManager_AnchorablesClosed(object sender, AnchorablesClosedEventArgs e)
        {
            var tools = new List<ITool>();
            foreach (var anchor in e.Anchors)
                if (anchor.Content is ITool tool)
                    tools.Add(tool);
            var eventArgs = new ToolsClosedEventArgs(tools);
            OnToolsClosed(eventArgs);
        }
    }
}