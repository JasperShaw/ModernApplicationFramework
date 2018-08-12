using System.Windows;
using System.Windows.Input;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public class ImeTextComposition : TextComposition
    {
        public ImeTextComposition(InputManager inputManager, IInputElement source, string resultText)
            : base(inputManager, source, resultText)
        {
        }
    }
}