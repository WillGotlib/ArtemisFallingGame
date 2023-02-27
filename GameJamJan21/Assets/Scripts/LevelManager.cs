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
    [SerializeField] private MatchDataScriptable matchDataScriptable;
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
        levels = matchDataScriptable.levels;
        powerUpManager = FindObjectOfType<PowerupManager>();
        levelsAmount = levels.Length;
        selectedLevel = matchDataScriptable.levelIdx;
        MakeLevel();
    }
    
    /* TEMPORARY */
    public void IncrementLevel()
    {
        selectedLevel = (selectedLevel + 1) % levels.Length;
    }

    public void EndLevel(int playerNumber) {
        if (playerNumber == 0) {
            matchDataScriptable.p1Wins += 1;
        } else {
            matchDataScriptable.p2Wins += 1;
        }
        if (matchDataScriptable.p1Wins < matchDataScriptable.numGames / 2 + 1 &&
                matchDataScriptable.p2Wins < matchDataScriptable.numGames / 2 + 1)
        {
            SceneManager.LoadScene("MidMatchMenu");
        } else {
            ResetData();
            SceneManager.LoadScene("Menu2");
        }
    }

    private void ResetData() {
        matchDataScriptable.p1Wins = 0;
        matchDataScriptable.p2Wins = 0;
        matchDataScriptable.levelIdx = 0;
    }
}