using System.Windows.Input;

namespace ModernApplicationFramework.EditorBase.Dialogs.SimpleTextEditor
{
    /// <summary>
    /// Interaktionslogik für SimpleTextEditorView.xaml
    /// </summary>
    public partial class SimpleTextEditorView
    {
        public SimpleTextEditorView()
        {
            InitializeComponent();
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
}
