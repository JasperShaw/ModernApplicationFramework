namespace ModernApplicationFramework.Text.Utilities
{
    public interface IObjectTracker
    {
        void TrackObject(object value, string bucketName);
    }
}