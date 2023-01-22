using System;
using Unity.VisualScripting;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public int maxAmmo = 12;
    public int maxBouncers = 1;
    // not serialized so that other things can read and update this, ammo refills or the ui that shows ammo
    [NonSerialized] public int ammoCount;
    [NonSerialized] public int bouncingCount;

    [Header("Object values")] public Animator animationController;
    
    void Start()
    {
        ammoCount = maxAmmo;
        bouncingCount = maxBouncers;
    }

    void Update()
    {
        
    }

    // returns true if fired
    public bool PrimaryFire()
    {
        Debug.Log("bullet animation");
        return true;
    }
    
    public bool SecondaryFire()
    {
        Debug.Log("ricochet bullet animation");
        return true;
    }
}
