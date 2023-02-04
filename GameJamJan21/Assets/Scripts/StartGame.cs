using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public GameObject playerPrefab;
    GameObject[] spawnPoints;
    private Scene _simulatorScene;
    private PhysicsScene _physicsScene;
    [SerializeField] private Transform _objects;
    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
        int i = 0;
        CreatePhysicsScene();
        foreach (GameObject spawn in spawnPoints) {
            print("Spawning a player");
            Instantiate(playerPrefab, spawn.transform.position, spawn.transform.rotation);
            // if 
            Destroy(spawn);
        }
    }

    void CreatePhysicsScene()
    {
        CreateSceneParameters parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        _simulatorScene = SceneManager.CreateScene("Trajectory", parameters);
        _physicsScene = _simulatorScene.GetPhysicsScene();
        foreach (Transform obj in _objects) {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            // ghostObj.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostObj, _simulatorScene);
        }
    }
}
