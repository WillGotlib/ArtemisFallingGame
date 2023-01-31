using System;
using UnityEngine;
using UnityEngine.InputSystem;           
using UnityEngine.InputSystem.Controls;

public enum FiringState
{
    NotFired,
    InFlight,
    Finished,
    Destroyed, // this state is so that the finished case only runs once
}

public class BulletFire : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject bullet;
    public int maxBounces = 6;

    public int splashRadius = 1;
    public GameObject splashZone;
    
    public float rotationSpeed = 0.3f;
    public float bulletSpeed = 3f;
    [NonSerialized] public FiringState fireStatus = FiringState.NotFired;

    private Vector3 motion;
    private Vector3 m_EulerAngleVelocity;
    private Vector3 vel;

    [NonSerialized] public ScoreUI scoring;
    private AudioSource _audioBullet;
    private PlayerController shooter;
    private Vector3 lookDirection;
    

    // Start is called before the first frame update
    void Start()
    {
        m_EulerAngleVelocity = new Vector3(0, 100, 0);   
        rb = GetComponent<Rigidbody> ();
        rb.velocity = new Vector3(0,0,0);
        scoring = GameObject.FindObjectOfType<ScoreUI>();
        print("Created bullet.");
        
        _audioBullet = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (fireStatus)
        {
            case FiringState.NotFired:
                PreShotOrienting();
                break;
            case FiringState.InFlight:
                InFlightBulletMove();
                break;
            case FiringState.Finished:
                print("Bullet path over");
                fireStatus = FiringState.Destroyed;
                break;
            default:
                break;
        }
        vel = rb.velocity;
    }

    public void Fire(Vector3 direction)
    {
        fireStatus = FiringState.InFlight;
        rb = GetComponent<Rigidbody>();
        print("Forward for this bullet: " + this.transform.forward);
        rb.velocity = direction.normalized * bulletSpeed;
        
        // Play sound
        _audioBullet = GetComponent<AudioSource>(); // Necessary?
        _audioBullet.Play(0);
        Debug.Log("Shot bullet!");
    }

    void PreShotOrienting() {
        transform.Rotate(0, lookDirection.x * rotationSpeed, 0);
        //transform.Rotate(Input.GetAxisRaw("Vertical") * rotationSpeed, 0, 0);
    }

    void InFlightBulletMove() {
        // print(rb.velocity);
        rb.velocity = transform.forward * bulletSpeed;
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
        } else if (collision.gameObject.tag == "Player") {
            print("Encountered player");
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.hitByShot();
            finishShot();
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
        
        reflectedVelo.y = 0;
        rb.velocity = reflectedVelo.normalized * bulletSpeed;
        print("Old velocity: " + oldvel.ToString() + " Old speed: " + speed.ToString() + " New vel: " + rb.velocity.ToString() + " New speed: " + rb.velocity.magnitude.ToString());

        // Subtract bounces and maybe destroy
        maxBounces -= 1;
        if (maxBounces < 1) {
            finishShot();
        }
    }

    void finishShot() {
        rb.velocity = new Vector3(0,0,0);
        bullet.GetComponent<MeshRenderer>().enabled = false;
        fireStatus = FiringState.Finished;
        Destroy(gameObject);
    }

    public void setShooter(PlayerController player) {
        shooter = player;
    }
}
