using System;
using System.Collections.Generic;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    /// <summary>
    /// Main API for interacting with the toolbox
    /// </summary>
    public interface IToolboxService
    {
        /// <summary>
        /// Adds a category to the toolbox state.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="suppressRefresh">if set to <see langword="true"/> refreshing the toolbox view will be supressed.</param>
        void AddCategory(IToolboxCategory category, bool suppressRefresh = false);

        /// <summary>
        /// Finds items by defintion.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>All found items</returns>
        IEnumerable<IToolboxItem> FindItemsByDefintion(ToolboxItemDefinitionBase definition);

        /// <summary>
        /// Gets all toolbox category names.
        /// </summary>
        /// <returns>The names</returns>
        IReadOnlyCollection<string> GetAllToolboxCategoryNames();

        /// <summary>
        /// Gets the category by its node ID.
        /// </summary>
        /// <param name="guid">The ID.</param>
        /// <returns>The found category.</returns>
        IToolboxCategory GetCategoryById(Guid guid);

        /// <summary>
        /// Gets the item by its node ID.
        /// </summary>
        /// <param name="guid">The ID.</param>
        /// <returns>The found item.</returns>
        IToolboxItem GetItemById(Guid guid);

        /// <summary>
        /// Gets the selected category.
        /// </summary>
        /// <returns>The category</returns>
        IToolboxCategory GetSelectedCategory();

        /// <summary>
        /// Gets a copy of the current toolbox layout state
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<IToolboxCategory> GetToolboxItemSource();

        /// <summary>
        /// Inserts a category.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="category">The category.</param>
        /// <param name="supressRefresh">if set to <see langword="true"/> refreshing the toolbox view will be supressed.</param>
        void InsertCategory(int index, IToolboxCategory category, bool supressRefresh = false);

        /// <summary>
        /// Removes a category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="cascading">if set to <see langword="true"/> all items references to this category will be removed also</param>
        /// <param name="supressRefresh">if set to <see langword="true"/> refreshing the toolbox view will be supressed.</param>
        void RemoveCategory(IToolboxCategory category, bool cascading = true, bool supressRefresh = false);

        /// <summary>
        /// Stores the and applies a given layout.
        /// </summary>
        /// <param name="layout">The layout.</param>
        void StoreAndApplyLayout(IEnumerable<IToolboxCategory> layout);

        /// <summary>
        /// Stores the current layout.
        /// </summary>
        void StoreCurrentLayout();

        /// <summary>
        /// Checks if the toolbox contains a item with the given definition
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns><see langowrd="true"/> if any item was found</returns>
        bool ToolboxHasItem(ToolboxItemDefinitionBase definition);
    }
}