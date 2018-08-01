using System;
using System.Text;

namespace ModernApplicationFramework.TextEditor
{
    public sealed class EncodingChangedEventArgs : EventArgs
    {
        public EncodingChangedEventArgs(Encoding oldEncoding, Encoding newEncoding)
        {
            OldEncoding = oldEncoding;
            NewEncoding = newEncoding;
        }

        public Encoding OldEncoding { get; }

        public Encoding NewEncoding { get; }
    }
}