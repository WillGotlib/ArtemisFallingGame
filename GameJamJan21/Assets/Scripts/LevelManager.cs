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

    public GameObject[] GetSpawnPoints()
    {
        return instantiated.GetComponent<Level>().playerSpawnPoints;
    }

    private void Awake()
    {
        MakeLevel();
    }
}
