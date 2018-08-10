using System;

namespace ModernApplicationFramework.Text.Ui.Tagging
{
    public class ErrorTag : IErrorTag
    {
        public string ErrorType { get; }

        public object ToolTipContent { get; }

        public ErrorTag(string errorType, object toolTipContent)
        {
            ErrorType = errorType ?? throw new ArgumentNullException(nameof(errorType));
            ToolTipContent = toolTipContent;
        }

        public ErrorTag(string errorType)
            : this(errorType, null)
        {
        }

        public ErrorTag()
            : this("syntax error", null)
        {
        }
    }
}