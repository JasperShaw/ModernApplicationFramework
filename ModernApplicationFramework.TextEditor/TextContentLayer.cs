using System.Windows;

namespace ModernApplicationFramework.TextEditor
{
    internal class TextContentLayer : FrameworkElement
    {
        public TextContentLayer()
        {
            IsVisibleChanged += OnVisibleChanged;
            IsHitTestVisible = false;
            Name = "Maf_TextEditorContent";
        }


        private void OnVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}