using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using ModernApplicationFramework.EditorBase.Layout;

namespace ModernApplicationFramework.EditorBase.Controls.SimpleTextEditor
{
    [Export(typeof(SimpleTextEditorViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)] //Ensures we can create multiple documents at the same type
    public class SimpleTextEditorViewModel : StorableDocument
    {
        private string _originalText;

        private SimpleTextEditorView _view;

        protected override Task CreateNewFile()
        {
            _originalText = string.Empty;
            ApplyOriginalText();
            return Task.FromResult(true);
        }

        protected override Task LoadFile(string filePath)
        {
            _originalText = File.ReadAllText(filePath);
            ApplyOriginalText();
            return Task.FromResult(true);
        }

        protected override void OnViewLoaded(object view)
        {
            _view = (SimpleTextEditorView) view;
        }

        protected override Task SaveFile(string filePath)
        {
            var newText = _view.TextBox.Text;
            File.WriteAllText(filePath, newText);
            _originalText = newText;
            return Task.FromResult(true);
        }

        private void ApplyOriginalText()
        {
            _view.TextBox.Text = _originalText;
            _view.TextBox.TextChanged +=
                delegate { IsDirty = string.CompareOrdinal(_originalText, _view.TextBox.Text) != 0; };
        }
    }
}