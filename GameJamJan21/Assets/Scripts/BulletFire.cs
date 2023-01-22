using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;           
using UnityEngine.InputSystem.Controls;

public class BulletFire : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject bullet;
    public int maxBounces = 6;
    
    public float rotationSpeed = 0.3f;
    public float bulletSpeed = 3f;
    // 0 = Not fired
    // 1 = In flight
    // 2 = Finished/finishing flight
    private int fire_status = 0;

    private Vector3 motion;
    private Vector3 m_EulerAngleVelocity;
    private Vector3 vel;
    private Vector3 lookDirection;

    public ScoreUI scoring;
    AudioSource audioBullet;

    // Start is called before the first frame update
    void Start()
    {
        m_EulerAngleVelocity = new Vector3(0, 100, 0);   
        rb = GetComponent <Rigidbody> ();
        // rb.velocity = Vector3.back;
        scoring = GameObject.FindObjectOfType<ScoreUI>();
    }

    public void OnFire()
    {
        fire_status = 1;
        rb.velocity = transform.forward * bulletSpeed;
        // Play sound
        audioBullet = GetComponent<AudioSource>();
        audioBullet.Play(0);
    }

    public void OnBulletLook(InputValue value) {
        lookDirection = value.Get<Vector3>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject == null || rb == null) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && fire_status == 0)
        {
            print("Pressed space!");
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
        vel = rb.velocity;
    }

    void PreShotOrienting() {
        transform.Rotate(0, lookDirection.x * rotationSpeed, 0);
        //transform.Rotate(Input.GetAxisRaw("Vertical") * rotationSpeed, 0, 0);
    }

    void InFlightBulletMove() {
        // print(rb.velocity);
        // rb.velocity = transform.forward * bulletSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        print("collided with something");
        // Check tag for Transient or Reflector
        // Reflect if applicable
        if (collision.gameObject.tag == "Transient")
        {
            print("Encountered transient object");
            // Do nothing and pass through
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
            // Call the add score function
            scoring.UpdateScore();
            // Destroy but keep velocity!
            Destroy(collision.gameObject);
            rb.velocity = vel;
        } else  {
            // Ricochet
            ricochetBullet(collision);
        }
    }

    void ricochetBullet(Collision collision) {
            ContactPoint contact = collision.contacts[0];
            Vector3 oldvel = vel;
            float speed = oldvel.magnitude;

            print("CONTACT NORMAL = " + contact.normal.ToString());

            Vector3 reflectedVelo = Vector3.Reflect(oldvel.normalized, contact.normal);
            float rot = 90 - Mathf.Atan2(reflectedVelo.z, reflectedVelo.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, rot, 0);
            
            rb.velocity = reflectedVelo.normalized * bulletSpeed;
            print("Old velocity: " + oldvel.ToString() + " Old speed: " + speed.ToString() + " New vel: " + rb.velocity.ToString() + " New speed: " + rb.velocity.magnitude.ToString());

            // Subtract bounces and maybe destroy
            maxBounces -= 1;
            if (maxBounces < 1) {
                rb.velocity = new Vector3(0,0,0);
                bullet.GetComponent<MeshRenderer>().enabled = false;
                // Destroy(gameObject);
            }
    }

    void OnCollisionExit(Collision collision) {
        // pass
    }
}
