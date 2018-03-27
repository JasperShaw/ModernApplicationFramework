using System;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Modules.Inspector
{
    public interface IInspectorTool : ITool
    {
        event EventHandler SelectedObjectChanged;
        IInspectableObject SelectedObject { get; set; }
    }
}