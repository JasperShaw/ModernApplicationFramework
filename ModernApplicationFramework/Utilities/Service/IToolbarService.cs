using System;
using ModernApplicationFramework.Controls;

namespace ModernApplicationFramework.Utilities.Service
{
    public interface IToolbarService
    {
        ToolBar GetToolbar(Type toolBarType);
    }
}