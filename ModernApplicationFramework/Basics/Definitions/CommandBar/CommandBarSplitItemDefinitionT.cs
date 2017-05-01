using System;
using System.Globalization;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public sealed class CommandBarSplitItemDefinitionT<T> : CommandBarItemDefinition where T : DefinitionBase
    {
        private int _selectedIndex;
        private string _statusString;
        public override DefinitionBase CommandDefinition { get; }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value == _selectedIndex)
                    return;
                _selectedIndex = value;
                OnPropertyChanged();
                StatusString = StringCreator?.CreateMessage(value + 1);
            }
        }

        public string StatusString
        {
            get => _statusString;
            set
            {
                if (value == _statusString) return;
                _statusString = value;
                OnPropertyChanged();
            }
        }

        public IStatusStringCreator StringCreator { get; set; }

        public CommandBarSplitItemDefinitionT(string statusString, CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : base(null, sortOrder, group, null, isVisible, isChecked, isCustom, isCustomizable)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            Text = CommandDefinition.Text;
            _statusString = statusString;
        }

        public CommandBarSplitItemDefinitionT(IStatusStringCreator statusStringCreator, CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : base(null, sortOrder, group, null, isVisible, isChecked, isCustom, isCustomizable)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            Text = CommandDefinition.Text;
            StringCreator = statusStringCreator;
            _statusString = StringCreator.CreateMessage();
        }
    }

    public class NumberStatusStringCreator : IStatusStringCreator
    {

        public string StatusTextTemplate { get; set; }
        public string PluralSuffix { get; set; }

        public NumberStatusStringCreator(string statusText, string pluralSuffix)
        {
            StatusTextTemplate = statusText;
            PluralSuffix = pluralSuffix;
        }

        public string CreateMessage(object value)
        {
            if (!int.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture, out int number))
                throw new ArgumentException(nameof(value));
            return Math.Abs(number) == 1 ? string.Format(StatusTextTemplate, number, string.Empty) : string.Format(StatusTextTemplate, number, PluralSuffix);
        }

        public string CreateMessage()
        {
            return CreateMessage(1);
        }
    }

    public interface IStatusStringCreator
    {
        string StatusTextTemplate { get; set; }
        string PluralSuffix { get; set; }

        string CreateMessage(object value);

        string CreateMessage();
    }
}