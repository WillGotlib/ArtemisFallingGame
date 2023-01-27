using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public GameObject playerPrefab;
    GameObject[] spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint");

        foreach (GameObject spawn in spawnPoints) {
            Instantiate(playerPrefab, spawn.transform.position, spawn.transform.rotation);
            Destroy(spawn);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
