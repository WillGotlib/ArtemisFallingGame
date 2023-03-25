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
            _primaryColour = value;
            if (_primaryMat) _primaryMat.SetColor(_colourAttributeId, value);
        }
    }

    public Color SecondaryColour
    {
        get => _secondaryColour;
        set
        {
            _secondaryColour = value;
            if (_secondaryMat) _secondaryMat.SetColor(_colourAttributeId, value);
        }
    }

    private Material _primaryMat;
    private Material _secondaryMat;
    private int _colourAttributeId;

    public void initialColourize()
    {
        if (_primaryMat != null) return; // dont init if already inited
        
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

        if (_primaryColour != Color.clear)
            PrimaryColour = _primaryColour;
        else
            _primaryColour = primary.GetColor(_colourAttributeId);
        
        if (_secondaryColour != Color.clear)
            SecondaryColour = _secondaryColour;
        else 
            _secondaryColour = secondary.GetColor(_colourAttributeId);
    }
}