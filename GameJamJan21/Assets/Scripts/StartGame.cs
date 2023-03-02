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
    [SerializeField] private HUDManager _hudManager;

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
        playerStocks = new int[Mathf.Min(playerCount, spawnPoints.Length)];
        var i=0;
        foreach (GameObject spawn in spawnPoints) {
            if (i >= playerStocks.Length) { break; }
            print("Spawning a player");
            Vector3 playerPos = spawnPoints[i].transform.position;
            playerPos.Set(playerPos.x, playerPos.y + 0.25f, playerPos.z);
            GameObject player = Instantiate(playerPrefab, playerPos, spawnPoints[i].transform.rotation, transform);
            player.name = "Player " + i;
            player.GetComponent<Controller>().playerNumber = i;
            playerStocks[i] = GlobalStats.defaultStockCount;
            PlayerStockUpdate(i, playerStocks[i]);

            if (i != 0)
            {
                var colourizer = player.GetComponent<PlayerColourizer>();
                colourizer.PrimaryColour = new Color(1, .64f, 0);
                colourizer.SecondaryColour = Color.magenta;
            }
            
            i++;
        }
    }

    // Currently this is just to support the UI.
    public void PlayerHealthUpdate(int playerNumber, float playerHealth) {
        _hudManager.ChangeHealth(playerNumber, playerHealth);
    }
    
    public void PlayerStockUpdate(int playerNumber, int playerStock) {
        _hudManager.ChangeStock(playerNumber, playerStock);
    }

    public void RespawnPlayer(Transform playerTransform, int playerNumber)
    {
        playerStocks[playerNumber]--;
        PlayerStockUpdate(playerNumber, playerStocks[playerNumber]);
        PlayerHealthUpdate(playerNumber, GlobalStats.baseHealth);
        print("STOCKS: " + playerStocks[0] + "/" + playerStocks[1]);

        if (playerStocks[playerNumber] > 0) {
            var spawnpoint = levelManager.GetSpawnPoints()[playerNumber];
            // TODO: For the future...Make sure the player spawns at an open spawn point.
            print("PLAYER " + playerNumber + " RESPAWNED at " + spawnpoint);
            playerTransform.position = spawnpoint.transform.position;
            playerTransform.rotation = spawnpoint.transform.rotation;
        }
        else {
            print("PLAYER " + playerNumber + " IS OUT!");
            levelManager.EndLevel(playerNumber);
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
            ghostObj.GetComponent<Renderer>().enabled = false;
            var obj_scale = obj.gameObject.transform.lossyScale;
            ghostObj.transform.localScale = obj_scale;
            ghostObj.tag = "SIMULATION";
            SceneManager.MoveGameObjectToScene(ghostObj, _simulatorScene);
        }
    }
}
