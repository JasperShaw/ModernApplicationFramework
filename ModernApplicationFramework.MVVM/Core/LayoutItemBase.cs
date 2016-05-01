using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Core
{
    public abstract class LayoutItemBase : Screen, ILayoutItem
    {
        private bool _isSelected;
        public abstract ICommand CloseCommand { get; }

        [Browsable(false)]
        public virtual string ContentId => Id.ToString();

        [Browsable(false)]
        public virtual Uri IconSource => null;

        [Browsable(false)]
        public Guid Id { get; } = Guid.NewGuid();

        [Browsable(false)]
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public virtual void LoadState(BinaryReader reader) {}

        public virtual void SaveState(BinaryWriter writer) {}

        [Browsable(false)]
        public virtual bool ShouldReopenOnStart => false;
    }
}