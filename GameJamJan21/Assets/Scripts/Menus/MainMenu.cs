using Online;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_Text version;
    public MatchDataScriptable mds;

    public void Start()
    {
        ResetLanding();
        version.text = Application.version;
    }
    
    public void Awake()
    {
        if (mds.skipMainMenu) {
            mds.skipMainMenu = false;
            PreMatchButton.onClick.Invoke();
        }
        
        // kill any existing network manager in case coming back from a game
        var networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager != null)
            Destroy(networkManager.gameObject);
    }

    [SerializeField] GameObject PreMatchMenu;
    [SerializeField] Button PreMatchButton;

    public void PlayGame() {
        if (PausedMenu.isPaused == true) {
            Time.timeScale = 1f;
            PausedMenu.isPaused = false;
        }

        SceneManager.LoadScene("Gameplay");
    }

    public void PlayTutorial() {
        if (PausedMenu.isPaused == true) {
            Time.timeScale = 1f;
            PausedMenu.isPaused = false;
        }

        SceneManager.LoadScene("Tutorial");
    }

    public void ResetLanding()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponent<Controller>().HideGun();
            player.GetComponentInChildren<AnimationUtils>().Landing = false;
        }
    }

    public void ShowPrematchMenu() 
    { 
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
            player.GetComponentInChildren<AnimationUtils>().PlayLanding();
        }    
    }

    public static void QuitGame() {
        Debug.Log("Good Bye");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
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