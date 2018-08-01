namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal abstract class SimpleTextViewWindow : ICommandTarget, ITypedTextTarget, ICommandTargetInner, /*IBackForwardNavigation*/ IMafTextView
    {
        public bool InProvisionalInput { get; set; }

        public int QueryStatus()
        {
            return 0;
        }

        public int Exec()
        {
            return 0;
        }

        public int InnerExec()
        {
            return 0;
        }

        public int InnerQueryStatus()
        {
            return 0;
        }
    }

    public interface IMafTextView
    {
    }
}
