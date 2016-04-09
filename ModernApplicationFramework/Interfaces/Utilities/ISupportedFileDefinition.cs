using System;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface ISupportedFileDefinition
    {
        string Description { get; }
        FileType FileType { get; }
        Uri IconSource { get; }
        string Name { get; }
        Type PrefferedEditor { get; }
        int SortOrder { get; }
    }
}