using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerColourizer : MonoBehaviour
{
    [SerializeField] private Transform model;
    [SerializeField] private Material primary;
    [SerializeField] private Material secondary;
    [SerializeField] private string colourAttribute = "_Colour";

    private Color _primaryColour =Color.clear;
    private Color _secondaryColour=Color.clear;
    public Color PrimaryColour
    {
        get => _primaryColour;
        set
        {
            UpdateColour(_primaryMats, value);
            _primaryColour = value;
        }
    }

    public Color SecondaryColour {
        get => _secondaryColour;
        set
        {
            UpdateColour(_secondaryMats, value);
            _secondaryColour = value;
        }
    }

    private readonly List<Material> _primaryMats = new();
    private readonly List<Material> _secondaryMats = new();

    public void Start()
    {
        foreach (var renderer in model.GetComponentsInChildren<MeshRenderer>())
        {
            var material = renderer.material;
            var name = material.name.Substring(0,
                material.name.Length - " (Instance)".Length); // probably not the best way to do this

            if (name == primary.name)
            {
                _primaryMats.Add(material);
            }

            if (name == secondary.name)
            {
                _secondaryMats.Add(material);
            }
        }

        if (_primaryColour != Color.clear)
        {
            PrimaryColour = _primaryColour;
        }

        if (_secondaryColour != Color.clear)
        {
            SecondaryColour = _secondaryColour;
        }
    }

    private void UpdateColour(List<Material> materials, Color colour)
    {
        var colourId = Shader.PropertyToID(colourAttribute);
        foreach (var material in materials)
        {
            material.SetColor(colourId,colour);
        }
    }
}