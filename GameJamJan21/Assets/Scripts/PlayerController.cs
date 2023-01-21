using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6f;
    private bool aiming_status = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (aiming_status == 0) {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

            if (direction.magnitude >= 0.1f) {
                controller.Move(direction * speed * Time.deltaTime);
            }

            if (Input.GetMouseButtonDown(1)) {
                startAiming();
            }
                
        } else {
            if (Input.GetMouseButtonDown(1))
                aiming_status = false;
        }
    }

    void startAiming() {
        aiming_status = true;
    }
}
