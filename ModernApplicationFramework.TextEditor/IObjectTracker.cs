namespace ModernApplicationFramework.TextEditor
{
    public interface IObjectTracker
    {
        void TrackObject(object value, string bucketName);
    }
}