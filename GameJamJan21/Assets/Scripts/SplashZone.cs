using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashZone : MonoBehaviour
{
    public float timeRemaining = 5;

    // Start is called before the first frame update
    void Start()
    {
        print("Spawning splash damage");
    }

    // Update is called once per frame
    void Update()
    {
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
        print("splash zone trigger was set off");
        if (collider.gameObject.tag == "Player") {
            Destroy(collider.gameObject);
        }
    }
}