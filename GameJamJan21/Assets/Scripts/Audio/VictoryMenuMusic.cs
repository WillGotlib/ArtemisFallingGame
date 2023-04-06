using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryMenuMusic : MonoBehaviour
{
    private static FMOD.Studio.EventInstance VictoryMusic;
    
    void Start()
    {
        VictoryMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Menu/Victory screen music");
        VictoryMusic.start();
        VictoryMusic.release();

    }
    
    private void OnDestroy()
    {
        VictoryMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
