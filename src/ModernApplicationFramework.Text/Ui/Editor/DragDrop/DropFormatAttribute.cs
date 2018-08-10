using System;
using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Text.Ui.Editor.DragDrop
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class DropFormatAttribute : Attribute
    {
        public string DropFormats { get; }

        public DropFormatAttribute(string dropFormat)
        {
            DropFormats = dropFormat;
        }
    }
}