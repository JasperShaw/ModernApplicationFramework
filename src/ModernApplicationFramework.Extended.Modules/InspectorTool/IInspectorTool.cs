using System;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool
{
    public interface IInspectorTool : ITool
    {
        event EventHandler SelectedObjectChanged;
        IInspectableObject SelectedObject { get; set; }
    }
}