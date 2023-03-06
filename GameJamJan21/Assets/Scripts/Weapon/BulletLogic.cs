using System;
using System.Collections.Generic;
using System.Collections;
using Analytics;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.InputSystem;           
using UnityEngine.InputSystem.Controls;


public class BulletLogic : MonoBehaviour, ITrackableScript
{
    private GlobalStats stats;
    
    public static float splashDamage = 0.5f; // TODO: Delete this.

    [SerializeField] private Rigidbody _rb;
    public GameObject bullet;
    public int maxBounces = 3;
    [NonSerialized] public int bounced;
    [SerializeField] private float _bulletSpeed = 5f;
    [SerializeField] public float cooldown;

    public GameObject splashZone;

    private Vector3 vel;
    private Vector3 lookDirection;

    public float rotationSpeed = 0.3f;
    private AudioSource _audioBullet;
    private Controller shooter;
    private bool isGhost;

    private bool _ricocheted=false;

    public TrailRenderer trail;

    public float maxFlightTimeSeconds = 10;
    private Coroutine expiration;

    public int ghostBounces = 3;
    private AnalyticsManager _analytics;

    private void Start()
    {
        _analytics = FindObjectOfType<AnalyticsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Look at this. Wasteful making this run every frame...
        _rb.velocity = vel.normalized * _bulletSpeed;
    }

    public void Fire(Vector3 direction, bool ghost)
    {
        _rb.velocity = direction.normalized * _bulletSpeed;
        vel = _rb.velocity;
        isGhost = ghost;
        
        if (isGhost)
        {
            bounced = 0;
            maxBounces = ghostBounces;
            return;
        }
        expiration = StartCoroutine(ExpirationTimer());

        trail.enabled = true;
        // Play sound
        _audioBullet = GetComponent<AudioSource>();
        _audioBullet.Play(0);
    }

    private IEnumerator ExpirationTimer() {
        yield return new WaitForSeconds(maxFlightTimeSeconds);
        Debug.Log("bullet expired");
        finishShot(false);
    }

    void PreShotOrienting() {
        transform.Rotate(0, lookDirection.x * rotationSpeed, 0);
        //transform.Rotate(Input.GetAxisRaw("Vertical") * rotationSpeed, 0, 0);
    }

    float GetBulletDamage() {
        // The main function that is used to find bullet damage
        return splashDamage * BulletDamageMultiplier();
    }

    int BulletDamageMultiplier() {
        // The multiplier for the base splash damage. Separate for checking purposes
        return Mathf.Min(maxBounces, bounced);
    }


    void OnCollisionEnter(Collision collision)
    {
        // Check tag for Transient or Reflector
        // Reflect if applicable
        if (collision.gameObject.tag == "Transient")
        {
            EncounterTransient(collision);
        } else if (isGhost == false && collision.gameObject.tag == "Player") {
            EncounterPlayer(collision);
        } else if (collision.gameObject.tag == "Powerup") {
            // Do nothing lol
        } else  {
            // Ricochet
            ricochetBullet(collision);
        }
    }

    void EncounterTransient(Collision collision) {
            // Do nothing and pass through
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
            // Destroy but keep velocity!
            Destroy(collision.gameObject);
            _rb.velocity = vel;
    }

    void EncounterPlayer(Collision collision) {
            Controller player = collision.gameObject.GetComponent<Controller>();
            float damage = GetBulletDamage();
            _analytics.DamageEvent(collision.gameObject,gameObject);
            player.InflictDamage(damage);
            finishShot(BulletDamageMultiplier()!=0);
    }

    void ricochetBullet(Collision collision) {
            ContactPoint contact = collision.contacts[0];
            Vector3 oldvel = vel;
            float speed = oldvel.magnitude;

            Vector3 reflectedVelo = Vector3.Reflect(oldvel.normalized, contact.normal);
            float rot = Mathf.Atan2(reflectedVelo.x, reflectedVelo.z) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, rot, 0);
            
            reflectedVelo.y = 0;
            // print("CONTACT NORMAL = " + contact.normal.ToString() + "\t NEW VEL = " + reflectedVelo.ToString());
            _rb.velocity = reflectedVelo.normalized * _bulletSpeed;
            vel = _rb.velocity;
            // Rather than: _rb.velocity = -reflectedVelo.normalized * _bulletSpeed;

            // add to bounces tally and maybe destroy
            bounced++;
            if (bounced > maxBounces) {
                finishShot(true);
            }
            else
            {
                // store that it ricocheted for analytics
                _ricocheted = true;
            }
    }

    void finishShot(bool explode) {
        _rb.velocity = new Vector3(0,0,0);
        if (isGhost)
        {
            return;
        }

        bullet.GetComponent<MeshRenderer>().enabled = false;
        if (explode) {
            GameObject splash = Instantiate(splashZone);
            var pos = transform.position+Vector3.zero;
            pos.y = 0;
            splash.transform.position = pos;
        }
        StopCoroutine(expiration);
        Destroy(gameObject);
    }

    public void setShooter(Controller player) {
        shooter = player;
    }

    public ByteString GetAnalyticsFields()
    {
        var data = "";

        data += _ricocheted ? "r" : "f"; // store an r if a ricochet happened else it was f for flying
        _ricocheted = false;
        
        return ByteString.CopyFromUtf8(data);
    }

    public string GetAnalyticsName()
    {
        return "Bullet-Controller";
    }
}
