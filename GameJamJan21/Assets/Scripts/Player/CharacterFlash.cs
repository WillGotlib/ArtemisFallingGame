using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFlash : MonoBehaviour
{

    [SerializeField] private Transform model;
    
    [Tooltip("Material to switch to when damaged")]
    [SerializeField] private Material damageMaterial;
    [SerializeField] private Material invincibleMaterial;

    [Tooltip("Duration of the flash.")]
    [SerializeField] private float damageDuration;
    [SerializeField] private float invincibleDuration;

    // defualt materials
    public Dictionary<MeshRenderer,Material> renderersDefaultMatts = new ();

    // The currently running coroutine.
    private Coroutine flashRoutine;

    void Start()
    {
        // Get the SpriteRenderer to be used,
        // alternatively you could set it from the inspector.
        var meshRenderers = model.transform.GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
            renderersDefaultMatts[meshRenderer] = new Material(meshRenderer.material);
        }

        // Copy the material so it can be modified without any side effects.
        damageMaterial = new Material(damageMaterial);
        invincibleMaterial = new Material(invincibleMaterial);
        
    }

    public void SetModel(Transform model) {
        model = model;
    }

    public void DamageFlash()
    {
        Flash(damageMaterial, damageDuration);
    }
    
    public void InvincibilityFlash()
    {
        Flash(invincibleMaterial, invincibleDuration);
    }

    private void Flash(Material mat, float duration) {
        if (flashRoutine == null)
        {
            flashRoutine = StartCoroutine(FlashRoutine(mat, duration));
            return;
        }

        if (mat != damageMaterial) {
                return; // We can let this happen if it's damage but not invincibility.
        }
        StopCoroutine(flashRoutine);
        flashRoutine = null;
        // return material to default on cancel
        foreach (var (renderer, material) in renderersDefaultMatts)
        {
            print(renderer.name + " " + material.name);
            renderer.material = material;
        }
    }

    private IEnumerator FlashRoutine(Material mat, float duration)
    {
        foreach (var (renderer, _) in renderersDefaultMatts)
        {
            // Swap to the flashMaterial.
            renderer.material = mat;
        }

        // Pause the execution of this function for "duration" seconds.
        yield return new WaitForSeconds(duration);

        // After the pause, swap back to the original material.
        foreach (var (renderer, material) in renderersDefaultMatts)
        {
            renderer.material = material;
        }
        
        yield return new WaitForSeconds(duration);

        // Set the flashRoutine to null, signaling that it's finished.
        flashRoutine = null;
    }
}
