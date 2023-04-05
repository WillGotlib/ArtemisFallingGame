using System;
using System.Collections;
using System.Collections.Generic;
using Analytics;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;


public class Controller : MonoBehaviour
{
    [NonSerialized] public int playerNumber;
    // Number of times a player can die before they are out of the game
    [NonSerialized] public int Stock = GlobalStats.defaultStockCount;

    [Header("Nodes")]
    public Rigidbody rb;
    public AnimationUtils animator;
    
    [Header("Values")]
    public float speed = 2f;
    public float sensitivity = 5;
    public float kbdSensitivity = 4;
    public GameObject weaponType;
    private GameObject weapon;
    [SerializeField] private Transform weaponHandBone;
    public float playerHealth { get; private set; } = GlobalStats.baseHealth;

    float turnSmoothVelocity;
    public Vector3 moveDirection;
    public Vector3 lookDirection;
    new Camera camera;
    bool followingCamera = true;
    public PausedMenu menu;
    
    CameraSwitch cameraController;
    private CharacterFlash flashManager;

    // public float gravity = 0.000001f; // TODO: OK to delete this?
    public float dashIntensity = 10;
    float currentCooldown;

    public float momentum = 0.85f;
    private float startMomentum;
    public float maxMomentum = 1.5f;
    public float dashDuration= 0.1f;
    public GameObject backupCamera;

    private bool currentlyDead;

    // These decrease as time goes on
    private float deathCooldown = GlobalStats.deathCooldown;
    private float invincibilityCooldown;

    private StartGame StartGame;
    private List<Effect> effects = new List<Effect>();

    private Vector3 direction;
    private bool kbdHeld;

    private AnalyticsManager _analyticsManager;
    private HUDManager _hudManager;
    
    public MatchDataScriptable mds;

    private DashJets _jetParticles;
    private Collider _capsule;

    static bool menuOnCooldown = false;

    public PlayerInput Input;

    // Start is called before the first frame update
    void Start()
    {
        if (Input.devices.Count == 0) {
            Input.SwitchCurrentControlScheme("P1Keyboard", Keyboard.current);
        }
        var unpaired = InputUser.GetUnpairedInputDevices();
        var ipa = new InputMaster();

        foreach (var devices in unpaired) {
            var scheme = InputControlScheme.FindControlSchemeForDevice(devices, ipa.controlSchemes);

            if (scheme?.name == "Gamepad2") {
                Input.SwitchCurrentControlScheme("Gamepad2", devices);
                break;
            }
        }

        var rotation = Quaternion.AngleAxis(direction.y * kbdSensitivity, Vector3.up);
        lookDirection = rotation * transform.rotation * Vector3.forward;
        
        _analyticsManager = FindObjectOfType<AnalyticsManager>();
        StartGame = FindObjectOfType<StartGame>();
        camera = GetComponentInChildren<Camera>();
        _jetParticles = GetComponent<DashJets>();
        cameraController = FindObjectOfType<CameraSwitch>();
        _hudManager = FindObjectOfType<HUDManager>();
        flashManager = GetComponent<CharacterFlash>();
        menu = FindObjectOfType<PausedMenu>();
        // menu.SwitchMenuState();
        
        _jetParticles.Shoot();

        // controller = GetComponent<CharacterController>();
        // controller = gameObject.GetComponent(typeof(CharacterController)) as CharacterController;
        if (camera == null)
        {
            camera = backupCamera.GetComponentInChildren<Camera>();
            followingCamera = false;
        }

        currentCooldown = 0;
        startMomentum = momentum;
        
        _analyticsManager.HealthEvent(gameObject, playerHealth);
        _analyticsManager.StockUpdate(gameObject, Stock);

        _capsule = GetComponent<CapsuleCollider>();
        
        StartGame.PlayerHealthUpdate(playerNumber, playerHealth);
        // StartGame.PlayerStockUpdate(playerNumber, ) TODO: Should stocks be stored here too?
    }

