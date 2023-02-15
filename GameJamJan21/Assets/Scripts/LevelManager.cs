using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int selectedLevel = 0;
    [SerializeField] private GameObject[] levels = {};
    public int levelsAmount { get; private set; }

    private GameObject instantiated;
    
    public void MakeLevel()
    {
        if (levels.Length == 0 || selectedLevel < 0 || selectedLevel > levelsAmount)
            throw new Exception("select valid level");

        if (instantiated != null)
        {
            Destroy(instantiated);
        }

        var level = levels[selectedLevel];
        instantiated = Instantiate(level, transform);
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
        levelsAmount = levels.Length;
    }
    
    /* TEMPORARY */
    public void IncrementLevel()
    {
        selectedLevel = (selectedLevel + 1) % levels.Length;
    }
}