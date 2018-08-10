using System;
using System.Text;

namespace ModernApplicationFramework.Text.Data
{
    public sealed class EncodingChangedEventArgs : EventArgs
    {
        public Encoding NewEncoding { get; }

        public Encoding OldEncoding { get; }

        public EncodingChangedEventArgs(Encoding oldEncoding, Encoding newEncoding)
        {
            OldEncoding = oldEncoding;
            NewEncoding = newEncoding;
        }
    }
}