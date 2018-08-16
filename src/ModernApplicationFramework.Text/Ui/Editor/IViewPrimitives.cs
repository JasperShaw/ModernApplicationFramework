namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IViewPrimitives : IBufferPrimitives
    {
        PrimitiveTextView View { get; }

        LegacySelection Selection { get; }

        Caret Caret { get; }
    }
}