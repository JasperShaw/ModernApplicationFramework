namespace ModernApplicationFramework.Input
{
    /// <summary>
    /// Specifies the validation mode for a <see cref="KeySequence"/>
    /// While <see cref="FirstSequence"/> uses the default windows rules as used in <see cref="KeyGesture"/>
    /// <see cref="SecondSequence"/> allows a bunch more combinations in order to support multi key gestures
    /// </summary>
    public enum ValidCheckMode
    {
        FirstSequence,
        SecondSequence
    }
}