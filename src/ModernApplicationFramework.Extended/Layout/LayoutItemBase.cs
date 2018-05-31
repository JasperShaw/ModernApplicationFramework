using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Layout
{
    public abstract class LayoutItemBase : Screen, ILayoutItemBase
    {
        private bool _isSelected;

        private string _toolTip;
        private ImageSource _iconSource;
        private LayoutContent _frame;

        [Browsable(false)]
        public virtual string ContentId => Id.ToString();

        [Browsable(false)]
        public Guid Id { get; } = Guid.NewGuid();

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

        protected Docking.Controls.LayoutItem DockingModel
        {
            get
            {
                var element = GetView() as UIElement;
                return element.FindLogicalAncestor<LayoutAnchorableControl>()?.LayoutItem ??
                         element.FindLogicalAncestor<LayoutDocumentControl>()?.LayoutItem;
            }
        }

        public virtual void LoadState(BinaryReader reader)
        {
        }

        public virtual void SaveState(BinaryWriter writer)
        {
        }

        [Browsable(false)]
        public virtual bool ShouldReopenOnStart => false;

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