    public void SpawnGun()
    {
        if (weapon)
        {
            weapon.SetActive(true);
            return;
        }
        
        weapon = Instantiate(weaponType, weaponHandBone);
        weapon.transform.localPosition = new Vector3(.6f, 6f, 0);
        weapon.transform.localRotation = Quaternion.Euler(-113, -180, 90);
        weapon.transform.localScale = new Vector3(.5f, .5f, .5f);
        
        var gunController = weapon.GetComponent<GunController>();
        gunController.setOwner(this);
        gunController.setSecondary(null);
        // gunController.setSecondary(mds.secondaryTypes[mds.playerSecondaries[playerNumber]]);
    }

    public void HideGun()
    {
        if (weapon)
            weapon.SetActive(false);
    }

    public void OnMovement(InputValue value)
    {
        // Vector3 temp = value.Get<Vector3>();
        // if (temp.magnitude > 0) print("3d vector input: " + temp);
        Vector2 temp2 = value.Get<Vector2>();
        moveDirection = new Vector3(-temp2.y, 0, temp2.x);
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
        // print("ROTATION (DASH): " + direction);
        direction = value.Get<Vector3>();
    }

    private void UpdateLookDirection()
    {
        if (!kbdHeld) return;
        if (direction.sqrMagnitude == 0) return;

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
        if (!currentlyDead && weapon != null && weapon.activeSelf)
        {
            weapon.GetComponent<GunController>().PrimaryFire();
        }
    }

    public void OnPrimaryFireCharge()
    {
        if (!currentlyDead && weapon != null && weapon.activeSelf)
        {
            weapon.GetComponent<GunController>().ApplyChargeSizeMultiplier();
            weapon.GetComponent<GunController>().PrimaryFire();
        }
    }

    public void OnSecondaryFire()
    {
        if (!currentlyDead && weapon != null && weapon.activeSelf)
        {
            if (weapon.GetComponent<GunController>().SecondaryFire()) {
                animator.Play(Animations.Lobbing);
            }
        }
    }

    public void OnEnterMenu() {
        if (menuOnCooldown) return;
        menuOnCooldown = true;
        menu.SwitchMenuState();
        StartCoroutine(MenuCooldown());
    }

    private IEnumerator MenuCooldown()
    {
        yield return null;
        menuOnCooldown = false;
    }

