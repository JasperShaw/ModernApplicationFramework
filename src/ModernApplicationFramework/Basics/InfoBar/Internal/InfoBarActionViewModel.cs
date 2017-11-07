using System.Windows.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Controls.InfoBar;

namespace ModernApplicationFramework.Basics.InfoBar.Internal
{
    internal class InfoBarActionViewModel : InfoBarTextViewModel
    {
        internal InfoBarActionViewModel(InfoBarViewModel owner, IInfoBarTextSpan textSpan) : base(owner, textSpan)
        {
            ClickActionItemCommand = new DelegateCommand(OnClickActionItemCommandExecuted);
        }

        private void OnClickActionItemCommandExecuted(object parameter)
        {
            Owner.NotifyActionItemClicked(ActionItem);
        }

        public ICommand ClickActionItemCommand { get; }

        public bool IsButton => ActionItem.IsButton;

        private IInfoBarActionItem ActionItem => (IInfoBarActionItem) TextSpan;
    }
}
