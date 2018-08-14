﻿using System.ComponentModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.KeyGestureScopeTest
{
    [DisplayName("TextViewModel")]
    [Export(typeof(TextViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class TextViewModel : KeyBindingLayoutItem
    {
        private object _content;
        public override GestureScope GestureScope => TextEditorScope.TextEditor;

        public object Content
        {
            get => _content;
            set
            {
                if (Equals(value, _content)) return;
                _content = value;
                NotifyOfPropertyChange();
            }
        }

        public TextViewModel()
        {
            DisplayName = "Text View";
            _content = IoC.Get<PrimitiveTextView>().Content;
        }
    }
}