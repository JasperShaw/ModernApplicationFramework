namespace ModernApplicationFramework.TextEditor
{
    internal interface INormalizeNewLines
    {
        int NormalizeNewlines(uint e);

        int IsBufferNormalized();
    }
}