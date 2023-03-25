using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phasing : DynamicComponent
{
    private Coroutine phasingRoutine;
    [SerializeField] bool startOut;
    private bool startOutCopy;
    [SerializeField] float inTime;
    [SerializeField] float outTime;

    [SerializeField] Material transparentMaterial;
    public Dictionary<MeshRenderer,Material> renderersDefaultMaterials = new ();


    // Start is called before the first frame update
    void Start()
    {
        startOutCopy = startOut;

        var meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
            renderersDefaultMaterials[meshRenderer] = new Material(meshRenderer.material);
        }
        
        
        phasingRoutine = StartCoroutine(Phase());
    }

    public override void DynamicAction()
    {

    }

    private IEnumerator Phase() {
        if (startOutCopy) {
            foreach (var (renderer, _) in renderersDefaultMaterials)
            {
                renderer.material = transparentMaterial; // Swap to the transparent material everywhere.
                renderer.gameObject.GetComponent<Collider>().enabled = false;
            }
            yield return new WaitForSeconds(outTime);
            startOutCopy = false;
        }
        while (true) {
            // After the pause, swap back to the original material.
            foreach (var (renderer, material) in renderersDefaultMaterials)
            {
                renderer.material = material;
                renderer.gameObject.GetComponent<Collider>().enabled = true;
            }
            yield return new WaitForSeconds(inTime);

            foreach (var (renderer, material) in renderersDefaultMaterials)
            {
                renderer.material = transparentMaterial; // Swap to the transparent material everywhere.
                renderer.gameObject.GetComponent<Collider>().enabled = false;
            }
            yield return new WaitForSeconds(outTime);
        }

    }
}
