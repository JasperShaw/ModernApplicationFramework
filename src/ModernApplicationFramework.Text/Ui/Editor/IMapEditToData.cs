namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface IMapEditToData
    {
        int MapEditToData(int editPoint);

        int MapDataToEdit(int dataPoint);
    }
}