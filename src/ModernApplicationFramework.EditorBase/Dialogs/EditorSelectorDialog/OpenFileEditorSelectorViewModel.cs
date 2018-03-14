using System.ComponentModel.Composition;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.Settings.EditorAssociation;

namespace ModernApplicationFramework.EditorBase.Dialogs.EditorSelectorDialog
{
    [Export(typeof(OpenFileEditorSelectorViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class OpenFileEditorSelectorViewModel : EditorSelectorViewModelBase
    {
        [ImportingConstructor]
        public OpenFileEditorSelectorViewModel([ImportMany] IEditor[] editors, IOpenFileEditorAssociationSettings settings) :
            base(editors, settings)
        {
            DisplayName = EditorSelectorResources.WindowTitle;
        }
    }
}