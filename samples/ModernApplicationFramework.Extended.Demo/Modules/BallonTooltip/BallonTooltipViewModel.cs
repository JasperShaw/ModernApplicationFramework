using System.ComponentModel.Composition;
using System.Media;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.BallonTooltip
{
    [Export(typeof(BallonTooltipViewModel))]
    public class BallonTooltipViewModel : Tool
    {
        public override PaneLocation PreferredLocation => PaneLocation.Right;

        public ICommand WarningCommand => new Command(ShowWarning, o => true);

        public ICommand InfoCommand => new Command(ShowInfo, o => true);

        public ICommand ErrorCommand => new Command(ShowError, o => true);

        private void ShowInfo(object obj)
        {
            new BalloonTooltip(GetView() as Control, "Info", "Info" , BalloonType.Info, SystemSounds.Asterisk).Show();
        }

        private void ShowError(object obj)
        {
            new BalloonTooltip(GetView() as Control, "Error", "Error", BalloonType.Error, SystemSounds.Asterisk).Show();
        }

        private void ShowWarning(object o)
        {
            new BalloonTooltip(GetView() as Control, "Warning", "Warning", BalloonType.Warning, SystemSounds.Asterisk).Show();
        }
    }
}
