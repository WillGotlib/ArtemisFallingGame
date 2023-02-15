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
    public string targetTag;
    [SerializeField] private Transform _objects;

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
        spawnPoints = GameObject.FindGameObjectsWithTag(targetTag);
        int i = 0;
        CreatePhysicsScene();
        while (i < playerCount && i < spawnPoints.Length) {
            print("Spawning a player");
            Vector3 playerPos = spawnPoints[i].transform.position;
            playerPos.Set(playerPos.x, playerPos.y + 0.25f, playerPos.z);
            GameObject player = Instantiate(playerPrefab, playerPos, spawnPoints[i].transform.rotation);
            player.GetComponent<Controller>().playerNumber = i;
            // Destroy(spawnPoints[i]);
            i++;
        }
    }

    public Vector3 RespawnPlayer(int playerNumber)
    {
        print("RESPAWNED!");
        // TODO: Make sure the player spawns at an open spawn point.
        Vector3 playerPos = spawnPoints[playerNumber].transform.position;
        playerPos.Set(playerPos.x, playerPos.y + 0.25f, playerPos.z);
        return playerPos;
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
