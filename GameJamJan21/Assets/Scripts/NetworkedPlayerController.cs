using System;
using Online;
using UnityEngine;

public class NetworkedPlayerController : MonoBehaviour, NetworkedElement
{
    public bool controlled = false;
    public string networkTypeId;
    public bool removeOnDisconnect = true;

    public void Awake()
    {
        if (!controlled)return;
        FindObjectOfType<NetworkManager>().RegisterObject(this);
    }

    public string Data()
    {
        return "";
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public ElementType GetControlType()
    {
        return controlled ? ElementType.Owner : ElementType.Listener;
    }

    public Vector2 GetPosition()
    {
        var pos = transform.position;
        return new Vector2 { x = pos.x, y = pos.z };
    }

    public void HandleUpdate(Vector2 position, string data)
    {
        transform.position = new Vector3
        {
            x = position.x,
            z = position.y,
        };
    }

    public string ID()
    {
        return networkTypeId;
    }

    public bool RemoveOnDisconnect()
    {
        return removeOnDisconnect;
    }
}