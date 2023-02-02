using Online;
using UnityEngine;

public class NetworkedObject : MonoBehaviour
{
    public bool controlled;
    private string _networkTypeId;
    public bool removeOnDisconnect;

    public NetworkedObject(string id)
    {
        _networkTypeId = id;
    }

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