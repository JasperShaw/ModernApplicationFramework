using System;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Modules.InspectorTool
{
    public interface IInspectorTool : ITool
    {
        event EventHandler SelectedObjectChanged;
        IInspectableObject SelectedObject { get; set; }
    }
}
