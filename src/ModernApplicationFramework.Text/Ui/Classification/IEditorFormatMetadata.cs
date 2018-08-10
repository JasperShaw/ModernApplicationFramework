using System.ComponentModel;

namespace ModernApplicationFramework.Text.Ui.Classification
{
    public interface IEditorFormatMetadata
    {
        string Name { get; }

        [DefaultValue(0)] int Priority { get; }

        [DefaultValue(false)] bool UserVisible { get; }
    }
}