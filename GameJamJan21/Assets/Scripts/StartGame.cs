using Online;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject[] spawnPoints;
    private Scene _simulatorScene;
    private PhysicsScene _physicsScene;
    [SerializeField] private Transform _objects;
    // Start is called before the first frame update
    void Start()
    {
        CreatePhysicsScene();
        if (FindObjectOfType<NetworkManager>() != null)
            StartOnlineGame();
        else
            StartLocalGame();
    }

    private void StartOnlineGame()
    {
        var spawnpoint = spawnPoints[GRPC.GetIndex()];
        var o = Instantiate(playerPrefab, spawnpoint.transform.position, spawnpoint.transform.rotation)
            .GetComponent<NetworkedPlayerController>();
        o.controlled = true;

        FindObjectOfType<NetworkManager>().RegisterObject(o);

        foreach (var spawn in spawnPoints)
        {
            Destroy(spawn);
        }
    }

    private void StartLocalGame(){
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