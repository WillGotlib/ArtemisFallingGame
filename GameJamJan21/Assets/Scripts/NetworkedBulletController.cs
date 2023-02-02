using Online;
using UnityEngine;

public class NetworkedBulletController : NetworkedObject, NetworkedElement
{
    private NetworkedBulletController() : base("BULLET")
    {
    }

    public string Data()
    {
        return "";
    }

    public void Destroy()
    {
        if (!controlled) Destroy(gameObject); // have the person who fired the bullet handle killing
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