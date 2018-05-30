using ModernApplicationFramework.EditorBase.FileSupport;

namespace ModernApplicationFramework.EditorBase.Interfaces.Packages
{
    public interface IMruFilePackage
    {
        FileSystemMruManager Manager { get; }
    }
}