using System;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal class Page
    {
        public readonly int Length;
        public readonly PageManager Manager;
        private readonly byte[] _compressedContents;
        private readonly WeakReference<char[]> _uncompressedContents;

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