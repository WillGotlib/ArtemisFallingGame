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

    public MatchDataScriptable mds;

    // Start is called before the first frame update
    void Start()
    {
        // 
        if (mds.primaryColours.Length != mds.accentColours.Length) throw new Exception("colour lists must be the same length");
        levelManager = FindObjectOfType<LevelManager>();
        _hudManager.InitHealth();
        StartMatch();
    }
    public void StartMatch()
    {
        foreach (Transform player in transform)
            Destroy(player.gameObject);
        
        var spawnPoints = levelManager.GetSpawnPoints();
        // spawnPoints = GameObject.FindGameObjectsWithTag(targetTag);
        // CreatePhysicsScene();
        players = new Controller[Mathf.Min(mds.numPlayers, spawnPoints.Length)];
        var i=0;
        foreach (GameObject spawn in spawnPoints) {
            if (i >= players.Length) { break; }
            print("Spawning a player");
            Vector3 playerPos = spawnPoints[i].transform.position;
            playerPos.Set(playerPos.x, playerPos.y + 0.25f, playerPos.z);
            GameObject player = Instantiate(playerPrefab, playerPos, spawnPoints[i].transform.rotation, transform);
            player.GetComponent<Controller>().playerNumber = i;
            player.name = "Player " + i;
            players[i] = player.GetComponent<Controller>();
            // playerStocks[i] = GlobalStats.defaultStockCount;
            PlayerStockUpdate(i, GlobalStats.defaultStockCount);

            if (i < mds.primaryColours.Length)
            {
                int playerIndex = mds.playerColourSchemes[i];
                var colourizer = player.GetComponent<PlayerColourizer>();
                colourizer.PrimaryColour = mds.primaryColours[playerIndex];
                colourizer.SecondaryColour = mds.accentColours[playerIndex];
                colourizer.initialColourize();
            }
            player.GetComponentInChildren<AnimationUtils>().Landing = true;
            
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

            player.GetComponentInChildren<AnimationUtils>().PlayLanding();
        }
        else {
            print("PLAYER " + playerNumber + " IS OUT!");
            int winner = 0;
            if (playerNumber == winner) winner = 1;
            levelManager.EndLevel(winner);
        }
    }
}
