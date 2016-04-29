namespace ModernApplicationFramework.MVVM.Modules.OutputTool
{
    /// <summary>
    /// Interaktionslogik für OutputView.xaml
    /// </summary>
    public partial class OutputView : IOutputView
    {
        public OutputView()
        {
            InitializeComponent();
        }

        public void ScrollToEnd()
        {
            outputText.Focus();
            outputText.CaretIndex = outputText.Text.Length;
            outputText.ScrollToEnd();
        }

        public void Clear()
        {
            outputText.Clear();
        }

        public void AppendText(string text)
        {
            outputText.AppendText(text);
            ScrollToEnd();
        }

        public void SetText(string text)
        {
            outputText.Text = text;
            ScrollToEnd();
        }
    }
}
