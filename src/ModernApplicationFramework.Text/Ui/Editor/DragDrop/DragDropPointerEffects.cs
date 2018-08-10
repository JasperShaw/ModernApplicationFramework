using System;

namespace ModernApplicationFramework.Text.Ui.Editor.DragDrop
{
    [Flags]
    public enum DragDropPointerEffects
    {
        None = 0,
        Copy = 1,
        Link = 2,
        Move = 4,
        Scroll = 8,
        Track = 16, // 0x00000010
        All = Track | Scroll | Move | Link | Copy // 0x0000001F
    }
}