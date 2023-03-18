using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] GameObject PreMatchMenu;
    [SerializeField] Button PreMatchButton;


    public void Awake() {
        if (mds.skipMainMenu) {
            mds.skipMainMenu = false;
            PreMatchButton.onClick.Invoke();
        }
    }

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
}