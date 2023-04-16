using System;
using System.Collections.Generic;
using UnityEngine;

public enum ControlSchemes
{
    Gamepad,
    Keyboard1,
    Keyboard2,
}

[CreateAssetMenu(fileName = "Scheme", menuName = "ScriptableObjects/Controllscheme", order = 2)]
public class CurrentControlScheme : ScriptableObject
{
    [SerializeField] private ControlSchemes currentScheme;

    public ControlSchemes ControlScheme
    {
        get => currentScheme;
        set
        {
            currentScheme = value;
            Broadcast();
        }
    }

    private void Awake()
    {
        if (!Application.isEditor) currentScheme = ControlSchemes.Gamepad;
    }

    [NonSerialized] private readonly HashSet<GameObject> _listeners = new();
    
    public void SetControlScheme(string controlSchemeName)
    {
        switch (controlSchemeName)
        {
            case "P1Keyboard":
                ControlScheme = ControlSchemes.Keyboard1;
                break;
            case "P2Keyboard":
                ControlScheme = ControlSchemes.Keyboard2;
                break;
            case "Gamepad2":
            default:
                ControlScheme = ControlSchemes.Gamepad;
                break;
        }
    }

    private void Broadcast()
    {
        foreach (var gameObject in _listeners)
        {
            if (gameObject != null) gameObject.BroadcastMessage("OnSchemeChange", currentScheme);
        }
    }

    public void RegisterListener(GameObject listener)
    {
        _listeners.Add(listener);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Broadcast();
    }
#endif
}
