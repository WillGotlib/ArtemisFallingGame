using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllScheme : MonoBehaviour
{
    [SerializeField] private int _currentController;

    public GameObject[] controllers;

    [Header("Labels")] public Transform shoot;
    public Transform secondary;
    public Transform dash;
    public Transform move;
    public Transform look;

    [Header("positions")] public Vector2[] shootPositions;
    public Vector2[] secondaryPositions;
    public Vector2[] dashPositions;
    public Vector2[] movePositions;
    public Vector2[] lookPositions;

    public int CurrentController
    {
        private get => _currentController;
        set
        {
            _currentController = value;
            if (_currentController < 0) _currentController = 0;
            if (_currentController >= controllers.Length) _currentController = controllers.Length-1;
            
            UpdatePositions();
        }
    }
    
    private void Awake()
    {
        var len = controllers.Length;
        if (shootPositions.Length != len && secondaryPositions.Length != len && dashPositions.Length != len &&
            movePositions.Length != len && lookPositions.Length != len)
        {
            throw new Exception("all lists must be the same length");
        }

        if (_currentController < 0) _currentController = 0;
        if (_currentController >= len) _currentController = len-1;

        UpdatePositions();
    }

    private void UpdatePositions()
    {
        for (var i = 0; i < controllers.Length; i++)
        {
            controllers[i].SetActive(i == _currentController);
        }

        secondary.localPosition =secondaryPositions[_currentController];
        shoot.localPosition =shootPositions[_currentController];
        dash.localPosition = dashPositions[_currentController];
        move.localPosition = movePositions[_currentController];
        look.localPosition =lookPositions[_currentController];
    }
}