using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;           
using UnityEngine.InputSystem.Controls;


public class Controller : MonoBehaviour
{   
    public int playerNumber;

    public CharacterController controller;
    public float speed = 6f;
    public float sensitivity = 1;
    public GameObject weaponType;
    private GameObject weapon;
    private float playerHealth = GlobalStats.baseHealth;

    float turnSmoothVelocity;
    Vector3 moveDirection;
    Vector3 lookDirection;
    new Camera camera;
    bool followingCamera = true;
    GameObject cameraController;
    // public float gravity = 0.000001f; // TODO: OK to delete this?
    public float dashIntensity = 50;
    float currentCooldown;

    public float momentum = 0.85f;
    private float startMomentum;
    public float maxMomentum = 1.5f;
    public GameObject backupCamera;

    private bool currentlyDead;
    // These decrease as time goes on
    private float deathCooldown;
    private float invincibilityCooldown; 

    private StartGame playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.Find("PlayerManager").GetComponent<StartGame>();
        camera = GetComponentInChildren<Camera>();
        cameraController = GameObject.Find("CameraControl");
        if (camera == null) {
            camera = backupCamera.GetComponentInChildren<Camera>();
            followingCamera = false;
        }
        currentCooldown = 0;
        Debug.Log("Spawning in Weapon");
        weapon = Object.Instantiate(weaponType, gameObject.transform, false);
        Vector3 cur_pos = this.transform.position;
        weapon.transform.position = new Vector3(cur_pos[0] + 0.25f, cur_pos[1] + 0.2f, cur_pos[2] + 0.2f);
        weapon.GetComponent<GunController>().setOwner(this);
        startMomentum = momentum;
    }

    // public void HitByShot() {
    //     Destroy(gameObject);
    // }

    public void OnMovement(InputValue value)
    {
        // Read value from control. The type depends on what type of controls.
        // the action is bound to.
        moveDirection = value.Get<Vector3>();
        // IMPORTANT: The given InputValue is only valid for the duration of the callback.
        //            Storing the InputValue references somewhere and calling Get<T>()
        //            later does not work correctly.
    }

    public void OnSwitchCamera() {
        if (cameraController != null) {
            cameraController.GetComponent<CameraSwitch>().SwitchCamera();
        }
    }

    public void OnLook(InputValue value)
    {
        // Read value from control. The type depends on what type of controls.
        // the action is bound to.
        if (value.Get<Vector3>() != lookDirection) {
            // print("LOOK ANGLE: " + value.Get<Vector3>());
        }
        lookDirection = value.Get<Vector3>() * sensitivity;
            // lookDirection = lookDirection * sensitivity * -1;
        // IMPORTANT: The given InputValue is only valid for the duration of the callback.
        //            Storing the InputValue references somewhere and calling Get<T>()
        //            later does not work correctly.
    }

    public void OnFire() {
        if (!currentlyDead) {
            print("Fired");
            weapon.GetComponent<GunController>().PrimaryFire();
        }
    }

    public void OnDash() {
        print("Dashed");
        if (currentCooldown <= 0) {
            var abs_x = Mathf.Abs(moveDirection.x);
            var abs_z = Mathf.Abs(moveDirection.z);
            if (abs_x == 0 && abs_z == 0) {
                return;
            }
            currentCooldown = GlobalStats.dashCooldown;
            controller.Move(moveDirection * speed * Time.deltaTime * dashIntensity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentlyDead) {
            deathCooldown -= Time.deltaTime;
            if (deathCooldown <= 0) {
                deathCooldown = GlobalStats.deathCooldown;
                currentlyDead = false;
                Vector3 newPos = playerController.RespawnPlayer(playerNumber);
                print("Respawn Position: " + newPos);
                this.transform.position = newPos;
            }
        }
        if (currentCooldown > 0)
            currentCooldown -= Time.deltaTime;

        if (followingCamera == true)
            camera.transform.localRotation = Quaternion.Euler(lookDirection);

        CharacterController controller = gameObject.GetComponent(typeof(CharacterController)) as CharacterController;
        if (!controller.isGrounded) {
            Vector3 fall = new Vector3(0, -(1), 0);
            controller.Move(fall * Time.deltaTime);
        }
        // if (state != PlayerState.Aiming) {
        
        if (lookDirection.magnitude >= 0.5f) {
            Quaternion newAngle = Quaternion.LookRotation(lookDirection, Vector3.up);
            //print("LOOK VALUE: " + lookDirection + " ADJUSTED ANGLE: " + newAngle);
            this.transform.rotation = newAngle;
            // this.transform.Rotate(lookDirection);
        }
        if (moveDirection.magnitude >= 0.1f) {

            // Camera relative stuff
            /*
            Vector3 forward = camera.transform.forward;
            Vector3 right = camera.transform.right;
            Vector3 forwardRelative = moveDirection.z * forward;
            Vector3 rightRelative = moveDirection.x * right;
            Vector3 relativeMove = forwardRelative + rightRelative;
            */
            moveDirection.y = 0;

            // float playerAngle = Vector3.SignedAngle(Vector3.forward, transform.forward, Vector3.up) + 90;
            // Quaternion rotation = Quaternion.AngleAxis(playerAngle, Vector3.up);
            // print("INITIAL DIRECTION: " + moveDirection + " ANGLE: " + playerAngle + " TRANSFORM DIR: " + rotation * moveDirection);
            // float turnSmoothTime = 2f;
            // float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            // float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), turnSmoothTime * Time.deltaTime);

            controller.Move((moveDirection).normalized * speed * Time.deltaTime * momentum);
            if (momentum < maxMomentum)
                momentum += 0.1f * Time.deltaTime;
        } else {
            momentum = startMomentum;
        }
        // }
    }

    public bool InflictDamage(float damageAmount) {
        if (invincibilityCooldown > 0) {
            print("PLAYER TOOK NO DAMAGE.");
            return false;
        }
        print("PLAYER TOOK " + damageAmount + " DAMAGE || HEALTH = " + playerHealth);
        /* TODO: ADD PLAYER HEALTH STUFF */
        // Uncomment/fix the next stuff when health is in
        playerHealth = Mathf.Max(0, playerHealth - damageAmount);
        if (playerHealth <= 0) {
            print("PLAYER DIED");
            PlayerDeath();
        }
        return true;
    }

    private void PlayerDeath() {
        currentlyDead = true;
        // Vector3 newPos = this.transform.position += Vector3.up * 10; // TODO: CHANGE THIS. HOW DO WE "DE-ACTIVATE" THE PLAYER
        // print("NEW PLAYER POSITION: " + newPos);
        this.transform.position = new Vector3(0, 10, 0);
        // SetActive(false);
    }

    public void ResetAttributes() {
        playerHealth = GlobalStats.baseHealth;
        currentCooldown = GlobalStats.dashCooldown;
    }

    public float getHealth() {
        return playerHealth;
    }

}
