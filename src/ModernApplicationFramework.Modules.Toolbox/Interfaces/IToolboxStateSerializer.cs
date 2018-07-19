using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    /// <summary>
    /// Serializer/Deserializer for the toolbox layout state
    /// </summary>
    /// <seealso cref="ModernApplicationFramework.Interfaces.Services.ILayoutSerializer{IToolboxNode}" />
    public interface IToolboxStateSerializer : ILayoutSerializer<IToolboxNode>
    {
    }
}