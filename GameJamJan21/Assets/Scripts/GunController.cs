using System;
using Unity.VisualScripting;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject bulletType;


    public int maxAmmo = 12;
    public int maxBouncers = 1;
    // not serialized so that other things can read and update this, ammo refills or the ui that shows ammo
    [NonSerialized] public int ammoCount;
    [NonSerialized] public int bouncingCount;

    [Header("Object values")] public Animator animationController;
    private PlayerController owner;

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
        GameObject bullet = UnityEngine.Object.Instantiate(bulletType);
        Vector3 cur_pos = this.transform.position + this.transform.forward;
        bullet.transform.position = cur_pos;
        bullet.transform.rotation = this.transform.rotation;
        bullet.GetComponent<BulletFire>().setShooter(owner);
        bullet.GetComponent<BulletFire>().Fire(this.transform.forward);
        return true;
    }
    
    public bool SecondaryFire()
    {
        Debug.Log("ricochet bullet animation");
        return true;
    }

    public void setOwner(PlayerController player) {
        owner = player;
    }
}
