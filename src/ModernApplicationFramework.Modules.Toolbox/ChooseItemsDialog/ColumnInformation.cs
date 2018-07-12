namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    public class ColumnInformation
    {
        public string Text { get; }

        public string Name { get; }

        public ColumnInformation(string propertyName, string localizedName)
        {
            Name = propertyName;
            Text = localizedName;
        }
    }
}