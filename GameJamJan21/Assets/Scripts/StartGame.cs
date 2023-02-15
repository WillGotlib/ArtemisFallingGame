using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public GameObject playerPrefab;
    public LevelManager levelManager;
    private Scene _simulatorScene;
    private PhysicsScene _physicsScene;
    // Start is called before the first frame update
    void Start()
    {
        StartMatch();
    }
    public void StartMatch()
    {
        foreach (Transform player in transform)
            Destroy(player.gameObject);
        
        
        var spawnPoints = levelManager.GetSpawnPoints();
        int i = 0;
        CreatePhysicsScene();
        foreach (GameObject spawn in spawnPoints) {
            print("Spawning a player");
            Instantiate(playerPrefab, spawn.transform.position, spawn.transform.rotation, transform);
            // if 
            Destroy(spawn);
        }
    }

    void CreatePhysicsScene()
    {
        if (!_simulatorScene.isLoaded)
        {
            CreateSceneParameters parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
            _simulatorScene = SceneManager.CreateScene("Trajectory", parameters);
            _physicsScene = _simulatorScene.GetPhysicsScene();
        }

        foreach (var sim in GameObject.FindGameObjectsWithTag("SIMULATION"))
            Destroy(sim);

        foreach (Transform obj in levelManager.GetObstacles()) {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            ghostObj.tag = "SIMULATION";
            // ghostObj.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostObj, _simulatorScene);
        }
    }
}
