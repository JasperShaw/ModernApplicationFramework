using System.ComponentModel.Composition;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.Settings;

namespace ModernApplicationFramework.EditorBase.Controls.EditorSelectorDialog
{
    [Export(typeof(NewFileEditorSelectorViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class NewFileEditorSelectorViewModel : EditorSelectorViewModelBase
    {
        [ImportingConstructor]
        public NewFileEditorSelectorViewModel([ImportMany] IEditor[] editors, INewFileEditorAssociationSettings settings) :
            base(editors, settings)
        {
            DisplayName = EditorSelectorResources.WindowTitle;
        }
    }
}