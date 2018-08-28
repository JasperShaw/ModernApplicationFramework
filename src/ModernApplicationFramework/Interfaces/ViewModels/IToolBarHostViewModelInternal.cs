using ModernApplicationFramework.Basics.CommandBar.DataSources;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    internal interface IToolBarHostViewModelInternal : IToolBarHostViewModel
    {
        /// <summary>
        /// Adds a new tool bar to the host
        /// </summary>
        /// <param name="definition">The data model of the toolbar</param>
        void AddToolbarDefinition(ToolBarDataSource definition);

        /// <summary>
        /// Removes a toolbar from the host
        /// </summary>
        /// <param name="definition">The model of the toolbar</param>
        void RemoveToolbarDefinition(ToolBarDataSource definition);
    }
}