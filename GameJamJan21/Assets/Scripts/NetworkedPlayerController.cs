using Online;
using UnityEngine;

public class NetworkedPlayerController : NetworkedObject, NetworkedElement
{
    private NetworkedPlayerController() : base("PLAYER")
    {
    }

    public string Data()
    {
        return "";
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public (Vector3, Quaternion) GetPosition()
    {
        return (transform.position, transform.rotation);
    }

    public void HandleUpdate(Vector3 position, Quaternion rotation, string data)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
}