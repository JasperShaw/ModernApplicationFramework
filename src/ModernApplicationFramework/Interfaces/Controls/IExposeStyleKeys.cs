using System.Windows;

namespace ModernApplicationFramework.Interfaces.Controls
{
    public interface IExposeStyleKeys
    {
        /// <summary>
        /// The style key for a simple button
        /// </summary>
        ResourceKey ButtonStyleKey { get; }

        /// <summary>
        /// The style key for a menu controller
        /// </summary>
        ResourceKey MenuControllerStyleKey { get; }

        /// <summary>
        /// The style key for a combo box
        /// </summary>
        ResourceKey ComboBoxStyleKey { get; }

        /// <summary>
        /// The style key for a menu item
        /// </summary>
        ResourceKey MenuStyleKey { get; }

        /// <summary>
        /// The style key for a separator
        /// </summary>
        ResourceKey SeparatorStyleKey { get; }
    }
}