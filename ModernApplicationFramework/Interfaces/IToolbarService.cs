using System;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.Interfaces
{
    public interface IToolbarService
    {
        ToolBar GetToolbar(Type toolBarType);
    }
}