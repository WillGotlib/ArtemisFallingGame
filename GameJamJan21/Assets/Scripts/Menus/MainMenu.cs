using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_Text version;
    public MatchDataScriptable matchDataScriptable;

    public void Start()
    {
        version.text = Application.version;
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