using Google.Protobuf;

namespace Analytics
{
    public interface ITrackableScript
    {
        public ByteString GetAnalyticsFields();
        public string GetAnalyticsName();
    }
}
