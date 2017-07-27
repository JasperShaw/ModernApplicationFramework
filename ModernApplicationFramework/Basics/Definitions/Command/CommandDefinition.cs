using System;
using System.Globalization;
using System.Text.RegularExpressions;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.CommandBase.Input;
using ModernApplicationFramework.Core.Converters.AccessKey;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <inheritdoc />
    /// <summary>
    /// Basic definition model used for application commands
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinitionBase" />
    public abstract class CommandDefinition : CommandDefinitionBase
    {
        /// <summary>
        /// The executable command of the definition
        /// </summary>
        public virtual UICommand Command { get; }

        public sealed override bool IsList => false;

        public override CommandControlTypes ControlType => CommandControlTypes.Button;

        /// <summary>
        /// A command specific parameter 
        /// </summary>
        public virtual object CommandParamenter { get; set; }

        /// <summary>
        /// Indicated whether the command is checked or not.
        /// </summary>
        public virtual bool IsChecked { get; set; }

        /// <summary>
        /// Option to suppress the <see cref="Command"/> from execution. Default is <see langword="true"/>.
        /// Gets automatically disabled when the command was added to the exclusion list of the <see cref="ICommandBarDefinitionHost"/>
        /// </summary>
        public bool AllowExecution { get; set; } = true;


        public string TrimmedCategoryCommandName
        {
            get
            {
                var name = (string)new AccessKeyRemovingConverter().Convert(Name, typeof(string), null, CultureInfo.CurrentCulture);
                if (name == null)
                    throw new ArgumentNullException(nameof(name));

                var category = Regex.Replace(Category.Name, @"\s+", "", RegexOptions.Compiled);
                var trimmedName = Regex.Replace(name, @"\s+", "", RegexOptions.Compiled);

                return $"{category}.{trimmedName}";
            }
        }

        protected CommandDefinition()
        {
            Gestures = new GestureCollection();
            Gestures.GestursChanged += Gestures_GestursChanged;
        }

        protected CommandDefinition(UICommand command) : this()
        {
            Command = command;
        }

        public string GestureText
        {
            get
            {
                var gesture = DefaultKeyGesture;
                if (gesture == null)
                    return null;
                return
                    (string)
                    MultiKeyGesture.KeyGestureConverter.ConvertTo(null, CultureInfo.CurrentCulture, gesture,
                        typeof(string));
            }
        }

        public abstract MultiKeyGesture DefaultKeyGesture { get; }

        public abstract CommandGestureCategory DefaultGestureCategory { get; }

        public GestureCollection Gestures { get; }


        private void Gestures_GestursChanged(object sender, GestureChangedEventArgs e)
        {

        }
    }
}