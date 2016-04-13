using System.Windows.Input;

namespace ModernApplicationFramework.MVVM.Demo.Modules.MyEditor
{
    /// <summary>
    /// Interaktionslogik für SimpleTextEditorView.xaml
    /// </summary>
    public partial class MyTextEditorView
    {
        public MyTextEditorView()
        {
            InitializeComponent();
            Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
}
