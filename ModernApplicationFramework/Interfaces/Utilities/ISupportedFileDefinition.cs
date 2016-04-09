using System;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface ISupportedFileDefinition
    {
        string Name { get; }
        int SortOrder { get; }
        string Description { get; }
        Uri IconSource { get; }
        FileType FileType { get; }
        Type PrefferedEditor { get; }
    }
}