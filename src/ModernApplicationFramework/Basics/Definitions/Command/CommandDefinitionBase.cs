using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using ModernApplicationFramework.Basics.Imaging;
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
        protected virtual char Delimiter => '.';


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


        public virtual ImageMoniker ImageMonikerSource { get; }


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

        /// <summary>
        /// The localized trimmed name of the command definition in this format:
        /// {Category}{Delimiter}{Name}
        /// </summary>
        public string TrimmedCategoryCommandName
        {
            get
            {
                var name = (string)new AccessKeyRemovingConverter().Convert(Name, typeof(string), null,
                    CultureInfo.CurrentCulture);
                if (name == null)
                    return string.Empty;

                var category = Regex.Replace(Category.Name, @"\s+", "", RegexOptions.Compiled);
                var trimmedName = Regex.Replace(name, @"\s+", "", RegexOptions.Compiled);

                return $"{category}{Delimiter}{trimmedName}";
            }
        }

        /// <summary>
        /// The localized trimmed name of the command definition in this format:
        /// {Category}{Delimiter}{Name}
        /// </summary>
        public string TrimmedCategoryCommandNameUnlocalized
        {
            get
            {
                var name = (string)new AccessKeyRemovingConverter().Convert(NameUnlocalized, typeof(string), null,
                    CultureInfo.CurrentCulture);
                if (name == null)
                    return string.Empty;

                var category = Regex.Replace(Category.NameUnlocalized, @"\s+", "", RegexOptions.Compiled);
                var trimmedName = Regex.Replace(name, @"\s+", "", RegexOptions.Compiled);

                return $"{category}{Delimiter}{trimmedName}";
            }
        }

        public abstract Guid Id { get; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}