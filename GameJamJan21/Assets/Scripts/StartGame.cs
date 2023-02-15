using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public GameObject playerPrefab;
    private LevelManager levelManager;
    private Scene _simulatorScene;
    private PhysicsScene _physicsScene;

    public int playerCount; 
    // Number of players participating in this game
    public int stockCount;
    // Number of times a player can die before they are out of the game
    private int[] playerStocks; // Stocks of each player

    [SerializeField] private float deathCooldown; // Amount of time before a player respawns
    [SerializeField] private float invincibilityCooldown; // Amount of time after respawning that the player cannot die

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        StartMatch();
    }
    public void StartMatch()
    {
        foreach (Transform player in transform)
            Destroy(player.gameObject);
        
        var spawnPoints = levelManager.GetSpawnPoints();
        // spawnPoints = GameObject.FindGameObjectsWithTag(targetTag);
        CreatePhysicsScene();
        var i=0;
        foreach (GameObject spawn in spawnPoints) {
            print("Spawning a player");
            Vector3 playerPos = spawnPoints[i].transform.position;
            playerPos.Set(playerPos.x, playerPos.y + 0.25f, playerPos.z);
            GameObject player = Instantiate(playerPrefab, playerPos, spawnPoints[i].transform.rotation, transform);
            player.GetComponent<Controller>().playerNumber = i;
            i++;
        }
    }

    public void RespawnPlayer(Transform playerTransform, int playerNumber)
    {
        var spawnpoint = levelManager.GetSpawnPoints()[playerNumber];
        print("PLAYER " + spawnpoint + " RESPAWNED!");
        // TODO: Make sure the player spawns at an open spawn point.
        Vector3 playerPos = spawnpoint.transform.position;
        playerPos.Set(playerPos.x, playerPos.y + 0.25f, playerPos.z);
        
        print("Respawn Position: " + playerPos);
        playerTransform.position = playerPos;
        playerTransform.rotation = spawnpoint.transform.rotation;
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
