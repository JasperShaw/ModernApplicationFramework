using System.Windows.Input;

namespace ModernApplicationFramework.EditorBase.Controls.SimpleTextEditor
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
