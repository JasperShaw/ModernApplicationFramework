using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ModernApplicationFramework.CommandBase.Input;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Controls.TextBoxes
{
    public class KeyGestureTextBox : TextBox
    {
        private readonly KeySequence[] _keySequences;
        private bool _isMultiState;
        
        public IReadOnlyCollection<KeySequence> KeySequences => new ReadOnlyCollection<KeySequence>(_keySequences);

        public KeyGestureTextBox()
        {
            _keySequences = new KeySequence[2];
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Back && NativeMethods.ModifierKeys == ModifierKeys.None)
            {
                HandleRemove();
                CreateText();
                e.Handled = true;
                return;
            }
            e.Handled = true;
            if (ValidateInput(e))
                CreateText();
        }

        private void CreateText()
        {
           var text = string.Empty;
            foreach (var keySequence in _keySequences)
            {
               if (keySequence == null)
                   break;
                text += $"{keySequence}, ";
            }

            text = text.Trim();

            if (text.EndsWith(","))
                text = text.Remove(text.Length - 1, 1);
            
            Text = text;
            CaretIndex = Text.Length;
        }

        private bool ValidateInput(KeyEventArgs e)
        {
            return _isMultiState ? ValidateMultiStateInput(e) : ValidateNormalStateInput(e);
        }

        private bool ValidateNormalStateInput(KeyEventArgs e)
        {
            CreateKeys(e, out var modifier, out var key);

            if (!ValidateBasicKeyInput(key))
                return false;
            
            //Only F-Keys are valid as single KeyGesture key
            if (modifier == ModifierKeys.None && !CheckForFunctionKey(key))
                    return false;

            _keySequences[0] = new KeySequence(modifier, key);
            _isMultiState = true;
            return true;
        }


        private bool ValidateMultiStateInput(KeyEventArgs e)
        {
            CreateKeys(e, out var modifier, out var key);

            if (_keySequences[1]?.Key == key && _keySequences[1].Modifiers == modifier)
            {
                HandleRemove();
                return true;
            }          
            if (!ValidateBasicKeyInput(key))
                return false;
            _keySequences[1] = new KeySequence(modifier, key);
            return true;
        }

        private bool CheckForFunctionKey(Key key)
        {
            var vCode = KeyInterop.VirtualKeyFromKey(key);
            if (vCode >= 112 && vCode <= 135)
                return true;
            return false;
        }

        private bool IsValidGestureKey(Key key)
        {
            var vCode = KeyInterop.VirtualKeyFromKey(key);
            if (vCode >= 8 && vCode <= 256)
            {
                //Filter Media Keys like Volume up
                if (vCode >= 166 && vCode <= 183 || vCode == 144)
                    return false;
                return true;
            }
            return false;
        }


        private bool ValidateBasicKeyInput(Key key)
        {
            if (key == Key.None)
                return false;
            if (!IsValidGestureKey(key))
                return false;
            if (MultiKeyGesture.IsModifierKey(key))
                return false;
            return true;
        }


        
        private void HandleRemove()
        {
            if (_isMultiState && _keySequences[1] == null)
            {
                _isMultiState = false;
                _keySequences[0] = null;
            }
            else
                _keySequences[1] = null;
        }

        private void CreateKeys(KeyEventArgs e, out ModifierKeys modifier, out Key key)
        {
            modifier = NativeMethods.ModifierKeys & ~ModifierKeys.Windows;
            key = e.Key == Key.System ? e.SystemKey : e.Key;
        }
    }
}