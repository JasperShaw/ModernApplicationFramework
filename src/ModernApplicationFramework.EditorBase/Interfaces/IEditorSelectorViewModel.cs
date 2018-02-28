using ModernApplicationFramework.EditorBase.Controls.SimpleTextEditor;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;

namespace ModernApplicationFramework.EditorBase.Interfaces
{
    public interface IEditorSelectorViewModel
    {
        IExtensionDefinition TargetExtension { get; set; }

        IEditor Result { get; }
    }
}