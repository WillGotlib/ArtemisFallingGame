using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashZone : MonoBehaviour
{
    public float timeRemaining = 5;
    public float splashRadius;
    public float explosionDamage;

    public bool damageOverTime;
    public bool explodesOnImpact;

    // Note: These fields only matter if damageOverTime == true.
    public float damageOverTimeDamage = 0.1f;
    private bool damageOverTimeActive = false;
    public float damageOverTimeCooldown = 0.2f;
    private float damageOverTimeRemaining = 0.2f;

    public ParticleSystem explosion;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(splashRadius, 1, splashRadius);
        explosion.transform.localScale *= Mathf.Max(splashRadius / 2f, 1);

        var dur = explosion.main.duration;
        explosion.Play();
        Invoke(nameof(DestroyParticleSystem),dur);
    }

    // Update is called once per frame
    void Update()
    {   
        if (damageOverTimeActive) {
            damageOverTimeRemaining -= Time.deltaTime;
        }
        if (timeRemaining > 0)
        {
            Color objectColour = this.GetComponent<MeshRenderer>().material.color;
            float fadeAmount = objectColour.a + (1 * Time.deltaTime);

            objectColour = new Color(objectColour.r, objectColour.g, objectColour.b, fadeAmount);
            this.GetComponent<MeshRenderer>().material.color = objectColour;
            timeRemaining -= Time.deltaTime;
        }
        else {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider collider) {
        // print("splash zone trigger was set off");
        if (explodesOnImpact && collider.gameObject.tag == "Player") {
            Controller playerEntered = collider.gameObject.GetComponent<Controller>();
            playerEntered.InflictDamage(explosionDamage);
        }
    }

    void OnTriggerStay(Collider collider) {
        if (damageOverTime && collider.gameObject.tag == "Player") {
            if (damageOverTimeRemaining <= 0) { // The countdown has expired. Inflict the damage
                Controller playerInside = collider.gameObject.GetComponent<Controller>();
                playerInside.InflictDamage(damageOverTimeDamage);
                damageOverTimeActive = false;
                damageOverTimeRemaining = damageOverTimeCooldown;
            } else if (damageOverTimeRemaining == damageOverTimeCooldown) {  // We need to start the countdown.
                damageOverTimeActive = true;
            }
        }
    }

    private void DestroyParticleSystem()
    {
        Destroy(explosion.gameObject);
    }
}
