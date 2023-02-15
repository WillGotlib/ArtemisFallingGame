using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int selectedLevel = 0;
    [SerializeField] private GameObject[] levels;

    private GameObject instantiated;

    public GameObject MakeLevel()
    {
        if (levels.Length == 0 || selectedLevel < 0 || selectedLevel > levels.Length)
            throw new Exception("select valid level");

        if (instantiated != null)
        {
            Destroy(instantiated);
        }

        var level = levels[selectedLevel];
        instantiated = Instantiate(level, transform);
        return instantiated;
    }

    private Level GetLevel()
    {
        return instantiated.GetComponent<Level>();
    }

    public GameObject[] GetSpawnPoints()
    {
        return GetLevel().playerSpawnPoints;
    }

    public Transform GetObstacles()
    {
        return GetLevel().obstacles;
    }

    private void Awake()
    {
        MakeLevel();
    }
}