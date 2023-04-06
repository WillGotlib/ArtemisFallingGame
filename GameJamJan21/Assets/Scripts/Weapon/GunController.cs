using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GunController : MonoBehaviour
{
    [SerializeField] private GameObject bulletType;
    [SerializeField] private GameObject secondaryType;

    [NonSerialized] public int secondaryAmmoCount = 0;
    
    // Primary and Secondary Cooldown Information. Probably could be stored more gracefully...
    private float primaryCooldown;
    private bool primaryOnCooldown = false;
    private float primaryCooldownTimer;
    private float secondaryCooldown;
    private bool secondaryOnCooldown = false;
    private float secondaryCooldownTimer;
    

    [SerializeField] private Trajectory _trajectory;
    
    [SerializeField] private float cooldownMultiplier = 1f;
    [SerializeField] private float cooldownIncrements = 0.1f;
    [SerializeField] private float cooldownRecovery = 0.001f;
    private float currentCooldownMultiplier;

    [SerializeField] private float sizeMultiplier = 1f;
    [SerializeField] private float sizeIncrements = 0.1f;
    [SerializeField] private float sizeRecovery = 0.001f;
    private float currentSizeMultiplier;
    private float oldSizeMultiplier;
    private bool charged = false;

    // The default is 1f since that will empty the gauge (gauge is 1f - amount passed)
    private float defaultChargeValue = 1f;
    private float currentChargeAmount = 1f;
    [SerializeField] private float chargeIncrements = 0.1f;

    [SerializeField] private float chargeBonus = 5f;

    [Header("Object values")] public Animator animationController;
    public Controller owner;
    // Note: This really could go in Controller.cs, but right now it's just taking care of gun stuff.
    private PlayerHUDManager _hudManagerLocal; 

    void Start()
    {
        primaryCooldown = bulletType.GetComponent<BulletLogic>().cooldown;
        primaryCooldownTimer = primaryCooldown;
        // secondaryCooldown = secondaryType.GetComponent<BulletLogic>().cooldown;
        // secondaryCooldownTimer = secondaryCooldown;
        currentCooldownMultiplier = cooldownMultiplier;
        ResetSizeMultiplier();
        _hudManagerLocal = owner.GetComponent<PlayerHUDManager>();
    }

    private int _runCounter;
    void Update()
    {
        // if (!_trajectory.isRegistered && SceneManager.GetSceneByName("Gameplay").IsValid()) {
        //     _trajectory.RegisterScene();  
        // }
        _runCounter = (_runCounter + 1) % 4;
        // if (_runCounter == 0 && SceneManager.GetSceneByName("Gameplay").IsValid())
        //     _trajectory.SimulateTrajectory(this); // todo properly optimise this

        if (primaryOnCooldown) {
            primaryCooldownTimer -= Time.deltaTime * currentCooldownMultiplier;
            _hudManagerLocal.UpdateCooldownBar(primaryCooldownTimer / primaryCooldown);
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
        var minimumSize = 0.1f;
        currentSizeMultiplier -= sizeIncrements;
        if(currentSizeMultiplier <= 0) {
            currentSizeMultiplier = minimumSize;
        }
    }

    void IncreaseBulletSize() {
        if (currentSizeMultiplier < sizeMultiplier)
            currentSizeMultiplier += sizeRecovery;
    }

    public void ApplyChargeSizeMultiplier() {
        oldSizeMultiplier = currentSizeMultiplier;
        currentSizeMultiplier = sizeMultiplier * 1.3f * chargeBonus;
        charged = true;
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
            if (charged) {
                currentSizeMultiplier = oldSizeMultiplier;
                charged = !charged;
            }
            ClearChargeGUI();
        } else {
            Debug.Log("Bullet would appear inside an object!");
        }
    }
    
    public bool SecondaryFire()
    {
        if (!secondaryType || secondaryAmmoCount <= 0) { print("No secondary to fire."); return false; }
        Debug.Log("Secondary Fire");
        if (secondaryOnCooldown) { return false; }
        
        // Instantiate and set up the projectile.
        GameObject grenade = UnityEngine.Object.Instantiate(secondaryType);
        Vector3 cur_pos = this.transform.position + (this.transform.forward / 3);
        grenade.transform.position = cur_pos;
        grenade.transform.rotation = this.transform.rotation;

        // Make necessary setup calls on the projectile to get it going. Now it's out of our hands.
        grenade.GetComponent<BulletLogic>().setShooter(owner);
        grenade.GetComponent<BulletLogic>().Fire(this.transform.forward, false);

        // Deal with the projectile's continued availability to the player.
        secondaryAmmoCount--;
        if (secondaryAmmoCount <= 0) {
            print($"P{owner.playerNumber} ran out of bullets in their {secondaryType}");
            setSecondary(null, 0);
        } else { secondaryOnCooldown = true; }

        return true;
    }

    public void setOwner(Controller player) {
        owner = player;
    }

    public void setSecondary(GameObject newSecondary, int ammo) {
        secondaryType = newSecondary;
        if (newSecondary) {
            secondaryAmmoCount = ammo;
            secondaryCooldown = newSecondary.GetComponent<BulletLogic>().cooldown;
            secondaryCooldownTimer = secondaryCooldown;
        } else {
            owner.RemoveSecondary();
            secondaryCooldown = 0;
            secondaryCooldownTimer = 0;
            secondaryOnCooldown = false;
        }
    }

    public void UpdateChargeGUI() {
        currentChargeAmount -= chargeIncrements;
        _hudManagerLocal.UpdateChargeBar(currentChargeAmount);
    }

    public void ClearChargeGUI() {
        currentChargeAmount = defaultChargeValue;
        _hudManagerLocal.UpdateChargeBar(defaultChargeValue);
    }
}
