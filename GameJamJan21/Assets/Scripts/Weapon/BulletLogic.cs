using System;
using System.Collections;
using Analytics;
using Google.Protobuf;
using UnityEngine;


public class BulletLogic : MonoBehaviour, ITrackableScript
{
    private GlobalStats stats;
    
    public static float splashDamage = 0.5f; // TODO: Delete this.
    
    private Rigidbody _rb;
    public GameObject bullet;
    public GameObject splashZone;

    public int maxBounces = 4;
    [NonSerialized] public int bounced;
    [SerializeField] private float _bulletSpeed = 5f;
    [SerializeField] public float cooldown;
    [SerializeField] private MatchDataScriptable matchData;
    private bool _ricocheted = false;
    
    [Tooltip("spin amount every frame for x y z in degrees per second")]
    public Vector3 spin;
    private Vector3 vel;
    private Vector3 lookDirection;
    
    private Vector3 previousBounceLocation;

    public float maxFlightTimeSeconds = 10;
    private Coroutine expiration;

    private Controller shooter;
    public TrailRenderer trail;
    private AudioSource _audioBullet;
    private AnalyticsManager _analytics;
    private BulletDynamics _dynamics;
    private StartGame startGame;
    private int playerNumber;

    [Header("Labels")]
    public Sprite thumbnail;
    public string label;

    private const string TrailColourName = "_trail_colour";
    private int _trailColourId;

    private void Awake()
    {
        startGame = FindObjectOfType<StartGame>();
        _rb = GetComponent<Rigidbody>();
        _analytics = FindObjectOfType<AnalyticsManager>();
        _dynamics = GetComponent<BulletDynamics>();

        _trailColourId = Shader.PropertyToID(TrailColourName);
    }

    // Update is called once per frame
    void Update()
    {
        bullet.transform.Rotate(spin * Time.deltaTime);
        
        // TODO: Look at this. Wasteful making this run every frame...
        _rb.velocity = vel.normalized * _bulletSpeed * GetBulletSpeedBonus();
    }

    public void Fire(Vector3 direction)
    {
        _rb.velocity = direction.normalized * (_bulletSpeed * GetBulletSpeedBonus());
        vel = _rb.velocity;
        
        expiration = StartCoroutine(ExpirationTimer());
        trail.enabled = true;
        for (int i = 0; i < startGame.players.Length; i++) {
            if (startGame.players[i] == shooter) {
                playerNumber = i;
            }
        }
        
        trail.material.SetColor(_trailColourId, matchData.primaryColours[playerNumber]);
        // Play sound
        FMODUnity.RuntimeManager.PlayOneShot("event:/Actions/Rocket Launch", GetComponent<Transform>().position);
    }

    private IEnumerator ExpirationTimer() {
        yield return new WaitForSeconds(maxFlightTimeSeconds);
        Debug.Log("bullet expired");
        FinishShot(false);
    }

    float GetBulletDamage() {
        // The main function that is used to find bullet damage
        return splashDamage * BulletDamageMultiplier();
    }

    float GetBulletSpeedBonus() {
        return Mathf.Sqrt(bounced) / 4 + 1;
        // return Mathf.Max(1, )
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
        } else if (collision.gameObject.tag == "Player") {
            EncounterPlayer(collision);
        } else if (collision.gameObject.tag == "Powerup") {
            // Do nothing lol
        } else if (collision.gameObject.tag == "Grenade") {
            EncounterGrenade(collision);
        }
        else  {
            // Ricochet
            ricochetBullet(collision);
        }
    }

    void EncounterGrenade(Collision collision) {
        collision.gameObject.GetComponent<BulletLogic>().FinishShot(true);
        FinishShot(false);
    }

    void EncounterTransient(Collision collision) {
        // Do nothing and pass through
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
        
            // Destroy but keep velocity!
            _rb.velocity = vel;
    }

    void EncounterPlayer(Collision collision) {
        Controller player = collision.gameObject.GetComponent<Controller>();
        float damage = GetBulletDamage();
        _analytics.DamageEvent(collision.gameObject,gameObject);
        player.InflictDamage(damage);
        FinishShot(BulletDamageMultiplier()!=0);
    }

    void ricochetBullet(Collision collision) {
        if ((transform.position - previousBounceLocation).sqrMagnitude < 0.01) {
            print("Ricocheted multiple times too close to previous position. Assume this wasn't good behaviour...");
            return;
        }
        previousBounceLocation = transform.position;
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
        // print($"Ricocheting Bullet!\n\tOld Vel: {oldvel}\tNew Vel: {vel}");
        // Rather than: _rb.velocity = -reflectedVelo.normalized * _bulletSpeed;

        // add to bounces tally and maybe destroy
        bounced++;
       
        // Modify model
        if (_dynamics != null) {
            float ratio = 1.0f * bounced / (maxBounces + 1);
            // _dynamics.BulletGrow(ratio);
            //print("Bounced: with ratio " + ratio + " --> bounced = " + bounced + ", maxBounces = " + maxBounces);
            _dynamics.BulletBrighten(ratio);
            transform.localScale += new Vector3(0.2f,0.2f,0.2f);
            _rb.mass *= 1.2f;
        }

        if (bounced > maxBounces) {
            FinishShot(true);
        }
        else
        
        {
            // store that it ricocheted for analytics
            _ricocheted = true;
            FMODUnity.RuntimeManager.PlayOneShot("event:/Actions/Richochet", GetComponent<Transform>().position);
        }
    }

    public void FinishShot(bool explode) {
        _rb.velocity = new Vector3(0,0,0);

        bullet.SetActive(false);
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
