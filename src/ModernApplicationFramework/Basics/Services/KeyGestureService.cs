using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Properties;
using Action = System.Action;

namespace ModernApplicationFramework.Basics.Services
{
    /// <inheritdoc />
    /// <summary>
    ///     A service to manage key input bindings
    /// </summary>
    /// <seealso cref="IKeyGestureService" />
    [Export(typeof(IKeyGestureService))]
    internal class KeyGestureService : IKeyGestureService
    {
        private readonly Dictionary<GestureScope, HashSet<UIElement>> _elementMapping;
        private readonly GestureScope[] _gestureScopes;
        private readonly IKeyboardInputService _keyboardInputService;
        private readonly IStatusBarDataModelService _statusBarDataModelService;
        private readonly CommandDefinition[] _keyboardShortcuts;

        private readonly object _lockObj = new object();

        private readonly List<MultiKeyGesture> _possibleMultiGestures = new List<MultiKeyGesture>();
        private bool _isEnhancedMultiKeyGestureModeEnabled;


        private bool _multiMode;
        private KeySequence _oldKeySequence;

        public event EventHandler Initialized;

        public bool EnableEnhancedInputFiltering { get; set; }

        public bool IsInitialized { get; private set; }

        public bool IsEnhancedMultiKeyGestureModeEnabled
        {
            get => _isEnhancedMultiKeyGestureModeEnabled;
            set
            {
                if (_isEnhancedMultiKeyGestureModeEnabled == value)
                    return;
                _isEnhancedMultiKeyGestureModeEnabled = value;
                if (value)
                    _keyboardInputService.PreviewKeyDown += HandlePreviewKeyInput;
                else
                    _keyboardInputService.PreviewKeyDown -= HandlePreviewKeyInput;
            }
        }

