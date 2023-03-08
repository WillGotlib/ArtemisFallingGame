using System;
using System.Collections;
using System.Collections.Generic;
using Analytics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Object = UnityEngine.Object;


public class Controller : MonoBehaviour
{
    [NonSerialized] public int playerNumber;
    // Number of times a player can die before they are out of the game
    [NonSerialized] public int Stock = GlobalStats.defaultStockCount;

    [Header("Nodes")]
    public CharacterController controller;
    public Animator animator;
    
    [Header("Values")]
    public float speed = 6f;
    public float sensitivity = 5;
    public float kbdSensitivity = 4;
    public GameObject weaponType;
    private GameObject weapon;
    public float playerHealth { get; private set; } = GlobalStats.baseHealth;

    float turnSmoothVelocity;
    Vector3 moveDirection;
    Vector3 lookDirection;
    new Camera camera;
    bool followingCamera = true;
    public PausedMenu menu;

    private string _animationSpeedAttrName = "speed";
    private string _animationDashAttrName = "dashing";

    CameraSwitch cameraController;
    private CharacterFlash flashManager;

    // public float gravity = 0.000001f; // TODO: OK to delete this?
    public float dashIntensity;
    float currentCooldown;

    public float momentum = 0.85f;
    private float startMomentum;
    public float maxMomentum = 1.5f;
    public float dashDuration;
    public GameObject backupCamera;

    private bool currentlyDead;

    // These decrease as time goes on
    private float deathCooldown = GlobalStats.deathCooldown;
    private float invincibilityCooldown;

    private StartGame playerController;
    private List<Effect> effects = new List<Effect>();

    private Vector3 direction;
    private bool kbdHeld;

    private AnalyticsManager _analyticsManager;

    private HUDManager _hudManager;

    private TempLivesManager _tempLivesManager;

    private DashJets _jetParticles;

    // Start is called before the first frame update
    void Start()
    {
        dashDuration = 0.1f;
        dashIntensity = 10f;
        var rotation = Quaternion.AngleAxis(direction.y * kbdSensitivity, Vector3.up);
        lookDirection = rotation * transform.rotation * Vector3.forward;
        
        _analyticsManager = FindObjectOfType<AnalyticsManager>();
        playerController = FindObjectOfType<StartGame>();
        camera = GetComponentInChildren<Camera>();
        _jetParticles = GetComponent<DashJets>();
        cameraController = FindObjectOfType<CameraSwitch>();
        _hudManager = FindObjectOfType<HUDManager>();
        _tempLivesManager = FindObjectOfType<TempLivesManager>();
        flashManager = GetComponent<CharacterFlash>();
        menu = FindObjectOfType<PausedMenu>();
        menu.SwitchMenuState();


        // controller = GetComponent<CharacterController>();
        // controller = gameObject.GetComponent(typeof(CharacterController)) as CharacterController;
        if (camera == null)
        {
            camera = backupCamera.GetComponentInChildren<Camera>();
            followingCamera = false;
        }

        currentCooldown = 0;
        weapon = Instantiate(weaponType, gameObject.transform);
        weapon.transform.localPosition = new Vector3(0.66f, 2f, 1.5f);
        weapon.GetComponent<GunController>().setOwner(this);
        startMomentum = momentum;
        
        _analyticsManager.HealthEvent(gameObject, playerHealth);
        _analyticsManager.StockUpdate(gameObject, Stock);
        
        playerController.PlayerHealthUpdate(playerNumber, playerHealth);
        // playerController.PlayerStockUpdate(playerNumber, ) TODO: Should stocks be stored here too?
    }

    public void OnMovement(InputValue value)
    {
        moveDirection = value.Get<Vector3>();
    }

    public void OnSwitchCamera()
    {
        if (cameraController != null)
        {
            cameraController.SwitchCamera();
        }
    }

