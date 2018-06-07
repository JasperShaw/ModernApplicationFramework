using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
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

        protected CommandDefinition(UICommand command) : this()
        {
            Command = command;
        }

        /// <summary>
        ///     The executable command of the definition
        /// </summary>
        public virtual ICommand Command { get; }

        public sealed override bool IsList => false;

        public override CommandControlTypes ControlType => CommandControlTypes.Button;

        /// <summary>
        ///     A command specific parameter
        /// </summary>
        public virtual object CommandParamenter { get; set; }

        /// <summary>
        ///     Indicated whether the command is checked or not.
        /// </summary>
        public virtual bool IsChecked { get; set; }

        /// <summary>
        ///     Option to suppress the <see cref="Command" /> from execution. Default is <see langword="true" />.
        ///     Gets automatically disabled when the command was added to the exclusion list of the
        ///     <see cref="ICommandBarDefinitionHost" />
        /// </summary>
        public bool AllowExecution { get; set; } = true;


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

        /// <summary>
        /// The default key gesture.
        /// </summary>
        public abstract MultiKeyGesture DefaultKeyGesture { get; }

        /// <summary>
        /// The <see cref="GestureScope"/> of the <see cref="DefaultKeyGesture"/>.
        /// </summary>
        public abstract GestureScope DefaultGestureScope { get; }

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