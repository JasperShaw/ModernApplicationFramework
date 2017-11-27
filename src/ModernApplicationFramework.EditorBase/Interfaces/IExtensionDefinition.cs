using System;

namespace ModernApplicationFramework.EditorBase.Interfaces
{
    public interface IExtensionDefinition
    {
        string ApplicationContext { get; }
        string Description { get; }
        Uri IconSource { get; }
        string Name { get; }
        string PresetElementName { get; }
        int SortOrder { get; }
    }
}