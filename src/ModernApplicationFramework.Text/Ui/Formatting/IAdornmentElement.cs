using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface IAdornmentElement : ISequenceElement
    {
        PositionAffinity Affinity { get; }

        double Baseline { get; }

        double BottomSpace { get; }

        object IdentityTag { get; }

        object ProviderTag { get; }

        double TextHeight { get; }

        double TopSpace { get; }
        double Width { get; }
    }
}