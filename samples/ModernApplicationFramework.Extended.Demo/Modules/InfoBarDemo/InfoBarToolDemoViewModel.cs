using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Input;
using ModernApplicationFramework.Controls.InfoBar;
using ModernApplicationFramework.Extended.Core.LayoutItems;
using ModernApplicationFramework.Extended.Core.Pane;
using ModernApplicationFramework.Input.Command;

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
            var model = new InfoBarModel(infoBarTextSpanArray);
            AddInfoBar(model);
        }
    }
}
