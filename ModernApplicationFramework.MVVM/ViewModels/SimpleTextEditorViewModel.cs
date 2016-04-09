using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using ModernApplicationFramework.MVVM.Controls;
using ModernApplicationFramework.MVVM.Views;

namespace ModernApplicationFramework.MVVM.ViewModels
{
    [Export(typeof(SimpleTextEditorViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)] //Ensures we can create multiple documents at the same type
    public class SimpleTextEditorViewModel : StorableDocument
    {
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

        protected override Task SaveFile(string filePath)
        {
            var newText = _view.TextBox.Text;
            File.WriteAllText(filePath, newText);
            _originalText = newText;
            return Task.FromResult(true);
        }

        private string _originalText;

        private void ApplyOriginalText()
        {
            _view.TextBox.Text = _originalText;
            _view.TextBox.TextChanged += delegate
            {
                IsDirty = string.CompareOrdinal(_originalText, _view.TextBox.Text) != 0;
            };
        }

        private SimpleTextEditorView _view;

        protected override void OnViewLoaded(object view)
        {
            _view = (SimpleTextEditorView)view;
        }
    }
}
