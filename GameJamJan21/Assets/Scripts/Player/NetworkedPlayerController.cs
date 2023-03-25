using System;
using Online;
using UnityEngine;

public class NetworkedPlayerController : NetworkedObject, NetworkedElement
{
    private Controller _playerController;

    private void Start()
    {
        _playerController = GetComponent<Controller>();
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
    private float lerpSpeed;

    private void Awake()
    {
        var n = FindObjectOfType<NetworkManager>();
        var fps = n == null ? 1 : n.updateFps;
        lerpSpeed = 1f / fps;
    }

    public override void HandleUpdate(Vector3 position, Quaternion rotation, string data)
    {
        startPos = transform.position;
        targetPos = position;
        fraction = 0;
        
        _playerController.lookDirection = rotation * Vector3.forward; //todo maybe use this for movement too
    }

    public void Update()
    {
        if (controlled) return;
        if (fraction > 1) return;

        fraction += Time.deltaTime / lerpSpeed;
        transform.position = Vector3.Lerp(startPos, targetPos, fraction);
    }
}