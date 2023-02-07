using System;
using UnityEngine;
using Online;


public class BulletLogic : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    public GameObject bullet;
    private int maxBounces;
    private float _bulletSpeed = 5f;

    public GameObject splashZone;

    private Vector3 vel;
    private Vector3 lookDirection;

    public float rotationSpeed = 0.3f;
    private AudioSource _audioBullet;
    private Controller shooter;
    private bool isGhost;
    
    private NetworkManager _networkedManager;
    private NetworkedBulletController _networkedBullet;

    // // Update is called once per frame
    // void Update()
    // {
    //     vel = rb.velocity;
    // }

    private void Awake()
    {
        _audioBullet = GetComponent<AudioSource>();

        _networkedManager = FindObjectOfType<NetworkManager>();
        _networkedBullet = GetComponent<NetworkedBulletController>();
        if (_networkedManager != null && _networkedBullet.controlled)
            _networkedManager.RegisterObject(_networkedBullet);
    }

    public void Fire(Vector3 direction, bool ghost)
    {
        _rb.velocity = direction.normalized * _bulletSpeed;
        vel = _rb.velocity;
        isGhost = ghost;
        
        // Play sound
        if (isGhost == false) {
            _audioBullet.Play(0);
            maxBounces = 4;
        }
        else {
            maxBounces = 3;
        }
    }

    void PreShotOrienting() {
        transform.Rotate(0, lookDirection.x * rotationSpeed, 0);
        //transform.Rotate(Input.GetAxisRaw("Vertical") * rotationSpeed, 0, 0);
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

            // print("CONTACT NORMAL = " + contact.normal.ToString());

            Vector3 reflectedVelo = Vector3.Reflect(oldvel.normalized, contact.normal);
            float rot = 90 - Mathf.Atan2(reflectedVelo.z, reflectedVelo.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, rot, 0);
            
            reflectedVelo.y = 0;
            _rb.velocity = reflectedVelo.normalized * _bulletSpeed;
            vel = _rb.velocity;

            // Subtract bounces and maybe destroy
            maxBounces -= 1;
            if (maxBounces < 1) {
                finishShot();
            }
    }

    void finishShot() {
        _rb.velocity = new Vector3(0,0,0);
        bullet.GetComponent<MeshRenderer>().enabled = false;
        if (isGhost == false) {
            print("Bullet terminating");
            GameObject splash = UnityEngine.Object.Instantiate(splashZone);
            splash.transform.position = this.transform.position;
        }
        Destroy(gameObject);
    }

    public void setShooter(Controller player) {
        shooter = player;
    }
    
    private void OnDestroy()
     {
         if (_networkedManager != null)
             _networkedManager.UnregisterObject(_networkedBullet);
     }
}
