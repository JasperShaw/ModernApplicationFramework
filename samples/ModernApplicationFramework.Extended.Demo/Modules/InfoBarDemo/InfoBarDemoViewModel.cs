using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.InfoBar;
using ModernApplicationFramework.Controls.InfoBar;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Controls.InfoBar;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Demo.Modules.InfoBarDemo
{
    [DisplayName("Info Bar Demo")]
    [Export(typeof(InfoBarDemoViewModel))]
    public sealed class InfoBarDemoViewModel : LayoutItem, IInfoBarUiEvents
    {
        private readonly IDockingHostViewModel _dockingHost;
        private readonly IDockingMainWindowViewModel _mainWindow;
        public ICommand ShowInfoBarCommand => new Command(ShowInfoBar);
        public ICommand OpenDemoToolCommand => new Command(OpenDemoTool);

        private void OpenDemoTool()
        {
            _dockingHost.ShowTool<InfoBarToolDemoViewModel>();
        }

        private void ShowInfoBar()
        {
            var infoBarTextSpanArray = new[]
            {
                new InfoBarTextSpan("Test Text "),
                new  InfoBarButton("www.google.de")
            };

            var ai = new[]
            {
                new InfoBarButton("Test")
            };


            var imageInfo = new ImageInfo("/ModernApplicationFramework.Extended.Demo;component/Resources/StatusInformation_16x.png");


            var model = new InfoBarModel(infoBarTextSpanArray, ai, imageInfo);
            var ui = IoC.Get<IInfoBarUiFactory>().CreateInfoBar(model);

            ui.Advise(this, out var _);

            _mainWindow.InfoBarHost.AddInfoBar(ui);
        }


        [ImportingConstructor]
        public InfoBarDemoViewModel(IDockingHostViewModel dockingHost, IDockingMainWindowViewModel mainWindow)
        {
            _dockingHost = dockingHost;
            _mainWindow = mainWindow;
            DisplayName = "Info Bar Demo";
        }

        void IInfoBarUiEvents.OnClosed(IInfoBarUiElement infoBarUiElement)
        {

        }

        void IInfoBarUiEvents.OnActionItemClicked(IInfoBarUiElement infoBarUiElement, IInfoBarActionItem actionItem)
        {
            try
            {
                Process.Start(actionItem.Text);
            }
            catch
            {
                //Ignored
            }
           
        }
    }
}
