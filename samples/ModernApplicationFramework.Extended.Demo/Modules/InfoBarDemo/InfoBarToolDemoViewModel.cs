using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Input;
using ModernApplicationFramework.Basics.InfoBar;
using ModernApplicationFramework.Controls.InfoBar;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Controls.InfoBar;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Demo.Modules.InfoBarDemo
{
    [Export(typeof(InfoBarToolDemoViewModel))]
    public sealed class InfoBarToolDemoViewModel : Tool
    {
        public override PaneLocation PreferredLocation => PaneLocation.Right;

        public ICommand ShowCommand => new Command(Show);

        public InfoBarToolDemoViewModel()
        {
            DisplayName = "InfoBar Demo";
        }

        protected override void OnInfoBarActionItemClicked(IInfoBarUiElement infoBarUi, InfoBarModel infoBar, IInfoBarActionItem actionItem)
        {
            base.OnInfoBarActionItemClicked(infoBarUi, infoBar, actionItem);
            Process.Start(actionItem.Text);
        }

        private void Show()
        {
            var infoBarTextSpanArray = new[]
            {
                new InfoBarTextSpan("Test Text "),
                new InfoBarHyperlink("www.google.de")
            };

            var imageInfo = new ImageInfo("StatusInfoIcon",
                "/ModernApplicationFramework;component/Resources/Icons/StatusInfo.xaml", true);

            var model = new InfoBarModel(infoBarTextSpanArray, imageInfo);
            AddInfoBar(model);
        }
    }
}
