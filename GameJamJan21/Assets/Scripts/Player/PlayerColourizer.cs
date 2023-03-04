using System.Collections.Generic;
using UnityEngine;

public class PlayerColourizer : MonoBehaviour
{
    [SerializeField] private Transform model;
    [SerializeField] private Material primary;
    [SerializeField] private Material secondary;
    [SerializeField] private string colourAttribute = "_Colour";

    private Color _primaryColour = Color.clear;
    private Color _secondaryColour = Color.clear;

    public Color PrimaryColour
    {
        get => _primaryColour;
        set
        {
            _primaryMat?.SetColor(_colourAttributeId, value);
            _primaryColour = value;
        }
    }

    public Color SecondaryColour
    {
        get => _secondaryColour;
        set
        {
            _secondaryMat?.SetColor(_colourAttributeId, value);
            _secondaryColour = value;
        }
    }

    private Material _primaryMat;
    private Material _secondaryMat;
    private int _colourAttributeId;

    public void Start()
    {
        _colourAttributeId = Shader.PropertyToID(colourAttribute);
        _primaryMat = Instantiate(primary);
        _secondaryMat = Instantiate(secondary);
        
        foreach (var renderer in model.GetComponentsInChildren<MeshRenderer>())
        {
            var materials = renderer.materials;
            for (var i = 0; i < materials.Length; i++)
            {
                var material = materials[i];
                var mattName = material.name.Substring(0,
                    material.name.Length - " (Instance)".Length); // probably not the best way to do this

                if (mattName == primary.name || material.name == primary.name)
                {
                    materials[i] = _primaryMat;
                }
                else if (mattName == secondary.name || material.name == secondary.name)
                {
                    materials[i] = _secondaryMat;
                }
                else
                {
                    Debug.Log($"material not matched: {material.name}");
                }
            }

            renderer.materials = materials;
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
}