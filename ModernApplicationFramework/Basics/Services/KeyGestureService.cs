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
    [Export(typeof(IKeyGestureService))]
    public class KeyGestureService : IKeyGestureService
    {
        private readonly Dictionary<CommandGestureCategory, HashSet<UIElement>> _elementMapping;
        private readonly CommandGestureCategory[] _gestureCategories;
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
        public KeyGestureService([ImportMany] CommandDefinitionBase[] keyboardShortcuts,
            [ImportMany] CommandGestureCategory[] gestureCategories,
            IKeyboardInputService keyboardInputService, IStatusBarDataModelService statusBarDataModelService)
        {
            _gestureCategories = gestureCategories;
            _keyboardInputService = keyboardInputService;
            _statusBarDataModelService = statusBarDataModelService;
            
            _keyboardShortcuts = keyboardShortcuts.OfType<CommandDefinition>().ToArray();

            _elementMapping = new Dictionary<CommandGestureCategory, HashSet<UIElement>>();

            foreach (var commandGestureCategory in gestureCategories)
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
            InitializeGestures();
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
                    if (gesture.Category.Equals(hostingModel.GestureCategory))
                    {
                        var inputBinding = new MultiKeyBinding(commandDefinition.Command, gesture.KeyGesture);
                        hostingModel.BindableElement.InputBindings.Add(inputBinding);
                        lock (_lockObj)
                        {
                            _elementMapping[hostingModel.GestureCategory]
                                .Add(hostingModel.BindableElement);
                        }
                    }
                    else if (gesture.Category.Equals(CommandGestureCategories.GlobalGestureCategory))
                    {
                        var inputBinding = new MultiKeyBinding(commandDefinition.Command, gesture.KeyGesture);
                        hostingModel.BindableElement.InputBindings.Add(inputBinding);
                        lock (_lockObj)
                        {
                            _elementMapping[CommandGestureCategories.GlobalGestureCategory]
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
                if (!_elementMapping.ContainsKey(hostingModel.GestureCategory))
                    return;
                _elementMapping[hostingModel.GestureCategory].Remove(hostingModel.BindableElement);
                if (!_elementMapping.ContainsKey(CommandGestureCategories.GlobalGestureCategory))
                    return;
                _elementMapping[CommandGestureCategories.GlobalGestureCategory].Remove(hostingModel.BindableElement);
            }
            hostingModel.BindableElement?.InputBindings.Clear();
        }

        public void AddKeyGestures(ICommand command, CategoryGestureMapping categoryKeyGesture)
        {
            if (!IsInitialized)
                return;
            if (command == null)
                return;
            if (categoryKeyGesture?.KeyGesture == null || categoryKeyGesture.Category == null)
                return;
            
            IEnumerable<UIElement> possibleElemetns;
            lock (_lockObj)
            {
                possibleElemetns = _elementMapping.Where(x => x.Key.Equals(categoryKeyGesture.Category))
                    .SelectMany(x => x.Value);
            }

            foreach (var element in possibleElemetns)
                element.InputBindings.Add(new MultiKeyBinding(command, categoryKeyGesture.KeyGesture));
        }

        public void RemoveAllKeyGestures()
        {
            if (!IsInitialized)
                return;
            lock (_lockObj)
            {
                var possibleElemetns = _elementMapping.SelectMany(x => x.Value);
                foreach (var element in possibleElemetns)
                {
                    var bindings = new ArrayList(element.InputBindings);
                    foreach (InputBinding binding in bindings)
                        if (binding is MultiKeyBinding)
                            element.InputBindings.Remove(binding);
                }
            }
        }

        public void RemoveKeyGesture(CategoryGestureMapping categoryKeyGesture)
        {
            if (categoryKeyGesture?.KeyGesture == null || categoryKeyGesture.Category == null)
                return;

            IEnumerable<UIElement> possibleElemetns;
            lock (_lockObj)
            {
                possibleElemetns = _elementMapping.Where(x => x.Key.Equals(categoryKeyGesture.Category))
                    .SelectMany(x => x.Value);
            }

            foreach (var element in possibleElemetns)
            {
                var bindings = new ArrayList(element.InputBindings);

                foreach (InputBinding binding in bindings)
                    if (binding is MultiKeyBinding multiKeyBinding &&
                        multiKeyBinding.Gesture.Equals(categoryKeyGesture.KeyGesture))
                        element.InputBindings.Remove(binding);
            }
        }


        /// <inheritdoc />
        /// <summary>
        ///     Loads all available key gestures and applies them to their command
        /// </summary>
        public virtual void LoadGestures()
        {
        }

        public virtual void LoadDefaultGestures()
        {
            var defaultCommands =
                _keyboardShortcuts.Where(x => x.DefaultGestureCategory != null && x.DefaultKeyGesture != null);

            foreach (var cd in defaultCommands)
            {
                cd.Gestures.Clear();
                cd.Gestures.Add(new CategoryGestureMapping(cd.DefaultGestureCategory, cd.DefaultKeyGesture));
            }
        }

        public IEnumerable<CommandCategoryGestureMapping> GetAllBindings()
        {
            return from commandDefinition in _keyboardShortcuts
                   from commandDefinitionGesture in commandDefinition.Gestures
                   select new CommandCategoryGestureMapping(commandDefinitionGesture.Category, commandDefinition,
                       commandDefinitionGesture.KeyGesture);
        }

        public IEnumerable<CommandDefinition> GetAllCommandDefinitions()
        {
            return new List<CommandDefinition>(_keyboardShortcuts);
        }

        public IEnumerable<CommandGestureCategory> GetAllCommandGestureCategories()
        {
            return new List<CommandGestureCategory>(_gestureCategories);
        }

        public IEnumerable<CommandCategoryGestureMapping> FindKeyGestures(IList<KeySequence> sequences, FindKeyGestureOption option)
        {    
            var list = new List<CommandCategoryGestureMapping>();
            foreach (var commandDefinition in _keyboardShortcuts)
            {
                foreach (var definitionGesture in commandDefinition.Gestures)
                {
                    if (definitionGesture.KeyGesture.Contains(sequences))
                        list.Add(new CommandCategoryGestureMapping(commandDefinition, definitionGesture));
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

        /// <summary>
        ///     Performs the initial Gesture to Command  mapping
        /// </summary>
        protected virtual void InitializeGestures()
        {
            LoadDefaultGestures();
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
}