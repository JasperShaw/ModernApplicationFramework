using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Commands;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <inheritdoc />
    /// <summary>
    ///     Basic definition model used for application commands
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinitionBase" />
    public abstract class CommandDefinition : CommandDefinitionBase
    {
        private IKeyGestureService _gestureService;
        
        protected CommandDefinition()
        {
            Gestures = new GestureCollection();
            Gestures.CollectionChanged += Gestures_GestursChanged;
            KeyGestures = new List<MultiKeyGesture>();
        }

        /// <summary>
        ///     The executable command of the definition
        /// </summary>
        public ICommandDefinitionCommand Command { get; protected set; }

        public sealed override bool IsList => false;

        public override CommandControlTypes ControlType => CommandControlTypes.Button;

        /// <summary>
        ///     Option to suppress the <see cref="Command" /> from execution. Default is <see langword="true" />.
        ///     Gets automatically disabled when the command was added to the exclusion list of the
        ///     <see cref="ICommandBarDefinitionHost" />
        /// </summary>
        /// TODO: Integrate into current code
        public bool AllowExecution { get; set; } = true;

        public virtual bool AllowGestureMapping => true;


        /// <summary>
        /// The gesture text of the first known KeyGesture.
        /// </summary>
        public string GestureText
        {
            get
            {
                if (Gestures.Count == 0)
                    return string.Empty;
                return
                    (string)
                    MultiKeyGesture.KeyGestureConverter.ConvertTo(null, CultureInfo.CurrentCulture,
                        Gestures[0].KeyGesture,
                        typeof(string));
            }
        }

        public virtual ReadOnlyCollection<GestureScopeMapping> DefaultGestureScopes =>
            new ReadOnlyCollection<GestureScopeMapping>(new List<GestureScopeMapping>());

        /// <summary>
        /// The collection of all gestures.
        /// </summary>
        public GestureCollection Gestures { get; }

        /// <summary>
        /// A read-only collection of all key gestures
        /// </summary>
        public IReadOnlyList<MultiKeyGesture> KeyGestures { get; private set; }

        private void Gestures_GestursChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_gestureService == null)
                _gestureService = IoC.Get<IKeyGestureService>();
            
            KeyGestures = new ReadOnlyCollection<MultiKeyGesture>(Gestures.Select(x => x.KeyGesture).ToList());
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var mapping = new CommandGestureScopeMapping(this, e.NewItems.OfType<GestureScopeMapping>().FirstOrDefault());
                    _gestureService.AddKeyGestures(mapping);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _gestureService.RemoveKeyGesture(e.OldItems.OfType<GestureScopeMapping>().FirstOrDefault());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var gesture in Gestures)
                        _gestureService.RemoveKeyGesture(gesture);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }          
            OnPropertyChanged(nameof(GestureText));
        }
    }
}