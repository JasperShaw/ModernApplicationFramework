using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.UndoRedoManager;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.Layout
{
    public abstract class LayoutItemBase : Screen, ILayoutItemBase
    {
        private bool _isSelected;

        private string _toolTip;
        private ImageSource _iconSource;
        private LayoutContent _frame;
        private IUndoRedoManager _undoRedoManager;

        [Browsable(false)]
        public virtual string ContentId => Id.ToString();

        [Browsable(false)]
        public Guid Id { get; } = Guid.NewGuid();

        [Browsable(false)]
        public virtual bool ShouldReopenOnStart => false;

        public string ToolTip
        {
            get => _toolTip;
            set
            {
                if (value == _toolTip) return;
                _toolTip = value;
                NotifyOfPropertyChange();
            }
        }

        [Browsable(false)]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public virtual Guid ContextMenuId => Guid.Empty;

        public virtual ImageSource IconSource
        {
            get => _iconSource;
            set
            {
                if (Equals(value, _iconSource)) return;
                _iconSource = value;
                NotifyOfPropertyChange();
            }
        }

        public ICommand RedoCommand => IoC.Get<IRedoCommand>();

        public ICommand UndoCommand => IoC.Get<IUndoCommand>();

        public IUndoRedoManager UndoRedoManager
        {
            get => _undoRedoManager ?? (_undoRedoManager = new Basics.UndoRedoManager.UndoRedoManager());
            protected set => _undoRedoManager = value;
        }

        public virtual void LoadState(BinaryReader reader)
        {
        }

        public virtual void SaveState(BinaryWriter writer)
        {
        }


        public override bool Equals(object obj)
        {
            if (!(obj is ILayoutItemBase layoutItem))
                return false;
            return Equals(layoutItem);
        }

        public bool Equals(ILayoutItemBase layoutItem)
        {
            return Id.Equals(layoutItem.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            if (view == null)
            {
                _frame = null;
                return;
            }
            if (!(view is UIElement uiElement))
                return;
            _frame = uiElement.FindLogicalAncestor<LayoutAnchorableControl>()?.Model ??
                     uiElement.FindLogicalAncestor<LayoutDocumentControl>()?.Model;
            if (_frame == null)
                return;
            _frame.Closing += _frame_Closing;
            _frame.Closed += _frame_Closed;
        }

        protected virtual void PushUndoRedoManager(string sender, object value)
        {
            UndoRedoManager.Push(new UndoRedoAction(this, sender, value));
        }

        protected Docking.Controls.LayoutItem DockingModel
        {
            get
            {
                var element = GetView() as UIElement;
                return element.FindLogicalAncestor<LayoutAnchorableControl>()?.LayoutItem ??
                       element.FindLogicalAncestor<LayoutDocumentControl>()?.LayoutItem;
            }
        }


        protected virtual void OnClosing(CancelEventArgs e)
        {
        }

        protected virtual void OnClosed()
        {
            _frame.Closing -= _frame_Closing;
            _frame.Closed -= _frame_Closed;
        }


        private void _frame_Closed(object sender, EventArgs e)
        {
            OnClosed();
        }

        private void _frame_Closing(object sender, CancelEventArgs e)
        {
            OnClosing(e);
        }
    }
}