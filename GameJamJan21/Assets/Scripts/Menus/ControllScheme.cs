using System;
using UnityEngine;

public class ControllScheme : MonoBehaviour
{
    [SerializeField] private int _currentController;

    [SerializeField] private GameObject[] controllers;

    [Header("Labels")]
    [SerializeField] private Transform shoot;
    [SerializeField] private Transform secondary;
    [SerializeField] private Transform dash;
    [SerializeField] private Transform move;
    [SerializeField] private Transform look;
    [SerializeField] private Transform pause;

    [Header("positions")]
    [SerializeField] private Vector2[] shootPositions;
    [SerializeField] private Vector2[] secondaryPositions;
    [SerializeField] private Vector2[] dashPositions;
    [SerializeField] private Vector2[] movePositions;
    [SerializeField] private Vector2[] lookPositions;
    [SerializeField] private Vector2[] pausePositions;

    public int CurrentController
    {
        set
        {
            _currentController = value;
            if (_currentController < 0) _currentController = 0;
            if (_currentController >= controllers.Length) _currentController = controllers.Length - 1;

            UpdatePositions();
        }
    }

    private void Awake()
    {
        var len = controllers.Length;
#if UnityEditor
        if (shootPositions.Length != len || secondaryPositions.Length != len || dashPositions.Length != len ||
            movePositions.Length != len || lookPositions.Length != len || pausePositions.Length!=len)
        {
            throw new Exception("all lists must be the same length");
        }
#endif

        if (_currentController < 0) _currentController = 0;
        if (_currentController >= len) _currentController = len - 1;

        UpdatePositions();
    }

    private void UpdatePositions()
    {
        for (var i = 0; i < controllers.Length; i++)
        {
            controllers[i].SetActive(i == _currentController);
        }

        secondary.localPosition = secondaryPositions[_currentController];
        shoot.localPosition = shootPositions[_currentController];
        dash.localPosition = dashPositions[_currentController];
        move.localPosition = movePositions[_currentController];
        look.localPosition = lookPositions[_currentController];
        pause.localPosition = pausePositions[_currentController];
    }
}