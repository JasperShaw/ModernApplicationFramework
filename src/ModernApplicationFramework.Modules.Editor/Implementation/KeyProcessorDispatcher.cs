using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal sealed class KeyProcessorDispatcher
    {
        private readonly GuardedOperations _guardedOperations;

        internal List<KeyProcessor> KeyProcessors { get; set; }

        internal KeyProcessorDispatcher(ITextView textView, List<KeyProcessor> keyProcessors,
            GuardedOperations guardedOperations)
        {
            KeyProcessors = keyProcessors;
            _guardedOperations = guardedOperations;
            DispatchKeyEvents(textView.VisualElement);
            DispatchTextInputEvents(textView.VisualElement);
        }

        private void Dispatch<T>(Action<KeyProcessor, T> action, T args) where T : RoutedEventArgs
        {
            foreach (var keyProcessor in KeyProcessors)
            {
                var processor = keyProcessor;
                if (!args.Handled || processor.IsInterestedInHandledEvents)
                    _guardedOperations.CallExtensionPoint(() => action(processor, args));
            }
        }

        private void DispatchKeyEvents(IInputElement element)
        {
            element.KeyDown += MakeHandler((p, args) => p.KeyDown(args));
            element.PreviewKeyDown += MakeHandler((p, args) => p.PreviewKeyDown(args));
            element.KeyUp += MakeHandler((p, args) => p.KeyUp(args));
            element.PreviewKeyUp += MakeHandler((p, args) => p.PreviewKeyUp(args));
        }

        private void DispatchTextInputEvents(FrameworkElement element)
        {
            element.TextInput += MakeHandler((p, args) => p.TextInput(args));
            element.PreviewTextInput += MakeHandler((p, args) => p.PreviewTextInput(args));
            TextCompositionManager.AddTextInputStartHandler(element, MakeHandler((p, args) => p.TextInputStart(args)));
            TextCompositionManager.AddPreviewTextInputStartHandler(element,
                MakeHandler((p, args) => p.PreviewTextInputStart(args)));
            TextCompositionManager.AddTextInputUpdateHandler(element,
                MakeHandler((p, args) => p.TextInputUpdate(args)));
            TextCompositionManager.AddPreviewTextInputUpdateHandler(element,
                MakeHandler((p, args) => p.PreviewTextInputUpdate(args)));
        }

        private KeyEventHandler MakeHandler(Action<KeyProcessor, KeyEventArgs> action)
        {
            return (sender, args) => Dispatch(action, args);
        }

        private TextCompositionEventHandler MakeHandler(Action<KeyProcessor, TextCompositionEventArgs> action)
        {
            return (sender, args) => Dispatch(action, args);
        }
    }
}