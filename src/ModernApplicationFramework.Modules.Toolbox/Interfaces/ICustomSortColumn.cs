namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    /// <summary>
    /// Custom column that has a <see cref="Compare"/> method for manual comparing
    /// </summary>
    internal interface ICustomSortColumn
    {
        /// <summary>
        /// Compares the specified values.
        /// </summary>
        /// <param name="first">The first value.</param>
        /// <param name="second">The second value.</param>
        /// <returns>Margin between first and second value</returns>
        int Compare(string first, string second);
    }
}