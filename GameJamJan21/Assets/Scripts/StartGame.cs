using System;
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
    private Controller[] players;
    
    [SerializeField] private float deathCooldown; // Amount of time before a player respawns
    [SerializeField] private float invincibilityCooldown; // Amount of time after respawning that the player cannot die

    [Header("Colours")]
    [Tooltip("Lists have to be the same length")]
    public Color[] primaryColours;
    public Color[] accentColours;
    //public Color player2PrimaryColour=new Color(.22f,.11f,.055f);
    //public Color player2AccentColour= Color.magenta;

    // Start is called before the first frame update
    void Start()
    {
        if (primaryColours.Length != accentColours.Length) throw new Exception("colour lists must be the same length");
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
        players = new Controller[Mathf.Min(playerCount, spawnPoints.Length)];
        var i=0;
        foreach (GameObject spawn in spawnPoints) {
            if (i >= players.Length) { break; }
            print("Spawning a player");
            Vector3 playerPos = spawnPoints[i].transform.position;
            playerPos.Set(playerPos.x, playerPos.y + 0.25f, playerPos.z);
            GameObject player = Instantiate(playerPrefab, playerPos, spawnPoints[i].transform.rotation, transform);
            player.name = "Player " + i;
            player.GetComponent<Controller>().playerNumber = i;
            playerStocks[i] = GlobalStats.defaultStockCount;
            PlayerStockUpdate(i, playerStocks[i]);

            if (i < primaryColours.Length)
            {
                var colourizer = player.GetComponent<PlayerColourizer>();
                colourizer.PrimaryColour = primaryColours[i];
                colourizer.SecondaryColour = accentColours[i];
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

    public void RespawnPlayer(Controller player)
    {
        player.Stock--;
        var playerNumber = player.playerNumber;
        PlayerStockUpdate(playerNumber, player.Stock);
        PlayerHealthUpdate(playerNumber, GlobalStats.baseHealth);
        print("STOCKS: " + players[0].Stock + "/" + players[1].Stock);

        if (player.Stock > 0) {
            var spawnpoint = levelManager.GetSpawnPoints()[playerNumber];
            // TODO: For the future...Make sure the player spawns at an open spawn point.
            print("PLAYER " + playerNumber + " RESPAWNED at " + spawnpoint);
            var playerTransform = player.transform;
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
