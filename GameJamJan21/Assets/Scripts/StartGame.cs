using Online;
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
        
        CreatePhysicsScene();
        if (FindObjectOfType<NetworkManager>() != null)
            StartOnlineGame();
        else
            StartLocalGame();
    }

    private void StartOnlineGame()
    {
        var spawnPoints = levelManager.GetSpawnPoints();
        var spawnpoint = spawnPoints[GRPC.GetIndex()];
        var player = spawnPlayer(spawnpoint.transform);
        
        player.GetComponent<Controller>().playerNumber = GRPC.GetIndex();
        
        var o = player.GetComponent<NetworkedPlayerController>();
        o.controlled = true;

        FindObjectOfType<NetworkManager>().RegisterObject(o);
    }

    private void StartLocalGame()
    {
        var spawnPoints = levelManager.GetSpawnPoints();
        for (var i =0; i<spawnPoints.Length; i++) {
            print("Spawning a player");
            var player = spawnPlayer(spawnPoints[i].transform);
            player.GetComponent<Controller>().playerNumber = i;
        }
    }

    private GameObject spawnPlayer(Transform spawnPoint){
        var playerPos = spawnPoint.position + Vector3.zero; // make a copy of the vector
        playerPos.Set(playerPos.x, playerPos.y + 0.25f, playerPos.z);
        return Instantiate(playerPrefab, playerPos, spawnPoint.rotation, transform);
    }

    public void RespawnPlayer(Transform playerTransform, int playerNumber)
    {
        var spawnpoint = levelManager.GetSpawnPoints()[playerNumber];
        print("PLAYER " + spawnpoint + " RESPAWNED!");
        // TODO: Make sure the player spawns at an open spawn point.
        Vector3 playerPos = spawnpoint.transform.position + Vector3.zero;
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