using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using Analytics;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class TrailerCameraScript : MonoBehaviour
{
    // public string cameraName;
    public MatchDataScriptable mds;
    public GameObject ep;
    public int playerIndex;
    // Start is called before the first frame update
    void Start()
    {
        // GameObject cam = GameObject.Find (cameraName);
        // transform.SetParent(cam.transform);
        // BulletLogic bullet = GetComponent<BulletLogic>();
        // bullet.Fire(this.transform.forward * 2, false);
        // Time.timeScale = 0.15f;

        var colourizer = GetComponent<PlayerColourizer>();
        colourizer.PrimaryColour = mds.primaryColours[playerIndex];
        colourizer.SecondaryColour = mds.accentColours[playerIndex];
        colourizer.initialColourize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Terminate() {
        if (ep) ep.GetComponent<ExplodePlayer>()?.Explode();
    }
}
