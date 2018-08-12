using System.ComponentModel;

namespace ModernApplicationFramework.Modules.Editor.EditorOptions
{
    public interface INameMetadata
    {
        [DefaultValue(null)] string Name { get; }
    }
}