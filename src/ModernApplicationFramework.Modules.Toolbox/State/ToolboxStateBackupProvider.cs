using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.State
{
    [Export(typeof(IToolboxStateBackupProvider))]
    internal class ToolboxStateBackupProvider : LayoutBackupProvider, IToolboxStateBackupProvider
    {

    }
}