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
    [SerializeField] private Trajectory _trajectory;

    [Header("Object values")] public Animator animationController;
    private Controller owner;

    void Start()
    {
        ammoCount = maxAmmo;
        bouncingCount = maxBouncers;
        _trajectory.RegisterScene();
    }

    void Update() {
        _trajectory.SimulateTrajectory(this);
    }


    // returns true if fired
    public bool PrimaryFire()
    {
        Debug.Log("bullet animation");
        GameObject bullet = UnityEngine.Object.Instantiate(bulletType);
        Vector3 cur_pos = this.transform.position + this.transform.forward;
        bullet.transform.position = cur_pos;
        bullet.transform.rotation = this.transform.rotation;
        bullet.GetComponent<BulletLogic>().setShooter(owner);
        bullet.GetComponent<BulletLogic>().Fire(this.transform.forward, false);
        return true;
    }
    
    public bool SecondaryFire()
    {
        Debug.Log("ricochet bullet animation");
        return true;
    }

    public void setOwner(Controller player) {
        owner = player;
    }
}
