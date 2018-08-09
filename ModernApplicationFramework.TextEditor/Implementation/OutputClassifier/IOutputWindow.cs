namespace ModernApplicationFramework.TextEditor.Implementation.OutputClassifier
{
    public interface IOutputWindow
    {
        //TODO: Output interface
        int GetPane(out IOutput ppPane);

        int CreatePane(string pszPaneName, bool visible);

        int DeletePane();
    }
}