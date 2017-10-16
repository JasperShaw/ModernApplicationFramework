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
    public abstract class KeyGestureService : IKeyGestureService
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

        protected KeyGestureService(CommandDefinitionBase[] keyboardShortcuts,GestureScope[] gestureScopes,
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
                foreach (var gesture in commandDefinition.Gestures)
                    if (gesture.Scope.Equals(hostingModel.GestureScope))
                    {
                        var inputBinding = new MultiKeyBinding(commandDefinition.Command, gesture.KeyGesture);
                        hostingModel.BindableElement.InputBindings.Add(inputBinding);
                        lock (_lockObj)
                        {
                            _elementMapping[hostingModel.GestureScope]
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


        /// <inheritdoc />
        /// <summary>
        ///     Removes an <see cref="T:ModernApplicationFramework.CommandBase.ICanHaveInputBindings" /> from the service
        /// </summary>
        /// <param name="hostingModel">The hosting model.</param>
        public void RemoveModel(ICanHaveInputBindings hostingModel)
        {
            lock (_lockObj)
            {
                if (!_elementMapping.ContainsKey(hostingModel.GestureScope))
                    return;
                _elementMapping[hostingModel.GestureScope].Remove(hostingModel.BindableElement);
                if (!_elementMapping.ContainsKey(GestureScopes.GlobalGestureScope))
                    return;
                _elementMapping[GestureScopes.GlobalGestureScope].Remove(hostingModel.BindableElement);
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
            var defaultCommands =
                _keyboardShortcuts.Where(x => x.DefaultGestureScope != null && x.DefaultKeyGesture != null);

            foreach (var cd in defaultCommands)
            {
                cd.Gestures.Clear();
                cd.Gestures.Add(new GestureScopeMapping(cd.DefaultGestureScope, cd.DefaultKeyGesture));
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
        protected virtual void CheckKeyInput(KeyEventArgs e)
        {
            if (_multiMode)
                CheckMultiStateKeyInput(e);
            else
            {
                _possibleMultiGestures.Clear();

                // Get the current pressed Keys
                var ks = _oldKeySequence = new KeySequence(NativeMethods.ModifierKeys, e.Key);

                foreach (var gesture in _keyboardShortcuts.SelectMany(x => x.KeyGestures).Where(x => x.IsRealMultiKeyGesture))
                    if (ks.Modifiers == gesture.GestureCollection[0].Modifiers &&
                        ks.Key == gesture.GestureCollection[0].Key)
                        _possibleMultiGestures.Add(gesture);
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
        protected virtual void CheckMultiStateKeyInput(KeyEventArgs e)
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
            
            //The correct Key gesture  was found. Now get the currently selected UIElement and apply the input
            var i = Keyboard.FocusedElement;
            if (!(i is UIElement element))
                return;
            var op = element.Dispatcher.BeginInvoke(
                DispatcherPriority.Input, CreateElementKeyDownAction(element, e)
            );
            op.Completed += (_, __) => ResetMultiState(true, ks, gesture);         
        }

        /// <summary>
        /// Sets the multi-key state
        /// </summary>
        /// <param name="foundSequence">The found sequence.</param>
        protected virtual void SetMultiState(KeySequence foundSequence)
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
        protected virtual void ResetMultiState(bool success, KeySequence newKeySequence, MultiKeyGesture gesture)
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

        protected virtual void OnInitialized()
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
    }

    [Export(typeof(IKeyGestureService))]
    public sealed class DefaultKeyGestureService : KeyGestureService
    {
        [ImportingConstructor]
        public DefaultKeyGestureService([ImportMany] CommandDefinitionBase[] keyboardShortcuts,
            [ImportMany] GestureScope[] gestureScopes, IKeyboardInputService keyboardInputService, 
            IStatusBarDataModelService statusBarDataModelService) : 
            base(keyboardShortcuts, gestureScopes, keyboardInputService, statusBarDataModelService)
        {
        }
    }
}