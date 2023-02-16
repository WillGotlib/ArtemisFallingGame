using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int selectedLevel = 0;
    [SerializeField] private GameObject[] levels = {};
    public int levelsAmount { get; private set; }

    private GameObject instantiated;
    private Level _level;
    private PowerupManager powerUpManager;
    
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
        powerUpManager.SetLevel(GetLevel());
        _level = null;
    }

    private Level GetLevel()
    {
        if (_level)
            return _level;
        _level = instantiated.GetComponent<Level>();
        return _level;
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
        powerUpManager = FindObjectOfType<PowerupManager>();
        levelsAmount = levels.Length;
        MakeLevel();
    }
    
    /* TEMPORARY */
    public void IncrementLevel()
    {
        selectedLevel = (selectedLevel + 1) % levels.Length;
    }
}