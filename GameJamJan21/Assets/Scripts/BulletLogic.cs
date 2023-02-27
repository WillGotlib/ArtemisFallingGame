using System;
using Analytics;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.InputSystem;           
using UnityEngine.InputSystem.Controls;


public class BulletLogic : MonoBehaviour, ITrackableScript
{
    private GlobalStats stats;

    [SerializeField] private Rigidbody _rb;
    public GameObject bullet;
    public int maxBounces;
    [SerializeField] private float _bulletSpeed = 5f;

    public GameObject splashZone;
    [SerializeField] private float splashRadius;
    [SerializeField] private float splashDuration;

    private Vector3 vel;
    private Vector3 lookDirection;

    public float rotationSpeed = 0.3f;
    private AudioSource _audioBullet;
    private Controller shooter;
    private bool isGhost;

    private bool _ricocheted=false;

    public TrailRenderer trail;

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
        
        // Play sound
        if (isGhost == false)
        {
            trail.enabled = true;
            
            _audioBullet = GetComponent<AudioSource>();
            _audioBullet.Play(0);
            // maxBounces = GlobalStats.bulletMaxBounces;
        } else {
            maxBounces = 3;
        }
    }

    void PreShotOrienting() {
        transform.Rotate(0, lookDirection.x * rotationSpeed, 0);
        //transform.Rotate(Input.GetAxisRaw("Vertical") * rotationSpeed, 0, 0);
    }

    float GetBulletDamage() {
        // The main function that is used to find bullet damage
        return GlobalStats.bulletSplashDamage * BulletDamageMultiplier();
    }

    int BulletDamageMultiplier() {
        // The multiplier for the base splash damage. Separate for checking purposes
        return GlobalStats.bulletMaxBounces - maxBounces;
    }


    void OnCollisionEnter(Collision collision)
    {
        // print("collided with something");
        // Check tag for Transient or Reflector
        // Reflect if applicable
        if (collision.gameObject.tag == "Transient")
        {
            print("Encountered transient object");
            // Do nothing and pass through
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
            // Destroy but keep velocity!
            Destroy(collision.gameObject);
            _rb.velocity = vel;
        } else if (isGhost == false && collision.gameObject.tag == "Player") {
            print("Encountered player");
            Controller player = collision.gameObject.GetComponent<Controller>();
            float damage = GetBulletDamage();
            player.InflictDamage(damage);
            finishShot(BulletDamageMultiplier()!=0);
        } else if (collision.gameObject.tag == "Powerup") {
            // Do nothing lol
        } else  {
            // Ricochet
            ricochetBullet(collision);
        }
    }

    void ricochetBullet(Collision collision) {
            ContactPoint contact = collision.contacts[0];
            Vector3 oldvel = vel;
            float speed = oldvel.magnitude;

            Vector3 reflectedVelo = Vector3.Reflect(oldvel.normalized, contact.normal);
            float rot = 90 - Mathf.Atan2(reflectedVelo.z, reflectedVelo.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, rot, 0);
            
            reflectedVelo.y = 0;
            // print("CONTACT NORMAL = " + contact.normal.ToString() + "\t NEW VEL = " + reflectedVelo.ToString());
            _rb.velocity = reflectedVelo.normalized * _bulletSpeed;
            vel = _rb.velocity;
            // Rather than: _rb.velocity = -reflectedVelo.normalized * _bulletSpeed;

            // Subtract bounces and maybe destroy
            maxBounces -= 1;
            if (maxBounces < 1) {
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
        if (!isGhost) {
            bullet.GetComponent<MeshRenderer>().enabled = false;
            if (explode) {
                GameObject splash = Instantiate(splashZone);
                SplashZone splashManager = splash.GetComponent<SplashZone>();
                if (splashRadius == 0) {
                    splashManager.splashRadius = GlobalStats.bulletSplashRadius; 
                } else {
                    splashManager.splashRadius = splashRadius;
                }
                splashManager.splashDamage = GlobalStats.bulletSplashDamage;
                splashManager.timeRemaining = splashDuration;
                var pos = transform.position+Vector3.zero;
                pos.y = 0;
                splash.transform.position = pos;
            }
            Destroy(gameObject);
        }
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
