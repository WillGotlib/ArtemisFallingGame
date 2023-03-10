using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationUtils : MonoBehaviour
{
    private DashJets _jets;

    // private Controller _playerController;
    private Animator _animator;

    private bool _holdPosition = true;

    void Awake()
    {
        _jets = GetComponentInParent<DashJets>();
        _animator = GetComponent<Animator>();
        // _playerController = GetComponentInParent<Controller>();
    }

    private bool _transition;


    private void LateUpdate()
    {
        // catch the falling edge
        var t = _animator.IsInTransition(0);
        if (!t && _transition)
        {
            // in the case an animation was quit early
            Debug.Log("switched state");
            _holdPosition = true;
            JetOff();
        }
        _transition = t;

        
        if (_holdPosition)
        {
            transform.localPosition = Vector3.zero; // take into account root motion
        }
    }

    public void JetOn()
    {
        _jets.Shoot();
    }

    public void JetOff()
    {
        _jets.Stop();
    }

    public void AllowAnimationMovement(int movement)
    {
        _holdPosition = movement == 0;
    }
}