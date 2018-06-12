using System;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Controls;

namespace ModernApplicationFramework.Modules.Toolbox.CommandBar
{
    internal class ToolboxContextMenuProvider : IContextMenuProvider
    {
        public ContextMenu Provide(object dataContext)
        {
            if ((Type) dataContext == typeof(ToolboxTreeView))
                return IoC.Get<IContextMenuHost>().GetContextMenu(ToolboxContextMenuDefinition.ToolboxContextMenu);
            return null;
        }
    }
}