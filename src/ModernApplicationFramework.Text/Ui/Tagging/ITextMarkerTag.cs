using ModernApplicationFramework.Text.Logic.Tagging;

namespace ModernApplicationFramework.Text.Ui.Tagging
{
    public interface ITextMarkerTag : ITag
    {
        string Type { get; }
    }
}