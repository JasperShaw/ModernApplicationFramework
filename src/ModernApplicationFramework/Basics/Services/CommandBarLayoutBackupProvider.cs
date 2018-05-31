using System.ComponentModel.Composition;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Services
{
    [Export(typeof(ICommandBarLayoutBackupProvider))]
    public class CommandBarLayoutBackupProvider : LayoutBackupProvider, ICommandBarLayoutBackupProvider
    {
    }
}
