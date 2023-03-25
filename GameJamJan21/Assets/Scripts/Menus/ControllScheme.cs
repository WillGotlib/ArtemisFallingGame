using System;
using UnityEngine;

public class ControllScheme : MonoBehaviour
{
    [SerializeField] private CurrentControlScheme controlScheme;

    [Tooltip("must be in the order of the enum")]
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

    private void Awake()
    {
        var len = controllers.Length;
        if (shootPositions.Length != len || secondaryPositions.Length != len || dashPositions.Length != len ||
            movePositions.Length != len || lookPositions.Length != len || pausePositions.Length!=len)
        {
            throw new Exception("all lists must be the same length");
        }

        controlScheme.RegisterListener(gameObject);
        OnSchemeChange(controlScheme.ControlScheme);
    }

    public void OnSchemeChange(ControlSchemes currentController)
    {
        UpdatePositions((int)currentController);
    }

    private void UpdatePositions(int currentController)
    {
        for (var i = 0; i < controllers.Length; i++)
        {
            controllers[i].SetActive(i == currentController);
        }

        secondary.localPosition = secondaryPositions[currentController];
        shoot.localPosition = shootPositions[currentController];
        dash.localPosition = dashPositions[currentController];
        move.localPosition = movePositions[currentController];
        look.localPosition = lookPositions[currentController];
        pause.localPosition = pausePositions[currentController];
    }
}