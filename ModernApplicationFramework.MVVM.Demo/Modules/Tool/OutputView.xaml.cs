using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModernApplicationFramework.MVVM.Demo.Modules.Tool
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
