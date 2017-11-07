using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Controls.Windows;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Interfaces.Controls.InfoBar;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.InfoBar.Internal
{
    internal sealed class InfoBarViewModel : ObservableObject, IThemableIconContainer
    {
        private object _icon;
        public bool IsCloseButtonVisible { get; }

        internal InfoBarUiElement Owner { get; set; }

        public ICommand CloseCommand { get; }

        public object IconSource { get; private set; }

        public object Icon { get; set; }

        public bool IsEnabled => true;

        public IEnumerable<InfoBarTextViewModel> MainText { get; }

        public IEnumerable<InfoBarActionViewModel> ActionItems { get; }

        internal InfoBarViewModel(InfoBarModel infoBar)
        {
            Validate.IsNotNull(infoBar, nameof(infoBar));
            IsCloseButtonVisible = infoBar.IsCloseButtonVisible;
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


            if (!infoBar.UseImageInfo)
                return;

            if (infoBar.ImageInfo.FromXamlResource == true)
            {
                var myResourceDictionary = new ResourceDictionary
                {
                    Source = infoBar.ImageInfo.Path
                };
                IconSource = myResourceDictionary[infoBar.ImageInfo.Id];
                this.SetThemedIcon((Color) ColorConverter.ConvertFromString("#F6F6F6"));
            }
            else
                Icon = new Image {Source = new BitmapImage(infoBar.ImageInfo.Path)};
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