        [ImportingConstructor]
        public KeyGestureService([ImportMany] CommandBarItemDefinition[] keyboardShortcuts, [ImportMany] GestureScope[] gestureScopes,
            IKeyboardInputService keyboardInputService, IStatusBarDataModelService statusBarDataModelService)
        {
            _gestureScopes = gestureScopes;
            _keyboardInputService = keyboardInputService;
            _statusBarDataModelService = statusBarDataModelService;

            _keyboardShortcuts = keyboardShortcuts.OfType<CommandDefinition>().ToArray();

            _elementMapping = new Dictionary<GestureScope, HashSet<UIElement>>();

            foreach (var commandGestureCategory in gestureScopes)
                _elementMapping.Add(commandGestureCategory, new HashSet<UIElement>());
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes the service.
        /// </summary>
        public void Initialize()
        {
            if (IsInitialized)
                return;
            IsEnhancedMultiKeyGestureModeEnabled = true;
            IsInitialized = true;
            OnInitialized();
        }


        /// <inheritdoc />
        /// <summary>
        ///     Registers an <see cref="T:ModernApplicationFramework.CommandBase.ICanHaveInputBindings" /> to the service
        /// </summary>
        /// <param name="hostingModel">The hosting model.</param>
        public void AddModel(ICanHaveInputBindings hostingModel)
        {
            hostingModel.BindableElement.InputBindings.Clear();
            foreach (var commandDefinition in _keyboardShortcuts.Where(x => x.Gestures.Count > 0))
            {
                foreach (var gesture in commandDefinition.Gestures)
                {
                    if (hostingModel.GestureScopes.Contains(gesture.Scope))
                    {
                        var inputBinding = new MultiKeyBinding(commandDefinition.Command, gesture.KeyGesture);
                        hostingModel.BindableElement.InputBindings.Add(inputBinding);
                        lock (_lockObj)
                        {
                            _elementMapping[gesture.Scope]
                                .Add(hostingModel.BindableElement);
                        }
                    }

                    else if (gesture.Scope.Equals(GestureScopes.GlobalGestureScope))
                    {
                        var inputBinding = new MultiKeyBinding(commandDefinition.Command, gesture.KeyGesture);
                        hostingModel.BindableElement.InputBindings.Add(inputBinding);
                        lock (_lockObj)
                        {
                            _elementMapping[GestureScopes.GlobalGestureScope]
                                .Add(hostingModel.BindableElement);
                        }
                    }
                }
            }
        }


        /// <inheritdoc />
        /// <summary>
        ///     Removes an <see cref="T:ModernApplicationFramework.CommandBase.ICanHaveInputBindings" /> from the service
        /// </summary>
        /// <param name="hostingModel">The hosting model.</param>
        public void RemoveModel(ICanHaveInputBindings hostingModel)
        {
            lock (_lockObj)
            {
                foreach (var scope in hostingModel.GestureScopes)
                {
                    if (_elementMapping.ContainsKey(scope))
                        _elementMapping[scope].Remove(hostingModel.BindableElement);
                }
            }
            hostingModel.BindableElement?.InputBindings.Clear();
        }

        public void AddKeyGestures(CommandGestureScopeMapping commandKeyGestureScope)
        {
            if (!IsInitialized)
                return;
            if (commandKeyGestureScope?.CommandDefinition == null || commandKeyGestureScope.GestureScopeMapping == null)
                return;

            IEnumerable<UIElement> possibleElements;
            lock (_lockObj)
            {
                possibleElements = _elementMapping.Where(x => x.Key.Equals(commandKeyGestureScope.GestureScopeMapping.Scope))
                    .SelectMany(x => x.Value);
            }

            foreach (var element in possibleElements)
                element.InputBindings.Add(new MultiKeyBinding(commandKeyGestureScope.CommandDefinition.Command, commandKeyGestureScope.GestureScopeMapping.KeyGesture));
        }

        public void RemoveAllKeyGestures()
        {
            if (!IsInitialized)
                return;
            foreach (var commandDefinition in _keyboardShortcuts)
                commandDefinition.Gestures.Clear();
        }

        public void RemoveKeyGesture(GestureScopeMapping keyGestureScope)
        {
            if (keyGestureScope?.KeyGesture == null || keyGestureScope.Scope == null)
                return;

            IEnumerable<UIElement> possibleElements;
            lock (_lockObj)
            {
                possibleElements = _elementMapping.Where(x => x.Key.Equals(keyGestureScope.Scope))
                    .SelectMany(x => x.Value);
            }

            foreach (var element in possibleElements)
            {
                var bindings = new ArrayList(element.InputBindings);

                foreach (InputBinding binding in bindings)
                    if (binding is MultiKeyBinding multiKeyBinding &&
                        multiKeyBinding.Gesture.Equals(keyGestureScope.KeyGesture))
                        element.InputBindings.Remove(binding);
            }
        }

        public void LoadDefaultGestures()
        {
            //var defaultCommands =
            //    _keyboardShortcuts.Where(x => x.DefaultGestureScope != null && x.DefaultKeyGestures != null);

            var defaultCommands =
                _keyboardShortcuts.Where(x => x.DefaultGestureScopes.Count >= 1);


            

            foreach (var cd in defaultCommands)
            {
                cd.Gestures.Clear();
                //foreach (var gesture in cd.DefaultKeyGestures)
                //{
                //    cd.Gestures.Add(new GestureScopeMapping(cd.DefaultGestureScope, gesture));
                //}
                foreach (var mapping in cd.DefaultGestureScopes)
                    cd.Gestures.Add(mapping);
            }
        }

        public IEnumerable<CommandGestureScopeMapping> GetAllBindings()
        {
            return from commandDefinition in _keyboardShortcuts
                   from commandDefinitionGesture in commandDefinition.Gestures
                   select new CommandGestureScopeMapping(commandDefinitionGesture.Scope, commandDefinition,
                       commandDefinitionGesture.KeyGesture);
        }

        public IEnumerable<CommandDefinition> GetAllCommandDefinitions()
        {
            return new List<CommandDefinition>(_keyboardShortcuts);
        }

        public IEnumerable<GestureScope> GetAllCommandGestureScopes()
        {
            return new List<GestureScope>(_gestureScopes);
        }

        public IEnumerable<CommandGestureScopeMapping> FindKeyGestures(IList<KeySequence> sequences, FindKeyGestureOption option)
        {
            var list = new List<CommandGestureScopeMapping>();
            foreach (var commandDefinition in _keyboardShortcuts)
            {
                foreach (var definitionGesture in commandDefinition.Gestures)
                {
                    if (definitionGesture.KeyGesture.Contains(sequences))
                        list.Add(new CommandGestureScopeMapping(commandDefinition, definitionGesture));
                }
            }
            return list;
        }


        /// <summary>
        /// Checks a keyboard input for multi-key gestures.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void CheckKeyInput(KeyEventArgs e)
        {
            if (_multiMode)
                CheckMultiStateKeyInput(e);
            else
            {
                if (IgnoreKey(e.Key))
                    return;

                _possibleMultiGestures.Clear();

                // Get the current pressed Keys
                var ks = _oldKeySequence = new KeySequence(NativeMethods.ModifierKeys, e.Key);

                foreach (var gesture in _keyboardShortcuts.SelectMany(x => x.KeyGestures).Where(x => x.IsRealMultiKeyGesture))
                    if (ks.Modifiers == gesture.GestureCollection[0].Modifiers &&
                        ks.Key == gesture.GestureCollection[0].Key)
                        _possibleMultiGestures.Add(gesture);


                var i = Keyboard.FocusedElement;
                var currentScopes = GestureHelper.GetScopesFromElement(i as UIElement);

                var breakflag = false;
                foreach (var currentScope in currentScopes)
                {
                    if (breakflag)
                        break;
                    if (currentScope.Equals(GestureScopes.GlobalGestureScope) && _possibleMultiGestures.Count == 0)
                        return;


                    if (TryCreateKeyGesture(_oldKeySequence, out var inputGesture))
                    {
                        foreach (var shortcut in _keyboardShortcuts)
                        {
                            if (shortcut.KeyGestures.Contains(inputGesture))
                            {
                                var t = shortcut.Gestures;
                                if (t.Contains(new GestureScopeMapping(currentScope, inputGesture)))
                                {
                                    shortcut.Command.Execute(null);

                                    // Prevents other commands beeing invoked
                                    e.Handled = true;
                                    breakflag = true;
                                    break;
                                }
                            }
                        }
                    }

                }


                if (_possibleMultiGestures.Count == 0)
                    return;

                //Prevents other KeyGestures being triggered
                e.Handled = true;
                SetMultiState(ks);
            }
        }

        /// <summary>
        /// Checks the keyboard input when the Service is in multi-key state
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void CheckMultiStateKeyInput(KeyEventArgs e)
        {
            //Ignore Modifier keys
            if (IgnoreKey(e.Key))
            {
                e.Handled = true;
                return;
            }

            var ks = new KeySequence(NativeMethods.ModifierKeys, e.Key);

            var gesture = _possibleMultiGestures.FirstOrDefault(x =>
                x.GestureCollection[1].Key == ks.Key && x.GestureCollection[1].Modifiers == ks.Modifiers);

            if (gesture == null)
            {
                ResetMultiState(false, ks, null);
                e.Handled = true;
                return;
            }
            gesture.WasFoundDuringMulti = true;

            //Prevents other KeyGestures being triggered
            e.Handled = true;

            //The correct Key gesture was found. Now get the currently selected UIElement and apply the input
            var element = Keyboard.FocusedElement as UIElement;
            if (element == null)
            {
                ResetMultiState(false, ks, null);
                e.Handled = true;
                return;
            }
            var currentScopes = GestureHelper.GetScopesFromElement(element);

            foreach (var currentScope in currentScopes)
            {
                if (currentScope != GestureScopes.GlobalGestureScope)
                {
                    IEnumerable<UIElement> elements;
                    lock (_lockObj)
                        elements = _elementMapping[currentScope];

                    element = GestureHelper.FindParentElementFromList(element, elements.ToList());
                    if (element == null)
                        return;
                }
            }

            var op = element.Dispatcher.BeginInvoke(
                DispatcherPriority.Input, CreateElementKeyDownAction(element, e)
            );
            op.Completed += (_, __) => ResetMultiState(true, ks, gesture);
        }

        /// <summary>
        /// Sets the multi-key state
        /// </summary>
        /// <param name="foundSequence">The found sequence.</param>
        private void SetMultiState(KeySequence foundSequence)
        {
            _multiMode = true;
            var message = string.Format(CommonUI_Resources.KeyGestureService_EnterMutliKeyState, foundSequence);
            _statusBarDataModelService.SetText(message);
        }

        /// <summary>
        /// Resets the multi-key input state. Displays result to Status Bar
        /// </summary>
        /// <param name="success">Flag that tells if the second, expected key sequence was found at all</param>
        /// <param name="newKeySequence">The second key sequence.</param>
        /// <param name="gesture">The gesture. May be <see langword="null"/></param>
        private void ResetMultiState(bool success, KeySequence newKeySequence, MultiKeyGesture gesture)
        {
            _multiMode = false;
            _statusBarDataModelService.SetText(string.Empty);

            if (!success)
            {
                var mkg = new MultiKeyGesture(new[] { _oldKeySequence, newKeySequence });
                var gm = (string)
                    MultiKeyGesture.KeyGestureConverter.ConvertTo(null, CultureInfo.CurrentCulture, mkg,
                        typeof(string));
                var message = string.Format(CommonUI_Resources.KeyGestureService_MultiKeyNotFound, gm);
                _statusBarDataModelService.SetText(message);
                SystemSounds.Asterisk.Play();

                if (gesture != null)
                    gesture.WasFoundDuringMulti = false;
                return;
            }
            if (gesture == null || gesture.WasFoundDuringMulti)
            {
                ResetMultiState(false, newKeySequence, gesture);
                return;
            }
            _statusBarDataModelService.SetText(string.Empty);
            _statusBarDataModelService.SetReadyText();
            gesture.WasFoundDuringMulti = false;
        }

        private void OnInitialized()
        {
            Initialized?.Invoke(this, EventArgs.Empty);
        }

        private void HandlePreviewKeyInput(object sender, KeyEventArgs e)
        {
            CheckKeyInput(e);
        }

        private static bool IgnoreKey(Key key)
        {
            if ((NativeMethods.ModifierKeys &
                 (ModifierKeys.Windows | ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift)) ==
                ModifierKeys.None)
                return false;
            switch (key)
            {
                case Key.LWin:
                case Key.RWin:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.LeftShift:
                case Key.RightShift:
                    return true;
            }
            return false;
        }

        private static Action CreateElementKeyDownAction(IInputElement element, KeyEventArgs e)
        {
            return () => element.RaiseEvent(
                new KeyEventArgs(
                        e.KeyboardDevice,
                        e.InputSource,
                        e.Timestamp,
                        Key.None)
                { RoutedEvent = Keyboard.KeyDownEvent });
        }

        private static bool TryCreateKeyGesture(KeySequence sequence, out MultiKeyGesture gesture)
        {
            gesture = default;
            if (!GestureHelper.IsKeyGestureValid(sequence.Key, sequence.Modifiers))
                return false;
            try
            {
                gesture = new MultiKeyGesture(sequence.Key, sequence.Modifiers);
                return true;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }
    }
}