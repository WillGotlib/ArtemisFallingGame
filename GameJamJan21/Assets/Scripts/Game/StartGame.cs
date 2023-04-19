using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Cinemachine;

public class StartGame : MonoBehaviour
{
    [SerializeField] GameObject ActiveCamera; // TODO: for tomorrow...This will have to change around now...

    public GameObject playerPrefab;
    private LevelManager levelManager;
    [SerializeField] private HUDManager _hudManager;

    public int playerCount; 
    // Number of players participating in this game
    public int stockCount;
    public Controller[] players;
    
    [SerializeField] private float deathCooldown; // Amount of time before a player respawns
    [SerializeField] private float invincibilityCooldown; // Amount of time after respawning that the player cannot die

    public MatchDataScriptable mds;

    private readonly GameObject[] _playerCameras = new GameObject[4];
    
    [SerializeField] private GameObject[] tutorialUI;
    private GameObject dynamicCamera;

    // Start is called before the first frame update
    void Start()
    {
        dynamicCamera = GameObject.Find("DynamicCamera");
        _playerCameras[0] = GameObject.Find("VirtualCameraPlayerOne"); 
        _playerCameras[1] = GameObject.Find("VirtualCameraPlayerTwo"); 
        _playerCameras[2] = GameObject.Find("VirtualCameraPlayerThree"); 
        _playerCameras[3] = GameObject.Find("VirtualCameraPlayerFour"); 
        if (mds.primaryColours.Length != mds.accentColours.Length) throw new Exception("colour lists must be the same length");
        levelManager = FindObjectOfType<LevelManager>();
        HandleTutorialUI();
        _hudManager.InitHealth();
        dynamicCamera.SetActive(true);
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
            players[i] = player.GetComponent<Controller>();
            players[i].playerNumber = i;
            player.name = "Player " + i;
            player.GetComponent<MovingUIPointer>().target = ActiveCamera.gameObject;

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

    private void HandleTutorialUI() {
        if (!mds.tutorial) {
            foreach (GameObject tut in tutorialUI) {
                tut.SetActive(false);
                if (tut.GetComponent<Renderer>() != null) {
                    tut.GetComponent<Renderer>().enabled = false;
                }
                if (tut.GetComponent<CanvasRenderer>() != null) {
                    tut.GetComponent<CanvasRenderer>().SetAlpha(0f);
                }
            }
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
        if (!CheckForElimination(playerNumber)) return -1;
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
        players[playerNumber].Stock--;
        PlayerStockUpdate(playerNumber, players[playerNumber].Stock);
        print("STOCKS: " + players[0].Stock + "/" + players[1].Stock);
        
        int winner = CheckForMatchEnding(playerNumber);
        if (winner != -1) {
            Debug.Log("winner decided");
            players[winner].isWinner = true;
            players[winner].invincibilityCooldown = 10f;
            // TODO: GAME IS OVER HERE. DO WHATEVER WE NEED TO DO (zoom in on player, etc...)
            var virtualCamera = _playerCameras[winner].GetComponent<CinemachineVirtualCamera>();
            StartCoroutine(VirtualCameraActivate(virtualCamera, winner));
        }
    }

    IEnumerator VirtualCameraActivate(CinemachineVirtualCamera virtualCamera, int winner) {
            yield return new WaitForSecondsRealtime(1);
            virtualCamera.Priority = 100;
            virtualCamera.DestroyCinemachineComponent<Cinemachine3rdPersonFollow>();
            dynamicCamera.SetActive(false);
            StartCoroutine(VictoryMotion(winner));

    }

    IEnumerator VictoryMotion(int winner)
    {
        yield return new WaitForSecondsRealtime(8);
        levelManager.EndLevel(winner);
    }

    private IEnumerator MatchEndDelay()
    {
        print("Match is over, but we will wait a few seconds before moving to victory screen.");
        yield return new WaitForSeconds(4f);
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
