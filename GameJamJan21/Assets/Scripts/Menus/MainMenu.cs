using Online;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_Text version;
    public MatchDataScriptable matchDataScriptable;

    public void Awake()
    {
        version.text = Application.version;
        // kill any existing network manager in case coming back from a game
        var networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager != null)
            Destroy(networkManager.gameObject);
    }

    public void QuitGame() {
        Debug.Log("Good Bye");
        Application.Quit();
    }
    
    public void StartLocal()
    {
        if (PausedMenu.isPaused == true) {
            Time.timeScale = 1f;
            PausedMenu.isPaused = false;
        }
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }

    public void StartMulti()
    {
        SceneManager.LoadScene("ServerSelector", LoadSceneMode.Single);
    }
}