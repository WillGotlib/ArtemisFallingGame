using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFire : MonoBehaviour
{
    public float rotationSpeed = 0.3f;
    public float bulletSpeed = 3f;
    // 0 = Not fired
    // 1 = In flight
    // 2 = Finished/finishing flight
    private int fire_status = 0;
    private Vector3 motion;
    private Vector3 m_EulerAngleVelocity;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        m_EulerAngleVelocity = new Vector3(0, 100, 0);   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fire_status = 1;
        }

        if (fire_status == 0) {
            PreShotOrienting();
        }
        else if (fire_status == 1) {
            InFlightBulletMove();
        }
        else {
            print("Bullet path over");
        }
    }

    void PreShotOrienting() {
        transform.Rotate(0, Input.GetAxisRaw("Horizontal") * rotationSpeed, 0);
    }

    void InFlightBulletMove() {
        print(transform.forward);
        transform.position += transform.forward * Time.deltaTime * bulletSpeed;
    }
}
