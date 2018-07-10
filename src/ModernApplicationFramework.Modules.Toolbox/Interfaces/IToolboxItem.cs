using System;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxItem : IToolboxNode
    {
        IToolboxCategory Parent { get; set; }

        IToolboxCategory OriginalParent { get; }

        ToolboxItemDefinitionBase DataSource { get; }

        bool IsVisible { get; set; }

        bool IsEnabled { get; set; }

        bool EvaluateEnabled(Type targetType);
    }
}