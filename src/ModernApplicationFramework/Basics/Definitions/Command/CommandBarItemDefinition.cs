using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Converters.AccessKey;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    /// <inheritdoc />
    /// <summary>
    /// Basic definition model for command bar elements
    /// </summary>
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public abstract class CommandBarItemDefinition : INotifyPropertyChanged
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
        /// The <see cref="CommandBarCategory"/> of this definition. May be <see langword="null"/>
        /// </summary>
        public abstract CommandBarCategory Category { get; }

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