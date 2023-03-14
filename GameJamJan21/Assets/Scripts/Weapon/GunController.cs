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
    
    [SerializeField] private float cooldownMultiplier = 1f;
    [SerializeField] private float cooldownIncrements = 0.1f;
    [SerializeField] private float cooldownRecovery = 0.001f;
    private float currentCooldownMultiplier;

    [SerializeField] private float sizeMultiplier = 1f;
    [SerializeField] private float sizeIncrements = 0.1f;
    [SerializeField] private float sizeRecovery = 0.001f;
    private float currentSizeMultiplier;

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
        currentCooldownMultiplier = cooldownMultiplier;
        ResetSizeMultiplier();
        _trajectory.RegisterScene();  
    }

    private int _runCounter;
    void Update()
    {
        _runCounter = (_runCounter + 1) % 4;
        if (_runCounter == 0)
            _trajectory.SimulateTrajectory(this); // todo properly optimise this

        if (primaryOnCooldown) {
            primaryCooldownTimer -= Time.deltaTime * currentCooldownMultiplier;
            if (primaryCooldownTimer <= 0) {
                Debug.Log("PRIMARY COOLDOWN PERIOD COMPLETED");
                primaryOnCooldown = false;
                primaryCooldownTimer = primaryCooldown / owner.GetFireRateBonus();
            }
        } else {
            IncreaseBulletSize();
        }
        if (secondaryOnCooldown) {
            secondaryCooldownTimer -= Time.deltaTime;
            if (secondaryCooldownTimer <= 0) {
                secondaryOnCooldown = false;
                secondaryCooldownTimer = secondaryCooldown;
            }
        }

    }

    void ResetSizeMultiplier() {
        currentSizeMultiplier = sizeMultiplier;
    }

    void DecreaseBulletSize() {
        currentSizeMultiplier -= sizeIncrements;
        if(currentSizeMultiplier <= 0) {
            currentSizeMultiplier = 0.1f;
        }
    }

    void IncreaseBulletSize() {
        if (currentSizeMultiplier < sizeMultiplier)
            currentSizeMultiplier += sizeRecovery;
    }

    void DecreaseCooldownMultiplier() {
        // Currently unused, slows down bullet fire speed
        currentCooldownMultiplier -= cooldownIncrements;
        if(currentCooldownMultiplier <= 0) {
            currentCooldownMultiplier = 0.1f;
        }
    }

    void IncreaseCooldownMultiplier() {
        // Currently unused, speeds up fire speed
        if (currentCooldownMultiplier < cooldownMultiplier)
            currentCooldownMultiplier += cooldownRecovery;
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
            bullet.transform.localScale *= currentSizeMultiplier;
            bullet.transform.position = cur_pos;
            bullet.transform.rotation = this.transform.rotation;
            bullet.GetComponent<BulletLogic>().setShooter(owner);
            bullet.GetComponent<BulletLogic>().Fire(this.transform.forward * 2, false);
            primaryOnCooldown = true;
            DecreaseBulletSize();
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

    public void setSecondary(GameObject newSecondary) {
        secondaryType = newSecondary;
    }
}
