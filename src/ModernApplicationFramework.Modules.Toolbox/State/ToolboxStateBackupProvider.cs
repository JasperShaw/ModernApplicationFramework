using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.State
{
    [Export(typeof(IToolboxStateBackupProvider))]
    public class ToolboxStateBackupProvider : LayoutBackupProvider, IToolboxStateBackupProvider
    {

    }
}