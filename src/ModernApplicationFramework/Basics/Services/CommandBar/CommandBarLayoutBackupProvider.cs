using System.ComponentModel.Composition;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Services.CommandBar
{
    [Export(typeof(ICommandBarLayoutBackupProvider))]
    public class CommandBarLayoutBackupProvider : LayoutBackupProvider, ICommandBarLayoutBackupProvider
    {
    }
}
