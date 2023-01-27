using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;           
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6f;
    public float sensitivity = 5f;
    private bool aiming_status = false;
    public GameObject projectile;
    private GameObject bullet;
    float turnSmoothVelocity;
    Vector3 moveDirection;
    Vector3 lookDirection;
    new Camera camera;
    bool followingCamera = true;
    GameObject cameraController;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponentInChildren<Camera>();
        cameraController = GameObject.Find("CameraControl");
        if (camera == null) {
            camera = GameObject.Find("Top-Down Camera").GetComponentInChildren<Camera>();
            followingCamera = false;
        }
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
        if (cameraController != null) {
            cameraController.GetComponent<CameraSwitch>().SwitchCamera();
        }
    }

    public void OnLook(InputValue value)
    {
        // Read value from control. The type depends on what type of controls.
        // the action is bound to.
        lookDirection = value.Get<Vector3>();
        lookDirection = lookDirection * sensitivity * -1;

        // IMPORTANT: The given InputValue is only valid for the duration of the callback.
        //            Storing the InputValue references somewhere and calling Get<T>()
        //            later does not work correctly.
    }

    public void OnAim()
    {
        if (aiming_status == false) {
            startAiming();
            aiming_status = true;
        } else {
            stopAiming();
            aiming_status = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (followingCamera == true)
            camera.transform.localRotation = Quaternion.Euler(lookDirection);

        if (aiming_status == false) {
            if (moveDirection.magnitude >= 0.1f) {

                // Camera relative stuff
                /**Vector3 forward = camera.transform.forward;
                Vector3 right = camera.transform.right;
                Vector3 forwardRelative = moveDirection.z * forward;
                Vector3 rightRelative = moveDirection.x * right;
                Vector3 relativeMove = forwardRelative + rightRelative;

                float turnSmoothTime = 2f;
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativeMove), turnSmoothTime * Time.deltaTime);**/
                
                controller.Move(moveDirection * speed * Time.deltaTime);
            }

            if (Input.GetMouseButtonDown(1)) {
                startAiming();
            }
                
        } else {
            if (Input.GetMouseButtonDown(1))
                stopAiming();
        }
    }

    void startAiming() {
        aiming_status = true;
        GameObject bullet = Object.Instantiate(projectile, gameObject.transform, false);
        Vector3 cur_pos = gameObject.transform.position;
        bullet.transform.position = new Vector3(cur_pos[0] + 0.5f, cur_pos[1] + 0.2f, cur_pos[2] + 0.5f);
    }

    void stopAiming() {
        aiming_status = false;
        Object[] allBullets = Object.FindObjectsOfType(typeof(GameObject));
        foreach(GameObject obj in allBullets) {
            if(obj.transform.name == "Bullet Parent(Clone)"){
                Destroy(obj);
            }
        }
        Destroy(bullet);
    }
}
