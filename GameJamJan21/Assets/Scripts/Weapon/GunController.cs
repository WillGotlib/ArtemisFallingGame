using System;
using Unity.VisualScripting;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] private GameObject bulletType;
    [SerializeField] private GameObject secondaryType;


    public int maxAmmo = 12;
    public int maxBouncers = 1;
    private float primaryCooldown;
    private bool primaryOnCooldown = false;
    private float primaryCooldownTimer;
    private float secondaryCooldown;
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
        primaryCooldown = bulletType.GetComponent<BulletLogic>().cooldown;
        primaryCooldownTimer = primaryCooldown;
        secondaryCooldown = secondaryType.GetComponent<BulletLogic>().cooldown;
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
                primaryCooldownTimer = primaryCooldown / owner.GetFireRateBonus();
            }
        }
        if (secondaryOnCooldown) {
            secondaryCooldownTimer -= Time.deltaTime;
            if (secondaryCooldownTimer <= 0) {
                secondaryOnCooldown = false;
                secondaryCooldownTimer = secondaryCooldown;
            }
        }

    }

    public void ClearPrimaryCooldown() {
        primaryOnCooldown = false;
        primaryCooldownTimer = primaryCooldown / owner.GetFireRateBonus();
    }

    public bool CheckShotValidity(Vector3 cur_pos) {
        if (!Physics.CheckBox(cur_pos, new Vector3(0.05f, 0.05f, 0.1f)) && 
            !Physics.CheckBox(transform.position, new Vector3(0.1f, 0.1f, 0.1f))) // TODO: Values are arbitrary; should be figured out
        {
            return true;
        }
        var overlaps = Physics.OverlapSphere(cur_pos, 0.5f);
        foreach (Collider overlapper in overlaps) {
            if (overlapper.gameObject.tag == "Reflector" || overlapper.gameObject.tag == "Player") {
                // TODO: Is this too restrictive? 
                return false;
            }
        }
        return true;
    }

    // returns true if fired
    public void PrimaryFire()
    {
        // Debug.Log("PRIMARY COOLDOWN: " + primaryCooldownTimer);
        if (primaryOnCooldown) {
            Debug.Log("Tried to primary fire, but cooldown has not completed yet.");
            return;
        } 
        Vector3 cur_pos = this.transform.position + (this.transform.forward / 3);

        // Check to make sure we aren't colliding
        if (CheckShotValidity(cur_pos)) {
            GameObject bullet = UnityEngine.Object.Instantiate(bulletType);
            bullet.transform.position = cur_pos;
            bullet.transform.rotation = this.transform.rotation;
            bullet.GetComponent<BulletLogic>().setShooter(owner);
            bullet.GetComponent<BulletLogic>().Fire(this.transform.forward * 2, false);
            primaryOnCooldown = true;
        } else {
            Debug.Log("Bullet would appear inside an object!");
        }
    }
    
    public bool SecondaryFire()
    {
        Debug.Log("Secondary Fire");
        if (secondaryOnCooldown) { return false; }
        GameObject grenade = UnityEngine.Object.Instantiate(secondaryType);
        Vector3 cur_pos = this.transform.position + (this.transform.forward / 3);
        grenade.transform.position = cur_pos;
        grenade.transform.rotation = this.transform.rotation;
        grenade.GetComponent<BulletLogic>().setShooter(owner);
        grenade.GetComponent<BulletLogic>().Fire(this.transform.forward, false);
        secondaryOnCooldown = true;
        return true;
    }

    public void setOwner(Controller player) {
        owner = player;
    }
}
