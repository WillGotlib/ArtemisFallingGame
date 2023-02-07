using Online;
using UnityEngine;

public abstract class NetworkedObject : MonoBehaviour, NetworkedElement
{
    public bool controlled;
    private string _networkTypeId;
    public bool removeOnDisconnect;

    protected NetworkedObject(string id)
    {
        _networkTypeId = id;
    }
    
    public abstract string Data();
    public abstract void Destroy();
    public abstract (Vector3, Quaternion) GetPosition();
    public abstract void HandleUpdate(Vector3 position, Quaternion rotation, string data);

    public ElementType GetControlType()
    {
        return controlled ? ElementType.Owner : ElementType.Listener;
    }

    public string ID()
    {
        return _networkTypeId;
    }

    public bool RemoveOnDisconnect()
    {
        return removeOnDisconnect;
    }
}