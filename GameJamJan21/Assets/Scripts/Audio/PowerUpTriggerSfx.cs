using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpTriggerSfx : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Actions/PowerUppickup", GetComponent<Transform>().position);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}