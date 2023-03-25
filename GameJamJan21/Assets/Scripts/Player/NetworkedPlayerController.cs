using System;
using Online;
using UnityEngine;

public class NetworkedPlayerController : NetworkedObject, NetworkedElement
{
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
    private float lerpSpeed;
    private int fps;

    private void Awake()
    {
        var n = FindObjectOfType<NetworkManager>();
        fps = n == null ? 1 : n.updateFps;
        lerpSpeed = 1f / fps;
    }

    public override void HandleUpdate(Vector3 position, Quaternion rotation, string data)
    {
        startPos = transform.position;
        targetPos = position;
        fraction = 0;
        transform.rotation = rotation;
    }

    public void Update()
    {
        if (controlled) return;
        if (fraction > 1) return;

        fraction += Time.deltaTime * lerpSpeed; //todo fix lerp function
        transform.position = Vector3.Lerp(startPos, targetPos, fraction*fps);
    }
}