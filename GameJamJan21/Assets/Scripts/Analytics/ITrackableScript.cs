using Google.Protobuf;

namespace Analytics
{
    public interface ITrackableScript
    {
        public ByteString GetFields();
        public string GetName();
    }
}
