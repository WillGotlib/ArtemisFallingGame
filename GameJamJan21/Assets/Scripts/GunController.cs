using System;
using Unity.VisualScripting;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject bulletType;


    public int maxAmmo = 12;
    public int maxBouncers = 1;
    [SerializeField] private float primaryCooldown = 0.3f;
    private bool primaryOnCooldown = false;
    private float primaryCooldownTimer;
    [SerializeField] private float secondaryCooldown = 1f;
    private bool secondaryOnCooldown = false;
    private float secondaryCooldownTimer;
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
        primaryCooldownTimer = primaryCooldown;
        secondaryCooldownTimer = secondaryCooldown;
        _trajectory.RegisterScene();
    }

    void Update() {
        _trajectory.SimulateTrajectory(this);
        if (primaryOnCooldown) {
            primaryCooldownTimer -= Time.deltaTime;
            if (primaryCooldownTimer <= 0) {
                Debug.Log("PRIMARY COOLDOWN PERIOD COMPLETED");
                primaryOnCooldown = false;
                primaryCooldownTimer = primaryCooldown;
            }
        }
        // if (secondaryOnCooldown) {
        //     secondaryCooldownTimer -= Time.deltaTime;
        //     if (secondaryCooldownTimer <= 0) {
        //         secondaryOnCooldown = false;
        //         secondaryCooldownTimer = secondaryCooldown;
        //     }
        // }

    }

    // returns true if fired
    public bool PrimaryFire()
    {
        Debug.Log("PRIMARY COOLDOWN: " + primaryCooldownTimer);
        if (primaryOnCooldown) {
            Debug.Log("Tried to primary fire, but cooldown has not completed yet.");
        } else {
            Debug.Log("bullet animation");
            GameObject bullet = UnityEngine.Object.Instantiate(bulletType);
            Vector3 cur_pos = this.transform.position + this.transform.forward;
            bullet.transform.position = cur_pos;
            bullet.transform.rotation = this.transform.rotation;
            bullet.GetComponent<BulletLogic>().setShooter(owner);
            bullet.GetComponent<BulletLogic>().Fire(this.transform.forward, false);
            primaryOnCooldown = true;
            return true;
        }
        return false;
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