    public void OnLook(InputValue value)
    {
        // Read value from control. The type depends on what type of controls.
        // the action is bound to.

        kbdHeld = !kbdHeld;
        direction = value.Get<Vector3>();
    }

    private void UpdateLookDirection()
    {
        if (!kbdHeld) return;

        if (direction.y != 0)
        {
            // Debug.Log(lookDirection);
            var rotation = Quaternion.AngleAxis(direction.y * kbdSensitivity, Vector3.up);
            lookDirection = rotation * transform.rotation * Vector3.forward;
            lookDirection.Normalize();
        }
        else 
            lookDirection = direction.normalized * sensitivity;
    }


    private void FixedUpdate()
    {
        UpdateLookDirection();
    }

    public void OnPrimaryFire()
    {
        if (!currentlyDead)
        {
            weapon.GetComponent<GunController>().PrimaryFire();
        }
    }

    public void OnSecondaryFire()
    {
        if (!currentlyDead)
        {
            weapon.GetComponent<GunController>().SecondaryFire();
        }
    }

    public void OnEnterMenu() {
        menu.SwitchMenuState();
    }

    public void OnDash()
    {
        if (currentCooldown <= 0)
        {
            currentCooldown = GlobalStats.dashCooldown;
            _hudManager.UseStamina(playerNumber);
            StartCoroutine(Dash());
        }
        else
        {
            // print("Dash on cooldown!");
        }
    }

    IEnumerator Dash() {
        float startTime = Time.time;
        _jetParticles.Shoot();
        animator.SetBool(_animationDashAttrName,true);

        while (Time.time < startTime + dashDuration) {
            if (moveDirection.magnitude > 0) {
                controller.Move(moveDirection.normalized * Time.deltaTime * dashIntensity * GetDashBonus());    
            }
            else {
                controller.Move(lookDirection * Time.deltaTime * dashIntensity * GetDashBonus());
            }
            yield return null;
        }
        _jetParticles.Stop();
        animator.SetBool(_animationDashAttrName,false);
    }

    // Update is called once per frame
    void Update()
    {
        // Tick down all effects
        TickDownEffects();

        if (currentlyDead)
        {
            if (transform.position.y != 100)
            {
                // TODO: Un-hard-code this value. Each map should have a "floor" coord?
                print("Not on the right plane:: on life plane");
                transform.position = new Vector3(0, 100, 0);
            }
            
            deathCooldown -= Time.deltaTime;
            if (deathCooldown <= 0)
            {
                if (playerController == null) {
                    var tutorialSpawner = FindObjectOfType<StartTutorial>();
                    tutorialSpawner.RespawnPlayer(this);
                } else {
                    playerController.RespawnPlayer(this);
                }
                // transform.position = pos;
                // print("Player position after respawn is: " + transform.position + ", should be " + pos);
                ResetAttributes();
                _hudManager.ChangeHealth(playerNumber, GlobalStats.baseHealth);
                
                _analyticsManager.RespawnEvent(gameObject);
                _analyticsManager.StockUpdate(gameObject, Stock); // maybe put all these events in the game manager rather then in each player and bullet
                    
                return;
            }
        }
        // ASSERTION: If player gets to this point they are not dead.
        if (invincibilityCooldown > 0) {
            flashManager.InvincibilityFlash();
            invincibilityCooldown -= Time.deltaTime;
        }

        if (currentCooldown > 0)
            currentCooldown -= Time.deltaTime;

        if (followingCamera == true)
            camera.transform.localRotation = Quaternion.Euler(lookDirection);

        if (!controller.isGrounded)
        {
            Vector3 fall = new Vector3(0, -(1), 0);
            controller.Move(fall * Time.deltaTime);
        }
        // if (state != PlayerState.Aiming) {

        if (lookDirection.magnitude >= 0.5f)
        {
            Quaternion newAngle = Quaternion.LookRotation(lookDirection, Vector3.up);
            //print("LOOK VALUE: " + lookDirection + " ADJUSTED ANGLE: " + newAngle);
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, newAngle, sensitivity * Time.deltaTime);
            // this.transform.Rotate(lookDirection);
        }

