using System.ComponentModel;

namespace ModernApplicationFramework.TextEditor
{
    public interface INameMetadata
    {
        [DefaultValue(null)]
        string Name { get; }
    }
}