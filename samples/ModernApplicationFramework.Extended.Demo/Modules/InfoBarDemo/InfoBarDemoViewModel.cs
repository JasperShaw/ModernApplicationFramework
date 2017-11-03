using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Input;
using ModernApplicationFramework.Controls.InfoBar;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.InfoBarDemo
{
    [DisplayName("Info Bar Demo")]
    [Export(typeof(InfoBarDemoViewModel))]
    public sealed class InfoBarDemoViewModel : Core.LayoutItems.LayoutItem, IInfoBarUiEvents
    {
        public ICommand ShowInfoBarCommand => new Command(ShowInfoBar);

        private void ShowInfoBar()
        {
            var infoBarTextSpanArray = new[]
            {
                new InfoBarTextSpan("Test Text "),
                new InfoBarHyperlink("www.google.de")
            };

            var model = new InfoBarModel(infoBarTextSpanArray);

            var host = InfoBarHostControl.Instance;

            var bar = host.CreateInfoBar(this, model);
            host.AddInfoBar(bar);

        }


        [ImportingConstructor]
        public InfoBarDemoViewModel()
        {
            DisplayName = "Info Bar Demo";
        }

        void IInfoBarUiEvents.OnClosed(IInfoBarUiElement infoBarUiElement)
        {

        }

        void IInfoBarUiEvents.OnActionItemClicked(IInfoBarUiElement infoBarUiElement, IInfoBarActionItem actionItem)
        {
            Process.Start("www.google.de");
        }
    }
}
