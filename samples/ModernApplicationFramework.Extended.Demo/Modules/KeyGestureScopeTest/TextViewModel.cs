using System.ComponentModel;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.KeyGestureScopeTest
{
    [DisplayName("TextViewModel")]
    [Export(typeof(TextViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class TextViewModel : KeyBindingLayoutItem
    {
        public override GestureScope GestureScope => TextEditorScope.TextEditor;

        public TextViewModel()
        {
            DisplayName = "Text View";
        }
    }
}
