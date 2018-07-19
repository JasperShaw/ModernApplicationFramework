using System.Collections.Generic;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    /// <summary>
    /// Toolbox state provider
    /// </summary>
    public interface IToolboxStateProvider
    {
        /// <summary>
        /// A copy of the toolbox layout state.
        /// </summary>
        IReadOnlyCollection<IToolboxCategory> State { get; }
    }
}