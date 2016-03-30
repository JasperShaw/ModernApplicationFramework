using ModernApplicationFramework.Caliburn.Platform.Action;
using ModernApplicationFramework.Caliburn.Platform.Utilities;

namespace ModernApplicationFramework.Caliburn.Platform.Interfaces
{
    /// <summary>
    /// Indicates that a message is parameterized.
    /// </summary>
    public interface IHaveParameters
    {
        /// <summary>
        /// Represents the parameters of a message.
        /// </summary>
        AttachedCollection<Parameter> Parameters { get; }
    }
}