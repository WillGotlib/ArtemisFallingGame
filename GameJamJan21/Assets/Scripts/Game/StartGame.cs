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
                // int playerIndex = mds.playerColourSchemes[i];
                int playerIndex = i;
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

    // Check whether or not the player will be eliminated by this death. 
    // Note: Only call this when we know the player's going to die.
    public bool CheckForElimination(int playerNumber) {
        if (players[playerNumber].Stock - 1 > 0) {
            return false;
        } else {
            // This player will be eliminated on this death.
            print("PLAYER " + playerNumber + " IS OUT!");
            return true;
        }
    }
    
    // Check whether or not the match will end upon this player death. 
    // Note: Only call this when we know the player's going to die.
    public int CheckForMatchEnding(int playerNumber) {
        if (!CheckForElimination(playerNumber)) return false;
        int anyoneAlive = -1;
        for (int i = 0; i < mds.numPlayers; i++) {
            if (players[i].Stock > 0) {
                if (anyoneAlive == -1) anyoneAlive = i; // One person can be alive.
                else return -1; // Match hasn't ended.
            }
        }
        return anyoneAlive; // This player is the winner.
    }

    public void ProcessDeath(int playerNumber) {
        player.Stock--;
        PlayerStockUpdate(playerNumber, players[playerNumber].Stock);
        print("STOCKS: " + players[0].Stock + "/" + players[1].Stock);
        
        int winner = CheckForMatchEnding(playerNumber);
        if (winner != -1) {
            // TODO: GAME IS OVER HERE. DO WHATEVER WE NEED TO DO (zoom in on player, etc...)
            
            levelManager.EndLevel(winner);
        }
    }

    public void RespawnPlayer(Controller player)
    {
        var playerNumber = player.playerNumber;
        PlayerHealthUpdate(playerNumber, GlobalStats.baseHealth);

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
            // print("PLAYER " + playerNumber + " IS OUT!");
        }
    }
}