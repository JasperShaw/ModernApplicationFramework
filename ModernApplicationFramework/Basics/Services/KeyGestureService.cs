using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.CommandBase.Input;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Services
{
    /// <inheritdoc />
    /// <summary>
    /// A service to manage key input bindings
    /// </summary>
    /// <seealso cref="IKeyGestureService" />
    [Export(typeof(IKeyGestureService))]
    public class KeyGestureService : IKeyGestureService
    {
        private readonly CommandGestureCategory[] _gestureCategories;
        private readonly CommandDefinition[] _keyboardShortcuts;
        private readonly Dictionary<CommandGestureCategory, HashSet<UIElement>> _elementMapping;


        public event EventHandler Initialized;

        public bool IsInitialized { get; private set; }


        [ImportingConstructor]
        public KeyGestureService([ImportMany] CommandDefinitionBase[] keyboardShortcuts, [ImportMany] CommandGestureCategory[] gestureCategories)
        {
            _gestureCategories = gestureCategories;
            _keyboardShortcuts = keyboardShortcuts.OfType<CommandDefinition>().ToArray();
            
            _elementMapping = new Dictionary<CommandGestureCategory, HashSet<UIElement>>();
            
            foreach (var commandGestureCategory in gestureCategories)
                _elementMapping.Add(commandGestureCategory, new HashSet<UIElement>());
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes the service.
        /// </summary>
        public void Initialize()
        {
            if (IsInitialized)
                return;
            InitializeGestures();
            IsInitialized = true;
            OnInitialized();           
        }

        /// <inheritdoc />
        /// <summary>
        /// Registers an <see cref="T:ModernApplicationFramework.CommandBase.ICanHaveInputBindings" /> to the service
        /// </summary>
        /// <param name="hostingModel">The hosting model.</param>
        public void Register(ICanHaveInputBindings hostingModel)
        {
            hostingModel.BindableElement.InputBindings.Clear();
            foreach (var commandDefinition in _keyboardShortcuts.Where(x => x.Gestures.Count > 0))
            {
                foreach (var gesture in commandDefinition.Gestures)
                {
                    if (!gesture.Category.Equals(hostingModel.GestureCategory) &&
                        !gesture.Category.Equals(CommandGestureCategories.GlobalGestureCategory))
                        continue;
                    var inputBinding = new MultiKeyBinding(commandDefinition.Command, gesture.KeyGesture);
                    hostingModel.BindableElement.InputBindings.Add(inputBinding);
                    _elementMapping[hostingModel.GestureCategory]
                        .Add(hostingModel.BindableElement);
                }
            }
        }


        /// <inheritdoc />
        /// <summary>
        /// Removes an <see cref="T:ModernApplicationFramework.CommandBase.ICanHaveInputBindings" /> from the service
        /// </summary>
        /// <param name="hostingModel">The hosting model.</param>
        public void Remove(ICanHaveInputBindings hostingModel)
        {
            if (!_elementMapping.ContainsKey(hostingModel.GestureCategory))
                return;
            _elementMapping[hostingModel.GestureCategory].Remove(hostingModel.BindableElement);
            if (!_elementMapping.ContainsKey(CommandGestureCategories.GlobalGestureCategory))
                return;
            _elementMapping[CommandGestureCategories.GlobalGestureCategory].Remove(hostingModel.BindableElement);

            hostingModel.BindableElement?.InputBindings.Clear();
        }

        public void SetKeyGestures()
        {
            if (!IsInitialized)
                return;
        }

        public void RemoveKeyGestures()
        {
            
        }


        /// <inheritdoc />
        /// <summary>
        /// Loads all available key gestures and applies them to their command
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
                cd.Gestures.Add(new CategoryKeyGesture(cd.DefaultGestureCategory, cd.DefaultKeyGesture));
            }
        }

        public IEnumerable<CommandCategoryGestureMapping> GetAllBindings()
        {
            return from commandDefinition in _keyboardShortcuts
                   from commandDefinitionGesture in commandDefinition.Gestures
                   select new CommandCategoryGestureMapping(commandDefinitionGesture.Category, commandDefinition,
                       commandDefinitionGesture.KeyGesture);
        }

        public IEnumerable<CommandDefinition> GetAllCommandCommandDefinitions()
        {
            return new List<CommandDefinition>(_keyboardShortcuts);
        }


        /// <summary>
        /// Performs the initial Gesture to Command  mapping
        /// </summary>
        protected virtual void InitializeGestures()
        {
            LoadDefaultGestures();
        }

        protected virtual void OnInitialized()
        {
            Initialized?.Invoke(this, EventArgs.Empty);
        }
    }

    public class CommandCategoryGestureMapping
    {
        public CommandCategoryGestureMapping(CommandGestureCategory category, CommandDefinitionBase command, MultiKeyGesture gesture)
        {
            Category = category;
            Command = command;
            Gesture = gesture;
        }

        public CommandDefinitionBase Command { get; }
        public MultiKeyGesture Gesture { get; }
        public CommandGestureCategory Category { get; }

        public override string ToString()
        {
            var gestureText = Gesture.GetDisplayStringForCulture(CultureInfo.CurrentCulture);
            return $"{Command.Name} + {Category.Name} = {gestureText}";
        }
    }
}