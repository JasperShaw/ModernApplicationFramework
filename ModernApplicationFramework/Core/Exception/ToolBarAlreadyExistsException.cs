namespace ModernApplicationFramework.Core.Exception
{
    internal class ToolBarAlreadyExistsException : System.Exception
    {
        public ToolBarAlreadyExistsException() {}

        public ToolBarAlreadyExistsException(string message) : base(message) {}

        public ToolBarAlreadyExistsException(string message, System.Exception inner)
            : base(message, inner) {}
    }
}