        animator.SetFloat(_animationSpeedAttrName,moveDirection.magnitude);
        if (!currentlyDead && moveDirection.magnitude >= 0.1f)
        {
            // Handle the actual movement
            moveDirection.y = 0;

            controller.Move((moveDirection).normalized * speed * GetSpeedBonus() * Time.deltaTime * momentum);
            if (momentum < maxMomentum)
                momentum += 0.1f * Time.deltaTime;
        }
        else
        {
            momentum = startMomentum;
        }
    }

    void TickDownEffects()
    {
        foreach (Effect e in new List<Effect>(effects))
        {
            e.TickDown();
            if (e.CheckTimer())
                effects.Remove(e);
        }
    }

    public bool hasEffect(Effect e) {
        return effects.Contains(e);
    }

    public float GetSpeedBonus()
    {
        float totalBonus = 1;
        
        foreach (Effect e in effects)
        {
            totalBonus *= e.speedBonus;
        }

        return totalBonus;
    }

    public float GetFireRateBonus()
    {
        // Not implemented, will require effects to be added to bullets
        float totalBonus = 1;
        foreach (Effect e in effects)
        {
            totalBonus *= e.fireRateBonus;
        }
        return totalBonus;
    }

    float GetDashBonus()
    {
        float totalBonus = 1;
        foreach (Effect e in effects)
        {
            totalBonus += e.dashBonus;
        }

        return totalBonus;
    }

    public bool InflictDamage(float damageAmount)
    {
        if (invincibilityCooldown > 0)
        {
            print("PLAYER TOOK NO DAMAGE.");
            return false;
        }

        if (damageAmount == 0)
        {
            Debug.Log("Direct shot invalidated");
            return false;
        }

        // print("P" + playerNumber + " TOOK " + damageAmount + " dmg >> HP = " + playerHealth);
        playerHealth = Mathf.Max(0, Mathf.Round((playerHealth - damageAmount) * 10) / 10);
        flashManager.DamageFlash();

        // playerController.PlayerHealthUpdate(playerNumber, playerHealth);
        _hudManager.ChangeHealth(playerNumber, playerHealth);
        _analyticsManager.HealthEvent(gameObject, playerHealth);
        if (playerHealth <= 0)
        {
            Debug.Log(playerNumber);
            PlayerDeath();
        }

        return true;
    }

    private void PlayerDeath()
    {
        if (!currentlyDead) {
            currentlyDead = true;
            // Vector3 newPos = this.transform.position += Vector3.up * 10; // TODO: CHANGE THIS. HOW DO WE "DE-ACTIVATE" THE PLAYER
            print("PLAYER DIED");
            // transform.position = transform.position + new Vector3(0, 10, 0);
            // SetActive(false);
            
            _tempLivesManager.ApplyDeath(playerNumber);
            _analyticsManager.DeathEvent(gameObject);
        }
    }

    public void ResetAttributes()
    {
        playerHealth = GlobalStats.baseHealth;
        currentCooldown = GlobalStats.dashCooldown;
        deathCooldown = GlobalStats.deathCooldown;
        invincibilityCooldown = GlobalStats.invincibilityCooldown;
        currentlyDead = false;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Powerup")
        {
            PowerupDrop powerup = collider.gameObject.GetComponent<PowerupDrop>();
            effects.Add(powerup.GiveEffect());
            if (powerup.requiresWeapon) {
                // TODO: Make this generally-applicable. Right now the only weapon powerup is the fire rate one...
                weapon.GetComponent<GunController>().ClearPrimaryCooldown();
            }
            powerup.removePowerup(); //todo make this script trackable and keep trac of powerups
            Destroy(collider.gameObject);
        }
    }

    public void AddEffect(Effect e) {
        effects.Add(e);
    }
}