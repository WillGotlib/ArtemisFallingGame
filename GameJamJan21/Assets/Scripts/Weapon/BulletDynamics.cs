using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDynamics : MonoBehaviour
{
    [SerializeField] private GameObject outerBullet;
    private Material outerMaterial;
    private MeshRenderer outerRenderer;

    private string colourAttribute = "_Color";

    // Start is called before the first frame update
    void Start()
    {
        outerRenderer = outerBullet.GetComponent<MeshRenderer>();
        outerMaterial = new Material(outerRenderer.material);
        // print(outerMaterial.name);
        // outerMaterial = Instantiate(outerMaterial); // So we can edit it safely.
        BulletBrighten(0f);
        // outerRenderer.material = outerMaterial;
    }

    public void BulletBrighten(float percentage) {
        Color temp = outerMaterial.GetColor(colourAttribute);
        temp.a = percentage;
        // print("Brightening bullet -> alpha: " + temp.a);
        outerMaterial.color = temp;
        outerRenderer.material = outerMaterial;
    }
}
