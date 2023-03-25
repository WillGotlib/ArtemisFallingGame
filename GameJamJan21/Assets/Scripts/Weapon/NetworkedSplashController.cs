using System;
using System.Text;
using Online;
using Unity.Mathematics;
using UnityEngine;

public class NetworkedSplashController : NetworkedObject, NetworkedElement
{
    private SplashZone _splashZone;
    
    private void Awake()
    {
        _splashZone = GetComponent<SplashZone>();
    }

    public override string Data()
    {
        return Encoding.Default.GetString(BitConverter.GetBytes(_splashZone.timeRemaining));
    }

    public override void Destroy()
    {
        if (!controlled) Destroy(gameObject);
    }

    public override (Vector3, Quaternion) GetPosition()
    {
        return (transform.position, quaternion.identity);
    }

    public override void HandleUpdate(Vector3 position, Quaternion rotation, string data)
    {
        transform.position = position;

        var timeRemaining = BitConverter.ToSingle(Encoding.Default.GetBytes(data));
        _splashZone.timeRemaining = timeRemaining;
    }
}