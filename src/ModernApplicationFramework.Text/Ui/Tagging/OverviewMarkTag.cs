namespace ModernApplicationFramework.Text.Ui.Tagging
{
    public class OverviewMarkTag : IOverviewMarkTag
    {
        public string MarkKindName { get; }

        public OverviewMarkTag(string markKindName)
        {
            MarkKindName = markKindName;
        }
    }
}