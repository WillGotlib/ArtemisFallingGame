using UnityEditor;
using UnityEngine;

namespace Online
{
    public abstract class NetworkedObject : MonoBehaviour, NetworkedElement
    {
        public bool controlled;
        public string networkTypeId = "";
        public bool removeOnDisconnect;

        public abstract string Data();
        public abstract void Destroy();
        public abstract (Vector3, Quaternion) GetPosition();
        public abstract void HandleUpdate(Vector3 position, Quaternion rotation, string data);

        public ElementType GetControlType()
        {
            return controlled ? ElementType.Owner : ElementType.Listener;
        }

        public bool RemoveOnDisconnect()
        {
            return removeOnDisconnect;
        }

        public string ID()
        {
            return networkTypeId;
        }
    }
}