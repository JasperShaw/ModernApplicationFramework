using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface IAdornmentElement : ISequenceElement
    {
        double Width { get; }

        double TopSpace { get; }

        double Baseline { get; }

        double TextHeight { get; }

        double BottomSpace { get; }

        object IdentityTag { get; }

        object ProviderTag { get; }

        PositionAffinity Affinity { get; }
    }
}