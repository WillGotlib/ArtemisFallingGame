using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{

    [SerializeField] private Rigidbody _rb;
    private int maxBounces;
    public bool _isGhost = false; 
    private Vector3 vel;
    public float _bulletSpeed = 3f;

    public void Fire(Vector3 velocity, bool isGhost, int bounces) {
        _isGhost = isGhost;
        _rb.velocity = velocity;
        vel = _rb.velocity;
        maxBounces = bounces;
    }

    private void OnCollisionEnter(Collision collision) {
        if (_isGhost) {
            if (collision.gameObject.tag == "Transient")
                {
                print("Encountered transient object");
                // Do nothing and pass through
                Physics.IgnoreCollision(GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
                _rb.velocity = vel;
            } else  {
                // Ricochet
                ricochetBullet(collision);
            }
        }

        else {
            if (collision.gameObject.tag == "Transient")
            {
                print("Encountered transient object");
                // Do nothing and pass through
                Physics.IgnoreCollision(GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
                // Destroy but keep velocity!
                Destroy(collision.gameObject);
                _rb.velocity = vel;
            } else  {
                // Ricochet
                ricochetBullet(collision);
            }
        }
    }

    private void ricochetBullet(Collision collision) {
        ContactPoint contact = collision.contacts[0];
        Vector3 oldvel = vel;

        print("CONTACT NORMAL = " + contact.normal.ToString());

        Vector3 reflectedVelo = Vector3.Reflect(oldvel.normalized, contact.normal);
        float rot = 90 - Mathf.Atan2(reflectedVelo.z, reflectedVelo.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 90, 0);
            
        _rb.velocity = reflectedVelo.normalized * _bulletSpeed;
        vel = reflectedVelo.normalized * _bulletSpeed;

        // Subtract bounces and maybe destroy
        maxBounces -= 1;
        if (maxBounces < 1) {
            _rb.velocity = new Vector3(0,0,0);
            GetComponent<MeshRenderer>().enabled = false;
            Destroy(this.gameObject);
        }
    }
}
