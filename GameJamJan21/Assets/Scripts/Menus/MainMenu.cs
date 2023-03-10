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

    public void QuitGame() {
        Debug.Log("Good Bye");
        Application.Quit();
    }
}