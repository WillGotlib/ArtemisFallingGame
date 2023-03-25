using System;
using Online;
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

    // [Header("Colours")]
    // [Tooltip("Lists have to be the same length")]
    // public Color[] primaryColours;
    // public Color[] accentColours;
    // //public Color player2PrimaryColour=new Color(.22f,.11f,.055f);
    // //public Color player2AccentColour= Color.magenta;
    public MatchDataScriptable mds;

    // Start is called before the first frame update
    void Start()
    {
        if (mds.primaryColours.Length != mds.accentColours.Length) throw new Exception("colour lists must be the same length");
        StartMatch();
    }
    public void StartMatch()
    {
        foreach (Transform player in transform)
            Destroy(player.gameObject);
        
        levelManager = FindObjectOfType<LevelManager>();
        var spawnPoints = levelManager.GetSpawnPoints();
        // CreatePhysicsScene();
        players = new Controller[Mathf.Min(playerCount, spawnPoints.Length)];

        var manager = FindObjectOfType<NetworkManager>();
        if (manager != null)
            StartOnlineGame(spawnPoints, manager);
        else
            StartLocalGame(spawnPoints);
    }

    private void StartOnlineGame(GameObject[] spawnPoints, NetworkManager networkManager)
    {
        var spawnpoint = spawnPoints[Connection.GetIndex()];
        var player = spawnPlayer(Connection.GetIndex(),spawnpoint.transform);
        
        var o = player.GetComponent<NetworkedPlayerController>();
        o.controlled = true;

        networkManager.RegisterObject(o);
        if (Connection.GetIndex() % 2 == 0)
            FindObjectOfType<PausedMenu>().SwitchMenuState();
    }

    private void StartLocalGame(GameObject[] spawnPoints)
    {
        for (var i = 0; i < players.Length; i++)
        {
            var spawn = spawnPoints[i].transform;
            spawnPlayer(i, spawn);
        }
    }

    private GameObject spawnPlayer(int index, Transform spawnPoint)
    {
        print("Spawning a player");
        Vector3 playerPos = spawnPoint.transform.position;
        playerPos.Set(playerPos.x, playerPos.y + 0.25f, playerPos.z);
        GameObject player = Instantiate(playerPrefab, playerPos, spawnPoint.transform.rotation, transform);
        player.GetComponent<Controller>().playerNumber = index;
        player.name = "Player " + index;
        players[index] = player.GetComponent<Controller>();
        // playerStocks[i] = GlobalStats.defaultStockCount;
        PlayerStockUpdate(index, GlobalStats.defaultStockCount);

        if (index < mds.primaryColours.Length)
        {
            int playerIndex = mds.playerColourSchemes[index];
            var colourizer = player.GetComponent<PlayerColourizer>();
            colourizer.PrimaryColour = mds.primaryColours[playerIndex];
            colourizer.SecondaryColour = mds.accentColours[playerIndex];
            colourizer.initialColourize();
        }
        player.GetComponentInChildren<AnimationUtils>().Landing = true;
        return player;
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
        print("STOCKS: " + players[0]?.Stock + "/" + players[1]?.Stock);

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
