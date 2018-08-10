namespace ModernApplicationFramework.Text.Ui.Tagging
{
    public class OverviewMarkTag : IOverviewMarkTag
    {
        public OverviewMarkTag(string markKindName)
        {
            MarkKindName = markKindName;
        }

        public string MarkKindName { get; }
    }
}