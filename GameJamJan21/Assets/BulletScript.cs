using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Rigidbody rb;
    public int maxBounces;
    // Start is called before the first frame update
    void Start()
    {
        // We'll give a sample movement vector (comment out later!)
        rb = GetComponent <Rigidbody> ();
        rb.velocity = Vector3.back;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check tag for Transient or Reflector
        // Reflect if applicable
        if (collision.gameObject.tag == "Transient")
        {
            // Do nothing and pass through
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.gameObject.GetComponent<Collider>());
            // Call the add score function
        } else if (collision.gameObject.tag == "Reflector") {
            // Ricochet
            ContactPoint contact = collision.contacts[0];
            
            rb.velocity = Vector3.Reflect(transform.forward, contact.normal);

            // Subtract bounces and maybe destroy
            maxBounces -= 1;
            if (maxBounces < 1) {
                Destroy(gameObject);
            }
        }

    }
}
