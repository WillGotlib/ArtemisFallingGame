using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenadeColour : MonoBehaviour
{
    [SerializeField] private Color colour;

    public Color Colour
    {
        set
        {
            colour = value;
            _sharedMat.color = value;
        }
        get => colour;
    }

    [SerializeField] private Transform grenade;
    [SerializeField] private Material colourMaterial;
    private Material _sharedMat;
    
    void Awake()
    {
        _sharedMat = Instantiate(colourMaterial);
        foreach (var obj in grenade.GetComponentsInChildren<MeshRenderer>())
        {
            if (obj.sharedMaterial == colourMaterial)
            {
                obj.material = _sharedMat;
            }
        }

        // update it
        Colour = colour;
    }
}
