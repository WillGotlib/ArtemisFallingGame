using System.Collections;
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
    private Coroutine _flashRoutine;

    public void DamageFlash()
    {
        Flash(damageColour, damageDuration);
    }
    
    public void InvincibilityFlash()
    {
        Flash(invincibleColour, invincibleDuration);
    }

    private void Flash(Color col, float duration) {
        if (_flashRoutine != null)
            return;
        
            
        _originalPrimary = _colourizer.PrimaryColour;
        _originalSecondary = _colourizer.SecondaryColour;
        _flashRoutine = StartCoroutine(FlashRoutine(col, duration));
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
        _flashRoutine = null;
    }
}
