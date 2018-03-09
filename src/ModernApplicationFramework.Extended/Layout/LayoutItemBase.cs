using System;
using System.ComponentModel;
using System.IO;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Layout
{
    public abstract class LayoutItemBase : Screen, ILayoutItemBase
    {
        private bool _isSelected;

        private string _toolTip;
        //public abstract ICommand CloseCommand { get; }

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
    }
}