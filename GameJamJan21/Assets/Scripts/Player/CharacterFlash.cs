using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFlash : MonoBehaviour
{

    private PlayerColourizer _colourizer;
    
    [Tooltip("Colours to switch to when damaged")]
    [SerializeField] private Color damageColour;
    [SerializeField] private Color invincibleColour;

    [Tooltip("Duration of the flash.")]
    [SerializeField] private float damageDuration;
    [SerializeField] private float invincibleDuration;

    private void Awake()
    {
        _colourizer = GetComponent<PlayerColourizer>();
    }

    // defualt colours
    private Color _originalPrimary;
    private Color _originalSecondary;

    // The currently running coroutine.
    private Coroutine flashRoutine;

    public void DamageFlash()
    {
        Flash(damageColour, damageDuration);
    }
    
    public void InvincibilityFlash()
    {
        Flash(invincibleColour, invincibleDuration);
    }

    private void Flash(Color col, float duration) {
        if (flashRoutine == null)
        {
            _originalPrimary = _colourizer.PrimaryColour;
            _originalSecondary = _colourizer.SecondaryColour;
            flashRoutine = StartCoroutine(FlashRoutine(col, duration));
            return;
        }

        if (col != damageColour) {
                return; // We can let this happen if it's damage but not invincibility.
        }
        StopCoroutine(flashRoutine);
        flashRoutine = null;
        // return colours to default on cancel
        _colourizer.PrimaryColour = _originalPrimary;
        _colourizer.SecondaryColour = _originalSecondary;
    }

    private IEnumerator FlashRoutine(Color col, float duration)
    {
        _colourizer.PrimaryColour = col;
        _colourizer.SecondaryColour = col;

        // Pause the execution of this function for "duration" seconds.
        yield return new WaitForSeconds(duration);

        _colourizer.PrimaryColour = _originalPrimary;
        _colourizer.SecondaryColour = _originalSecondary;

        yield return new WaitForSeconds(duration);

        // Set the flashRoutine to null, signaling that it's finished.
        flashRoutine = null;
    }
}
