using System.Collections.Generic;
using UnityEngine;

public class PlayerColourizer : MonoBehaviour
{
    [SerializeField] private Transform model;
    [SerializeField] private Material primary;
    [SerializeField] private Material secondary;
    [SerializeField] private string colourAttribute = "_Colour";

    private Color _primaryColour;
    private Color _secondaryColour;

    public Color PrimaryColour
    {
        get => _primaryColour;
        set => _primaryMat?.SetColor(_colourAttributeId, _primaryColour = value);
    }

    public Color SecondaryColour
    {
        get => _secondaryColour;
        set => _secondaryMat?.SetColor(_colourAttributeId, _secondaryColour = value);
}

    private Material _primaryMat;
    private Material _secondaryMat;
    private int _colourAttributeId;

    public void initialColourize()
    {
        _colourAttributeId = Shader.PropertyToID(colourAttribute);
        _primaryMat = Instantiate(primary);
        _secondaryMat = Instantiate(secondary);
        
        foreach (var renderer in model.GetComponentsInChildren<MeshRenderer>())
        {
            var materials = renderer.sharedMaterials;
            for (var i = 0; i < materials.Length; i++)
            {
                var material = materials[i];

                if (material == primary)
                {
                    materials[i] = _primaryMat;
                }
                else if (material == secondary)
                {
                    materials[i] = _secondaryMat;
                }
                else
                {
                    Debug.Log($"material not matched on {renderer.gameObject.name}: {material.name}");
                }
            }

            renderer.materials = materials;
        }

        
        _primaryColour = primary.GetColor(_colourAttributeId);
        _secondaryColour = secondary.GetColor(_colourAttributeId);
    }
}