using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Controls.InfoBar;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.InfoBar.Internal
{
    internal sealed class InfoBarViewModel : ObservableObject
    {
        public bool IsCloseButtonVisible { get; }

        internal InfoBarUiElement Owner { get; set; }

        public ICommand CloseCommand { get; }

        public ImageMoniker Icon { get; set; }

        public IEnumerable<InfoBarTextViewModel> MainText { get; }

        public IEnumerable<InfoBarActionViewModel> ActionItems { get; }

        internal InfoBarViewModel(InfoBarModel infoBar)
        {
            Validate.IsNotNull(infoBar, nameof(infoBar));
            IsCloseButtonVisible = infoBar.IsCloseButtonVisible;
            Icon = infoBar.Image;
            var barTextViewModelArray = new InfoBarTextViewModel[infoBar.TextSpans.Count];
            var stringBuilder = new StringBuilder();
            for (var index = 0; index < infoBar.TextSpans.Count; ++index)
            {
                var span = infoBar.TextSpans.GetSpan(index);
                var actionItem = span as IInfoBarActionItem;
                barTextViewModelArray[index] = actionItem == null ? new InfoBarTextViewModel(this, span) : new InfoBarActionViewModel(this, actionItem);
                stringBuilder.Append(barTextViewModelArray[index].Text);
                stringBuilder.Append(" ");
            }
            InfoBarActionViewModel[] barActionViewModelArray;
            if (infoBar.ActionItems != null)
            {
                barActionViewModelArray = new InfoBarActionViewModel[infoBar.ActionItems.Count];
                for (int index = 0; index < infoBar.ActionItems.Count; ++index)
                {
                    var actionItem = infoBar.ActionItems.GetItem(index);
                    barActionViewModelArray[index] = new InfoBarActionViewModel(this, actionItem);
                    stringBuilder.Append(barActionViewModelArray[index].Text);
                    stringBuilder.Append(" ");
                }
            }
            else
                barActionViewModelArray = new InfoBarActionViewModel[0];
            MainText = barTextViewModelArray;
            ActionItems = barActionViewModelArray;
            CloseCommand = new DelegateCommand(OnCloseCommandExecuted, CanExecuteCloseCommand);
        }

        private bool CanExecuteCloseCommand(object obj)
        {
            return IsCloseButtonVisible;
        }

        private void OnCloseCommandExecuted(object obj)
        {
            Owner.Close();
        }

        public void NotifyActionItemClicked(IInfoBarActionItem action)
        {
            Owner.ForEach((cookie, events) => events.OnActionItemClicked(Owner, action));
        }
    }
}
