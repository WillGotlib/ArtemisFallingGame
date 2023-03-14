using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Actions/RocketExplosion", GetComponent<Transform>().position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
