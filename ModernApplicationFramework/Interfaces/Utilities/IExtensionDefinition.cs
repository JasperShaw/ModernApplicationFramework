using System;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IExtensionDefinition
    {
        string ApplicationContext { get; }
        string Description { get; }
        Uri IconSource { get; }
        string Name { get; }
        int SortOrder { get; }
    }
}