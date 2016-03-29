using System.Windows.Input;
using ModernApplicationFramework.Docking.Commands;
using ModernApplicationFramework.MVVM.Core;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Controls
{
    public abstract class Tool : LayoutItemBase, ITool
    {
        private ICommand _closeCommand;

        private bool _isVisible;

        protected Tool()
        {
            IsVisible = true;
        }

        public override ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(p => IsVisible = false, p => true)); }
        }

        public override bool ShouldReopenOnStart => true;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                NotifyOfPropertyChange(() => IsVisible);
            }
        }

        public virtual double PreferredHeight => 200;

        public abstract PaneLocation PreferredLocation { get; }

        public virtual double PreferredWidth => 200;
    }
}