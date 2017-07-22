using System.Windows.Input;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Core.Pane;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Core.LayoutItems
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
            get { return _closeCommand ?? (_closeCommand = new ObjectCommand(p => IsVisible = false, p => true)); }
        }

        public override bool ShouldReopenOnStart => true;

        public bool IsVisible
        {
            get => _isVisible;
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