using System;
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

    private Vector3 startPos;
    private Vector3 targetPos;
    private float fraction;
    public float lerpSpeed = 1/30f;

    public override void HandleUpdate(Vector3 position, Quaternion rotation, string data)
    {
        startPos = transform.position;
        targetPos = position;
        transform.rotation = rotation;
    }

    public void Update()
    {
        if (controlled) return;
        if (fraction < 1) {
            fraction += Time.deltaTime * lerpSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, fraction);
        }
    }
}