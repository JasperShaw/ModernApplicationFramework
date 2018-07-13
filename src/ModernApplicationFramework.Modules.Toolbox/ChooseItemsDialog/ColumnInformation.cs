namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    public class ColumnInformation
    {
        public string Name { get; }
        public string Text { get; }

        public ColumnInformation(string propertyName, string localizedName)
        {
            Name = propertyName;
            Text = localizedName;
        }
    }
}