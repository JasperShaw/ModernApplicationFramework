namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IViewPrimitives : IBufferPrimitives
    {
        PrimitiveTextView View { get; }

        Selection Selection { get; }

        Caret Caret { get; }
    }
}