namespace ModernApplicationFramework.Editor
{
    internal interface INormalizeNewLines
    {
        int NormalizeNewlines(uint e);

        int IsBufferNormalized();
    }
}