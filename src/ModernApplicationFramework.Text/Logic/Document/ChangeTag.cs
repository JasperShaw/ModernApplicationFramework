using ModernApplicationFramework.Text.Logic.Tagging;

namespace ModernApplicationFramework.Text.Logic.Document
{
    public class ChangeTag : ITag
    {
        public ChangeTypes ChangeTypes { get; }

        public ChangeTag(ChangeTypes type)
        {
            ChangeTypes = type;
        }
    }
}