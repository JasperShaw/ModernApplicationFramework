using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Caliburn.Micro;
using ModernApplicationFramework.Core.Converters.AccessKey;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

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
            Gestures.GestursChanged += Gestures_GestursChanged;
            KeyGestures = new List<MultiKeyGesture>();
        }

        protected CommandDefinition(UICommand command) : this()
        {
            Command = command;
        }

        /// <summary>
        ///     The executable command of the definition
        /// </summary>
        public virtual UICommand Command { get; }

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
        /// The trimmed name of the command definition in this format:
        /// {Category}.{Name}
        /// </summary>
        public string TrimmedCategoryCommandName
        {
            get
            {
                var name = (string) new AccessKeyRemovingConverter().Convert(Name, typeof(string), null,
                    CultureInfo.CurrentCulture);
                if (name == null)
                    return string.Empty;

                var category = Regex.Replace(Category.Name, @"\s+", "", RegexOptions.Compiled);
                var trimmedName = Regex.Replace(name, @"\s+", "", RegexOptions.Compiled);

                return $"{category}.{trimmedName}";
            }
        }

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
        /// The <see cref="CommandGestureCategory"/> of the <see cref="DefaultKeyGesture"/>.
        /// </summary>
        public abstract CommandGestureCategory DefaultGestureCategory { get; }

        /// <summary>
        /// The collection of all gestures.
        /// </summary>
        public GestureCollection Gestures { get; }

        /// <summary>
        /// A read-only collection of all key gestures
        /// </summary>
        public IReadOnlyList<MultiKeyGesture> KeyGestures { get; private set; }


        private void Gestures_GestursChanged(object sender, GestureCollectionChangedEventArgs e)
        {
            if (_gestureService == null)
                _gestureService = IoC.Get<IKeyGestureService>();
            
            KeyGestures = new ReadOnlyCollection<MultiKeyGesture>(Gestures.Select(x => x.KeyGesture).ToList());
            switch (e.Type)
            {
                case GestureCollectionChangedType.Added:
                    _gestureService.AddKeyGestures(Command ,e.CategoryKeyGesture.FirstOrDefault());
                    break;
                case GestureCollectionChangedType.Removed:
                    _gestureService.RemoveKeyGesture(e.CategoryKeyGesture.FirstOrDefault());
                    break;
                case GestureCollectionChangedType.Cleared:
                    foreach (var gesture in e.CategoryKeyGesture)
                        _gestureService.RemoveKeyGesture(gesture);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }          
            OnPropertyChanged(nameof(GestureText));
        }
    }
}