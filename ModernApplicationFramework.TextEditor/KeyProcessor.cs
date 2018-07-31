using System.Windows.Input;

namespace ModernApplicationFramework.TextEditor
{
    public abstract class KeyProcessor
    {
        public virtual bool IsInterestedInHandledEvents => false;

        public virtual void PreviewKeyDown(KeyEventArgs args)
        {
        }

        public virtual void KeyDown(KeyEventArgs args)
        {
        }

        public virtual void PreviewKeyUp(KeyEventArgs args)
        {
        }

        public virtual void KeyUp(KeyEventArgs args)
        {
        }

        public virtual void PreviewTextInputStart(TextCompositionEventArgs args)
        {
        }

        public virtual void TextInputStart(TextCompositionEventArgs args)
        {
        }

        public virtual void PreviewTextInput(TextCompositionEventArgs args)
        {
        }

        public virtual void TextInput(TextCompositionEventArgs args)
        {
        }

        public virtual void PreviewTextInputUpdate(TextCompositionEventArgs args)
        {
        }

        public virtual void TextInputUpdate(TextCompositionEventArgs args)
        {
        }
    }
}