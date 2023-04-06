using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    private static FMOD.Studio.EventInstance Music; 
    // Start is called before the first frame update
    void Start()
    {
        Music = FMODUnity.RuntimeManager.CreateInstance("event:/Menu/MainMenuMusic");
        Music.start();
        Music.release();
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    void Update()
    {
        
    }
}
