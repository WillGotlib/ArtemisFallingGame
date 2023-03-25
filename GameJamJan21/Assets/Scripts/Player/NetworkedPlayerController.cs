using System;
using Online;
using UnityEngine;

public class NetworkedPlayerController : NetworkedObject, NetworkedElement
{
    private Controller _playerController;
    private Rigidbody _rb;

    private void Start()
    {
        _playerController = GetComponent<Controller>();
        _rb = GetComponent<Rigidbody>();
    }

    private bool lastDash;
    public override string Data()
    {
        var v = !lastDash && _playerController.Dashing ? "d" : "w";
        
        lastDash = _playerController.Dashing;
        return v;
    }

    public override void Destroy()
    {
        if (!controlled)Destroy(gameObject);
    }

    public override (Vector3, Quaternion) GetPosition()
    {
        return (transform.position, transform.rotation);
    }

    private Vector3 target;

    public override void HandleUpdate(Vector3 position, Quaternion rotation, string data)
    {
        // move to the last position
        // _rb.MovePosition(target - transform.position);
        // update last pos
        target = position;

        _playerController.lookDirection = rotation * Vector3.forward;
        
        if (data == "d")
            _playerController.OnDash();
    }

    public void Update()
    {
        if (controlled) return;
        
        // set the move direction for in between updates
        var m = target - transform.position;
        if (m.sqrMagnitude > .01f)
            _playerController.moveDirection = m.normalized;
        else
            _playerController.moveDirection = Vector3.zero;
    }
}