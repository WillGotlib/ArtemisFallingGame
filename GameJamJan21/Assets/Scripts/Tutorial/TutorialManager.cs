using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialLevel;
    private GameObject instantiated;

    private void Awake() {
        MakeLevel();
    }

    public void MakeLevel()
    {

        if (instantiated != null)
        {
            Destroy(instantiated);
        }
        var level = tutorialLevel;
        instantiated = Instantiate(level, transform);
        foreach (GameObject obstacle in GameObject.FindObjectsOfType<GameObject>()) {
            if (obstacle.tag == "Untagged"){
                obstacle.layer = LayerMask.NameToLayer("Obstacle");
            }
        }
        tutorialLevel = null;
        
        //FindObjectOfType<AnalyticsManager>()?.ChangeMap(level.name);
    }

    public GameObject[] GetSpawnPoints()
    {
        return GetLevel().playerSpawnPoints;
    }

    public Transform GetObstacles()
    {
        return GetLevel().obstacles;
    }

    private Level GetLevel()
    {
        var _level = instantiated.GetComponent<Level>();
        _level.SortSpawnPoints();
        return _level;
    }

    public void EndLevel(int playerNumber) {
        SceneManager.LoadScene("Menu2");
    }
}
