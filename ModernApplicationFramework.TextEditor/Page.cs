using System;

namespace ModernApplicationFramework.TextEditor
{
    internal class Page
    {
        private readonly WeakReference<char[]> _uncompressedContents;
        private readonly byte[] _compressedContents;
        public readonly int Length;
        public readonly PageManager Manager;

        public Page(PageManager manager, char[] contents, int length)
        {
            Manager = manager;
            Length = length;
            _uncompressedContents = new WeakReference<char[]>(contents);
            _compressedContents = Compressor.Compress(contents, length);
        }

        public char[] Expand()
        {
            if (!_uncompressedContents.TryGetTarget(out var target))
            {
                target = new char[Length];
                Compressor.Decompress(_compressedContents, Length, target);
                _uncompressedContents.SetTarget(target);
            }
            Manager.UpdateMRU(this, target);
            return target;
        }
    }
}