using System.Text;
using Google.Protobuf;
using Online;
using protoBuff;
using UnityEngine;

public class NetworkedBulletController : NetworkedObject, NetworkedElement
{
    private BulletLogic _bulletLogic;

    private NetworkedBulletController() : base("BULLET")
    {
    }

    private void Awake()
    {
        _bulletLogic = GetComponent<BulletLogic>();
    }

    public override string Data()
    {
        return new Position
        {
            Y = _bulletLogic.vel.y,
            X = _bulletLogic.vel.x,
            Z = _bulletLogic.vel.z,
        }.ToByteString().ToString();
    }

    public override void Destroy()
    {
        if (!controlled) Destroy(gameObject); // todo have the person who fired the bullet handle killing
    }

    public override (Vector3, Quaternion) GetPosition()
    {
        // return (Vector3.zero, quaternion.identity);
        return (transform.position, transform.rotation);
    }

    public override void HandleUpdate(Vector3 position, Quaternion rotation, string data)
    {
        transform.position = position;
        transform.rotation = rotation;
        
        var vel = Position.Parser.ParseFrom(Encoding.ASCII.GetBytes(data));
        _bulletLogic.UpdateVelocity(new Vector3 { x = vel.X, y = vel.Y, z = vel.Z });
    }
}