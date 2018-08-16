using System;
using System.Windows.Media;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    internal sealed class SolidColorBrushSelectionPainter : BrushSelectionPainter
    {
        public SolidColorBrushSelectionPainter(IAdornmentLayer adornmentLayer, IMultiSelectionBroker broker)
            : base(adornmentLayer, broker)
        {
        }

        public override void Activate()
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Update(bool selectionChanged)
        {
            throw new NotImplementedException();
        }

        internal Brush Brush { get; private set; }
    }
}