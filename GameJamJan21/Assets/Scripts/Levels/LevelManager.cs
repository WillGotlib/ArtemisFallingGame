using System;
using Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    
    public int selectedLevel = 0;
    public int levelsAmount { get; private set; }

    private GameObject instantiated;
    private Level _level;
    private PowerupManager powerUpManager;

    [Header("If you were expecting to see a list of levels here,\ncheck Recurring/MatchData\n")]
    [SerializeField] private MatchDataScriptable mds;
    private GameObject[] levels = {};
    
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
        foreach (GameObject obstacle in GameObject.FindObjectsOfType<GameObject>()) {
            if (obstacle.tag == "Untagged"){
                obstacle.layer = LayerMask.NameToLayer("Obstacle");
            }
        }
        powerUpManager.SetLevel(GetLevel());
        _level = null;
        
        FindObjectOfType<AnalyticsManager>()?.ChangeMap(level.name);
    }

    private Level GetLevel()
    {
        if (_level)
            return _level;
        _level = instantiated.GetComponent<Level>();
        _level.SortSpawnPoints();
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
        levels = mds.levels;
        powerUpManager = FindObjectOfType<PowerupManager>();
        levelsAmount = levels.Length;
        selectedLevel = mds.levelIdx;
        MakeLevel();
    }
    
    /* TEMPORARY */
    public void IncrementLevel()
    {
        selectedLevel = (selectedLevel + 1) % levels.Length;
    }

    public void EndLevel(int playerNumber) {
        mds.playerWins[playerNumber]++;
        mds.lastWinner = playerNumber;
        SceneManager.LoadScene("VictoryMenu");
        // int threshold = mds.numGames / 2 + 1;
        // if (mds.playerWins[0] < threshold && mds.playerWins[1] < threshold) {
        //     SceneManager.LoadScene("MidMatchMenu");
        // } else {
        //     SceneManager.LoadScene("VictoryMenu");
        // }
    }

    
}