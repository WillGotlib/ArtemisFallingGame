using UnityEngine;
using UnityEngine.UI;

public class UIControlImage : MonoBehaviour
{
    [SerializeField] private CurrentControlScheme controlScheme;

    [Tooltip("must be in order of ControlScheme enum")] [SerializeField]
    private Sprite[] images;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        controlScheme.RegisterListener(gameObject);
        OnSchemeChange(controlScheme.ControlScheme);
    }

    public void OnSchemeChange(ControlSchemes currentController)
    {
        _image.sprite = images[(int)currentController];
    }
}