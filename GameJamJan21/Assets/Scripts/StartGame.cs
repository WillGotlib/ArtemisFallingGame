using Online;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject[] spawnPoints;

    // Start is called before the first frame update
    void Start()
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

    // Update is called once per frame
    void Update()
    {
    }
}