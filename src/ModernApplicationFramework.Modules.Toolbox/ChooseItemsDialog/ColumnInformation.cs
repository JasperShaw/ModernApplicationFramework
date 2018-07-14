namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    /// <summary>
    /// Holds information for a Column of a Toolbox item page
    /// </summary>
    public class ColumnInformation
    {

        /// <summary>
        /// Property name of <see cref="ItemDataSource"/> to bind with
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Localized display text of the column
        /// </summary>
        public string Text { get; }

        public ColumnInformation(string propertyName, string localizedName)
        {
            Name = propertyName;
            Text = localizedName;
        }
    }
}