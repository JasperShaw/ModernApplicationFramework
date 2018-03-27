using System.Windows.Input;

namespace ModernApplicationFramework.Modules.Editors.MyEditor
{
    /// <summary>
    /// Interaktionslogik für SimpleTextEditorView.xaml
    /// </summary>
    public partial class MyTextEditorView
    {
        public MyTextEditorView()
        {
            InitializeComponent();
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
}
