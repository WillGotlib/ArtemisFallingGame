using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    public int maxNumPowerups;
    public int scndsBetweenSpawns;
    private float curCooldown;
    private int curPowerups = 0;
    private Level _level;
    public GameObject[] powerups;

    // Start is called before the first frame update
    void Start()
    {
        curCooldown = scndsBetweenSpawns;   
    }

    // Update is called once per frame
    void Update()
    {
        curCooldown -= Time.deltaTime;
        if (curCooldown <= 0) {
            curCooldown = scndsBetweenSpawns;
            if (curPowerups < maxNumPowerups && _level != null)
                SpawnPowerup();
        }
    }

    void SpawnPowerup() {
        List<GameObject> validSpawns = new List<GameObject>();
        foreach (GameObject dropPoint in _level.powerupDropPoints) {
            if (!dropPoint.GetComponent<PowerupPoint>().Occupied) {
                validSpawns.Add(dropPoint);
            }
        }

        // Only if successfully spawned
        if (validSpawns.Count > 0) {
            var rand = new System.Random();
            GameObject target = validSpawns[rand.Next(validSpawns.Count)];
            GameObject chosenPowerup = ChoosePowerup(rand);
            GameObject newPowerup = Instantiate(chosenPowerup, target.transform.position, target.transform.rotation, target.transform);
            newPowerup.GetComponent<PowerupDrop>().SetRelatedPoint(target.GetComponent<PowerupPoint>());
            target.GetComponent<PowerupPoint>().Occupied= true;
            curPowerups += 1;
        }
    }

    GameObject ChoosePowerup(System.Random rand) {
        // Grabs a random powerup from the list. Separated for ease of access in the future
        // We also may want to modify this function to make it more intelligent
        return powerups[rand.Next(powerups.Length)];
    }

    public void SetLevel(Level level) {
        _level = level;
        foreach (GameObject dropPoint in _level.powerupDropPoints) {
            dropPoint.GetComponent<PowerupPoint>().Occupied = false;
        }
    }
}