    public void OnDash()
    {
        if (currentCooldown > 0)
        {
            // print("Dash on cooldown!");
            return;
        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/Actions/Dash", GetComponent<Transform>().position);
        currentCooldown = GlobalStats.dashCooldown;
        _hudManager.UseStamina(playerNumber);
        StartCoroutine(Dash());
    }

    private bool _dashing;
    IEnumerator Dash() {
        float startTime = Time.time;
        //_jetParticles.Shoot();
        _jetParticles.SetStartSpeed(10);
        animator.Dashing = true;
        _dashing = true;

        while (Time.time < startTime + dashDuration) {
            if (moveDirection.magnitude > 0) {
                // controller.Move(moveDirection.normalized * Time.deltaTime * dashIntensity * GetDashBonus());    
                rb.AddForce((dashIntensity * GetDashBonus()) * moveDirection.normalized);    
            }
            else {
                rb.AddForce((dashIntensity * GetDashBonus()) * lookDirection.normalized);
            }
            yield return null;
        }
        // _jetParticles.Stop();
        _jetParticles.SetStartSpeed();
        animator.Dashing = false;
        _dashing = false;
    }

    void RespawnRoutine() {
        if (StartGame == null) {
            var tutorialSpawner = FindObjectOfType<StartTutorial>();
            tutorialSpawner.RespawnPlayer(this);
        } else {
            StartGame.RespawnPlayer(this);
        }
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
                // print("Not on the right plane:: on life plane");
                transform.position = new Vector3(0, 100, 0);
            }
            
            deathCooldown -= Time.deltaTime;
            if (deathCooldown <= 0)
            {
                RespawnRoutine();
                // transform.position = pos;
                // print("Player position after respawn is: " + transform.position + ", should be " + pos);
                ResetAttributes();
                _hudManager.ChangeHealth(playerNumber, GlobalStats.baseHealth);
                
                _analyticsManager.RespawnEvent(gameObject);
                _analyticsManager.StockUpdate(gameObject, Stock); // maybe put all these events in the game manager rather then in each player and bullet
                    
                return;
            }
        } else {
            if (transform.position.y <= -1) {
                print("Not on the right plane:: phased through the floor");
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
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

        if (!isGrounded())
        {
            animator.XSpeed = 0;
            animator.YSpeed = 0;
            animator.AnimationSpeed = 1;
            return;
        }
        // if (state != PlayerState.Aiming) {

        if (lookDirection.magnitude >= 0.5f)
        {
            Quaternion newAngle = Quaternion.LookRotation(lookDirection, Vector3.up);
            //print("LOOK VALUE: " + lookDirection + " ADJUSTED ANGLE: " + newAngle);

            // transform.rotation = Quaternion.RotateTowards(transform.rotation, newAngle, sensitivity * Time.deltaTime);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, newAngle, sensitivity * Time.deltaTime));
        }

        Vector3 newMove = new Vector3(-lookDirection.x,0,lookDirection.z).normalized;
        var animationMovement = Quaternion.LookRotation(newMove) * moveDirection;
        if (newMove.sqrMagnitude != 0) {
        }
        animator.XSpeed = animationMovement.x;
        animator.YSpeed = animationMovement.z;
        if (!currentlyDead && moveDirection.magnitude >= 0.1f && !animator.Landing)
        {
            // Handle the actual movement
            moveDirection.y = 0;

            animator.AnimationSpeed = /*speed **/ GetSpeedBonus() * momentum;
            // rb.MovePosition(transform.position + moveDirection.normalized*(speed * animator.AnimationSpeed*Time.deltaTime)); // todo temporary
            if (momentum < maxMomentum)
                momentum += 0.1f * Time.deltaTime;
        }
        else
        {
            momentum = startMomentum;
        }
    }

    private void LateUpdate()
    {       
        var mag = animator.transform.localPosition.magnitude;
        if (mag != 0 &&
            !currentlyDead &&
            !animator.Landing &&
            !_dashing &&
            isGrounded()) // todo you cant go up on ledges 
            // rb.MovePosition(transform.position + moveDirection.normalized * mag);       todo reenable this
            rb.MovePosition(transform.position + moveDirection.normalized * (0.01f * GetSpeedBonus() * speed * momentum));
        else if (!currentlyDead && !animator.Landing && !_dashing && !isGrounded()) {
            moveDirection.y = -10;
            rb.MovePosition(transform.position + moveDirection.normalized * momentum);
        }
    }

    bool isGrounded()
    {
        var bounds = _capsule.bounds;
        return Physics.Raycast(bounds.center, Vector3.down, bounds.extents.y + 0.1f);
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
            flashManager.InvincibilityFlash();
            return false;
        }

        // print("P" + playerNumber + " TOOK " + damageAmount + " dmg >> HP = " + playerHealth);
        playerHealth = Mathf.Max(0, Mathf.Round((playerHealth - damageAmount) * 10) / 10);
        flashManager.DamageFlash();

        // StartGame.PlayerHealthUpdate(playerNumber, playerHealth);
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
            
            HideGun();
            if (StartGame)
                StartGame.ProcessDeath(playerNumber);
            
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

    // For use with powerups!
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Powerup")
        {
            PowerupDrop powerup = collider.gameObject.GetComponent<PowerupDrop>();
            effects.Add(powerup.GiveEffect());
            FMODUnity.RuntimeManager.PlayOneShot("event:/Actions/PowerUppickup", GetComponent<Transform>().position);
            if (powerup.requiresWeapon) {
                // TODO: Make this generally-applicable. Right now the only weapon powerup is the fire rate one...
                weapon.GetComponent<GunController>().ClearPrimaryCooldown();
            }
            powerup.removePowerup(); //todo make this script trackable and keep trac of powerups
            Destroy(collider.gameObject);
        } else if (collider.gameObject.tag == "Weapon") {
            print("PICKED UP A SECONDARY!");
            WeaponDrop newSecondary = collider.gameObject.GetComponent<WeaponDrop>();

            var gunController = weapon.GetComponent<GunController>();
            gunController.setSecondary(newSecondary.secondaryType, newSecondary.ammo);
            newSecondary.removePowerup();
            Destroy(collider.gameObject);
        }
    }

    public void AddEffect(Effect e) {
        effects.Add(e);
    }
}