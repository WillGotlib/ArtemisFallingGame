using Online;
using UnityEngine;

public class NetworkedPlayerController : NetworkedObject, NetworkedElement
{
    private NetworkedPlayerController() : base("PLAYER")
    {
    }

    public override string Data()
    {
        return "";
    }

    public override void Destroy()
    {
        if (!controlled)Destroy(gameObject);
    }

    public override (Vector3, Quaternion) GetPosition()
    {
        return (transform.position, transform.rotation);
    }

    public override void HandleUpdate(Vector3 position, Quaternion rotation, string data)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
}