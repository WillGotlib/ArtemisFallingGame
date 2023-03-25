using System.Text;
using Google.Protobuf;
using Online;
using protoBuff;
using UnityEngine;

public class NetworkedBulletController : NetworkedObject, NetworkedElement
{
    private BulletLogic _bulletLogic;

    private void Awake()
    {
        _bulletLogic = GetComponent<BulletLogic>();
        if (!controlled)
            _bulletLogic.Fire(Vector3.zero);
    }

    public override string Data()
    {
        return new Position
        {
            Y = _bulletLogic.vel.y,
            X = _bulletLogic.vel.x,
            Z = _bulletLogic.vel.z,
        }.ToByteString().ToBase64();
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
        transform.position = position; // change only move if this is too far from intended location
        transform.rotation = rotation;
        
        var vel = Position.Parser.ParseFrom(ByteString.FromBase64(data));
        _bulletLogic.UpdateVelocity(new Vector3 { x = vel.X, y = vel.Y, z = vel.Z });
    }
}