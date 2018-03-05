using System;

namespace ModernApplicationFramework.EditorBase.FileSupport.Exceptions
{
    public class FileNotSupportedException : Exception
    {
        public FileNotSupportedException()
        {
            
        }

        public FileNotSupportedException(string message) : base(message)
        {
            
        }
    }
}
