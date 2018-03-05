using System;

namespace ModernApplicationFramework.EditorBase.Editor
{
    public class EditorException : Exception
    {
        public EditorException()
        {

        }

        public EditorException(string message) : base(message)
        {

        }
    }
}