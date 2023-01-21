using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Rigidbody rb;
    public int maxBounces;
    public ScoreUI scoring;

    // Start is called before the first frame update
    void Start()
    {
        // We'll give a sample movement vector (comment out later!)
        rb = GetComponent <Rigidbody> ();
        rb.velocity = Vector3.back;
        scoring = GameObject.FindObjectOfType<ScoreUI>();
    }

    // Update is called once per frame
    void Update()
    {
        // Change to be player position
        Ray preview = new Ray(rb.position, rb.velocity);
        RaycastHit hit;
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
            scoring.UpdateScore();
        } else {
            // Ricochet
            ContactPoint contact = collision.contacts[0];
            Vector3 oldvel = rb.velocity;
            float speed = oldvel.magnitude;
            
            rb.velocity = Vector3.Reflect(oldvel.normalized, contact.normal);
            print("Old velocity: " + oldvel.ToString() + " Old speed: " + speed.ToString() + " New vel: " + rb.velocity.ToString() + " New speed: " + rb.velocity.magnitude.ToString());

            // Subtract bounces and maybe destroy
            maxBounces -= 1;
            if (maxBounces < 1) {
                Destroy(gameObject);
            }
        }

    }
}
