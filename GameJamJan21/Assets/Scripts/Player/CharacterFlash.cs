using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFlash : MonoBehaviour
{
    
    [SerializeField] private Material normalMaterial;
    [Tooltip("Material to switch to when damaged")]
    [SerializeField] private Material damageMaterial;
    [SerializeField] private Material invincibleMaterial;

    [Tooltip("Duration of the flash.")]
    [SerializeField] private float damageDuration;
    [SerializeField] private float invincibleDuration;

    // The SpriteRenderer that should flash.
    private MeshRenderer meshRenderer;

    // The currently running coroutine.
    private Coroutine flashRoutine;

    void Start()
    {
        // Get the SpriteRenderer to be used,
        // alternatively you could set it from the inspector.
        meshRenderer = GetComponent<MeshRenderer>();

        // Copy the material so it can be modified without any side effects.
        damageMaterial = new Material(damageMaterial);
        invincibleMaterial = new Material(invincibleMaterial);
        
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
        if (flashRoutine != null)
        {
            if (mat != damageMaterial) {
                return; // We can let this happen if it's damage but not invincibility.
            }
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }
        if (flashRoutine == null) {
            flashRoutine = StartCoroutine(FlashRoutine(mat, duration));
        }
    }

    private IEnumerator FlashRoutine(Material mat, float duration)
    {
        // Swap to the flashMaterial.
        meshRenderer.material = mat;

        // Pause the execution of this function for "duration" seconds.
        yield return new WaitForSeconds(duration);

        // After the pause, swap back to the original material.
        meshRenderer.material = normalMaterial;
        
        yield return new WaitForSeconds(duration);

        // Set the flashRoutine to null, signaling that it's finished.
        flashRoutine = null;
    }
}
