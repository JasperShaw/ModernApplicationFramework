using System;

namespace ModernApplicationFramework.TextEditor
{
    public class ErrorTag : IErrorTag
    {
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

        public string ErrorType { get; }

        public object ToolTipContent { get; }
    }
}