﻿namespace ModernApplicationFramework.MVVM.Core
{
    public sealed class SaveDirtyDocumentItem
    {
        public SaveDirtyDocumentItem(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }
}
