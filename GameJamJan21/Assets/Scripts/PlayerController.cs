using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;           
using UnityEngine.InputSystem.Controls;

public enum PlayerState {
    Free,
    Locked,
    Aiming
}

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6f;
    public float sensitivity = 1;
    public GameObject weaponType;
    private GameObject weapon;
    // private bool bulletInChamber;

    float turnSmoothVelocity;
    Vector3 moveDirection;
    Vector3 lookDirection;
    new Camera camera;
    bool followingCamera = true;
    GameObject cameraController;
    public float gravity = 0.000001f;
    PlayerState state;
    public float dashIntensity = 10;
    public float dashCooldown = 5;
    float currentCooldown;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponentInChildren<Camera>();
        cameraController = GameObject.Find("CameraControl");
        if (camera == null) {
            camera = GameObject.Find("Top-Down Camera").GetComponentInChildren<Camera>();
            followingCamera = false;
        }
        state = PlayerState.Free;
        currentCooldown = 0;
        Debug.Log("Spawning in Weapon");
        weapon = Object.Instantiate(weaponType, gameObject.transform, false);
        Vector3 cur_pos = this.transform.position;
        weapon.transform.position = new Vector3(cur_pos[0] + 0.25f, cur_pos[1] + 0.2f, cur_pos[2] + 0.2f);
        weapon.GetComponent<GunController>().setOwner(this);
    }

    public void hitByShot() {
        LockState();
        Destroy(gameObject);
    }

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
        if (cameraController != null && state != PlayerState.Locked) {
            cameraController.GetComponent<CameraSwitch>().SwitchCamera();
        }
    }

    public void OnLook(InputValue value)
    {
        // Read value from control. The type depends on what type of controls.
        // the action is bound to.
        if (state != PlayerState.Locked) {
            lookDirection = value.Get<Vector3>();
            // lookDirection = lookDirection * sensitivity * -1;
        }
        // IMPORTANT: The given InputValue is only valid for the duration of the callback.
        //            Storing the InputValue references somewhere and calling Get<T>()
        //            later does not work correctly.
    }

    public void OnAim()
    {
        if (state != PlayerState.Locked) {
            if (state != PlayerState.Aiming) {
                startAiming();
            } else {
                stopAiming();
            }
        }
    }

    public void OnFire() {
        weapon.GetComponent<GunController>().PrimaryFire();
    }

    public void OnDash() {
        if (currentCooldown <= 0) {
            var abs_x = Mathf.Abs(moveDirection.x);
            var abs_z = Mathf.Abs(moveDirection.z);
            if (abs_x == 0 && abs_z == 0) {
                return;
            }
            currentCooldown = dashCooldown;
            controller.Move(moveDirection * speed * Time.deltaTime * dashIntensity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCooldown > 0)
            currentCooldown -= Time.deltaTime;

        if (followingCamera == true)
            camera.transform.localRotation = Quaternion.Euler(lookDirection);

        CharacterController controller = gameObject.GetComponent(typeof(CharacterController)) as CharacterController;
        if (!controller.isGrounded) {
            Vector3 fall = new Vector3(0, -(gravity), 0);
            controller.Move(fall * Time.deltaTime);
        }
        // if (state != PlayerState.Aiming) {
        
        if (lookDirection.magnitude >= 0.5f) {
            // print("LOOK VALUE: " + lookDirection + " TRANSFORM: " + this.transform.position);
            this.transform.Rotate(lookDirection);
        }
        if (moveDirection.magnitude >= 0.1f) {

            // Camera relative stuff
            /**Vector3 forward = camera.transform.forward;
            Vector3 right = camera.transform.right;
            Vector3 forwardRelative = moveDirection.z * forward;
            Vector3 rightRelative = moveDirection.x * right;
            Vector3 relativeMove = forwardRelative + rightRelative;
            **/
            moveDirection.y = 0;

            float playerAngle = Vector3.SignedAngle(Vector3.forward, transform.forward, Vector3.up) + 90;
            Quaternion rotation = Quaternion.AngleAxis(playerAngle, Vector3.up);
            // print("INITIAL DIRECTION: " + moveDirection + " ANGLE: " + playerAngle + " TRANSFORM DIR: " + rotation * moveDirection);
            // float turnSmoothTime = 2f;
            // float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            // float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), turnSmoothTime * Time.deltaTime);

            controller.Move((rotation * moveDirection).normalized * speed * Time.deltaTime);
        }
        // }
    }

    void startAiming() {
        // AimState();
        // GameObject bullet = Object.Instantiate(projectile, gameObject.transform, false);
        // Vector3 cur_pos = gameObject.transform.position;
        // bullet.transform.position = new Vector3(cur_pos[0] + 0.5f, cur_pos[1] + 0.2f, cur_pos[2] + 0.5f);
        // bullet.GetComponent<BulletFire>().setShooter(this);
    }

    void stopAiming() {
        // FreeState();
        Object[] allBullets = Object.FindObjectsOfType(typeof(GameObject));
        foreach(GameObject obj in allBullets) {
            if(obj.transform.name == "Bullet Parent(Clone)" && obj.transform.parent == gameObject.transform){
                Destroy(obj);
            }
        }
        Destroy(weapon);
    }

    public void FreeState() {
        state = PlayerState.Free;
    }

    public void LockState() {
        state = PlayerState.Locked;
    }

    public void AimState() {
        state = PlayerState.Aiming;
    }
}
