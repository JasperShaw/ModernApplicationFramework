using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ModernApplicationFramework.Core.Converters.AccessKey;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <inheritdoc />
    /// <summary>
    /// Basic definition model for command bar elements
    /// </summary>
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public abstract class CommandDefinitionBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Fires when a property was changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The localized name of the definition
        /// </summary>
        public virtual string Name => (string)AccessKeyRemovingConverter.Instance.Convert(Text, typeof(string), null,
            CultureInfo.CurrentCulture);

        public abstract string NameUnlocalized { get; }

        /// <summary>
        /// The localized display text including possible mnemonic underlining
        /// </summary>
        public abstract string Text { get; }

        /// <summary>
        /// The tooltip of the definition
        /// </summary>
        public abstract string ToolTip { get; }

        /// <summary>
        /// An URI to a resource that holds an icon
        /// </summary>
        public abstract Uri IconSource { get; }

        /// <summary>
        /// The key or ID of the icon inside the <see cref="IconSource"/>
        /// </summary>
        public abstract string IconId { get; }

        /// <summary>
        /// Options that identifies the definition as a container of a list of definitions
        /// </summary>
        public abstract bool IsList { get; }

        /// <summary>
        /// The <see cref="CommandCategory"/> of this definition. May be <see langword="null"/>
        /// </summary>
        public abstract CommandCategory Category { get; }

        /// <summary>
        /// The type of this definition
        /// </summary>
        public abstract CommandControlTypes ControlType { get; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}