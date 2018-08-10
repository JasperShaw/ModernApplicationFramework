namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal interface INewHiddenRegion
    {
        int Type { get; }

        uint Behavior { get; }

        uint State { get; }

        TextSpan HiddenText { get; }

        string Banner { get; }

        uint ClientData { get; }

        uint Length { get; }

        uint[] BannerAttr { get; }
    }
}