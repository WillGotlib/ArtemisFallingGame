using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject[] playerSpawnPoints;
    public GameObject[] powerupDropPoints;
    public Transform obstacles;
    public string nid; // NIDs must be unique per level

    public void SortSpawnPoints()
    {
        var sorted = false;
        while (!sorted)
        {
            sorted = true;
            for (var i = 0; i < playerSpawnPoints.Length - 1; i++)
            {
                var item = playerSpawnPoints[i];
                var nextItem = playerSpawnPoints[i + 1];
                if (nextItem.transform.position.z < item.transform.position.z)
                {
                    continue;
                }

                sorted = false;
                (playerSpawnPoints[i], playerSpawnPoints[i + 1]) = (nextItem, item);
            }
        }
    }
}