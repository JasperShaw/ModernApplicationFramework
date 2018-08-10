using System;

namespace ModernApplicationFramework.Text.Ui.Commanding
{
    public struct CommandState
    {
        public bool IsUnspecified { get; }

        public bool IsAvailable { get; }

        public bool IsChecked { get; }

        public string DisplayText { get; }

        public CommandState(bool isAvailable = false, bool isChecked = false, string displayText = null,
            bool isUnspecified = false)
        {
            if (isUnspecified && (isAvailable | isChecked || displayText != null))
                throw new ArgumentException(
                    "Unspecified command state cannot be combined with other states or command text.");
            IsAvailable = isAvailable;
            IsChecked = isChecked;
            IsUnspecified = isUnspecified;
            DisplayText = displayText;
        }

        public static CommandState Available { get; } = new CommandState(true);

        public static CommandState Unavailable { get; } = new CommandState(false);

        public static CommandState Unspecified { get; } = new CommandState(false, false, null, true);
    }
}