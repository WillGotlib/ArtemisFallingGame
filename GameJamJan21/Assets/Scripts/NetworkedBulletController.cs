using Online;
using UnityEngine;

public class NetworkedBulletController : NetworkedObject, NetworkedElement
{
    private NetworkedBulletController() : base("BULLET")
    {
    }

    public override string Data()
    {
        return ""; //todo send velocity to other bullets on wall change
    }

    public override void Destroy()
    {
        if (!controlled) Destroy(gameObject); // have the person who fired the bullet handle killing
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