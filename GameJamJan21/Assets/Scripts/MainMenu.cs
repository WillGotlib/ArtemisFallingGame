using Online;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        // kill any existing network manager in case coming back from a game
        var networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager != null)
            Destroy(networkManager.gameObject);
    }

    public void StartLocal()
    {
        SceneManager.LoadScene("ConceptLevel", LoadSceneMode.Single);
    }

    public void StartMulti()
    {
        SceneManager.LoadScene("ServerSelector", LoadSceneMode.Single);
    }
}