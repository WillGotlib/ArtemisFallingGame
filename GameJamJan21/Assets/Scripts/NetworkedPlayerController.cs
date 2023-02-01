using Online;
using UnityEngine;

public class NetworkedPlayerController : MonoBehaviour, NetworkedElement
{
    public bool controlled = false;
    public string networkTypeId;
    public bool removeOnDisconnect = true;

    public string Data()
    {
        return "";
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public ElementType GetControlType()
    {
        return controlled ? ElementType.Owner : ElementType.Listener;
    }

    public (Vector3,Quaternion) GetPosition()
    {
        return (transform.position, transform.rotation);
    }

    public void HandleUpdate(Vector3 position, Quaternion rotation, string data)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    public string ID()
    {
        return networkTypeId;
    }

    public bool RemoveOnDisconnect()
    {
        return removeOnDisconnect;
    }
}