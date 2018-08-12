namespace ModernApplicationFramework.Editor.Implementation
{

    //TODO: Add implementations
    public interface IObscuringTip
    {
        bool Dismiss();

        double Opacity { get; }

        void SetOpacity(double opacity);
    }
}